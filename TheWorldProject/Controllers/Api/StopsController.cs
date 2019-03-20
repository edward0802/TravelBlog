using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWorldProject.Models;
using TheWorldProject.Services;
using TheWorldProject.ViewModels;

namespace TheWorldProject.Controllers.Api
{
    [Route("/api/trips/{tripName}/stops")]
    [Authorize]
    public class StopsController: Controller
    {
        private IWorldRepository _repository;
        private ILogger<StopsController> _logger;
        private GeoCoordsService _coordService;

        public StopsController(IWorldRepository repository, ILogger<StopsController> logger, GeoCoordsService coordService)
        {
            _repository = repository;
            _logger = logger;
            _coordService = coordService;
        }

        [HttpGet("")]
        public IActionResult Get(string tripName)
        {
            try
            {
                var trip = _repository.GetUserTripByName(tripName, User.Identity.Name);

                return Ok( Mapper.Map<IEnumerable<StopViewModel>>( trip.Stops.OrderBy(stop => stop.Order).ToList() ) ); // отримаємо в порядку їх створення у виді списка

            } catch (Exception ex)
            {
                _logger.LogError($"Failed to get stops:{ex}");
            }

            return BadRequest("Failed to get stops");
        }

        [HttpPost("")]
        public async Task<IActionResult> Post(string tripName, [FromBody]StopViewModel vm)
        {

            try
            {
                
                // If the VM is valid
                if(vm.Arrival > DateTime.Now) 
                {
                    ModelState.AddModelError("", "This date didn't become!");
                }
                if (ModelState.IsValid)
                {
                    var newStop = Mapper.Map<Stop>(vm);

                    //Lookup the Geocodes
                    var result = await _coordService.GetCoordsAsync(newStop.Name);
                    
                    if(!result.Success)
                    {
                        _logger.LogError($"Failed to get coords: {result.Message}");
                    }
                    else
                    {
                        newStop.Latitude = result.Latitude;
                        newStop.Longitude = result.Longitude;

                        // Save to the Database
                        _repository.AddStop(tripName, newStop, User.Identity.Name);

                        if (await _repository.SaveChangesAsync())
                            return Created($"/api/trips/{tripName}/stops/{newStop.Name}", Mapper.Map<StopViewModel>(newStop));
                    }

                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error occured while POST: {ex}");
                
            }

            return BadRequest($"Error occured while POST:");
        }


    }
}
