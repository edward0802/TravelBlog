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
using TheWorldProject.ViewModels;

namespace TheWorldProject.Controllers.Api
{
    [Route("api/trips")]
    [Authorize] 
    public class TripsController : Controller
    {
        private IWorldRepository _repository;
        private ILogger<TripsController> _logger;

        public TripsController(IWorldRepository repository, ILogger<TripsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        [HttpGet("")] 
        public IActionResult Get()
        {
            try
            {
                var results = _repository.GetTripsByUserName(this.User.Identity.Name); // IEnumerable<Trip>
                return Ok(Mapper.Map<IEnumerable<TripViewModel>>(results));
            }
            catch(Exception ex)
            {
                _logger.LogError($"ex.Message: {ex.Message}");
                return BadRequest($"Error occurred: {ex.Message}");           
            }
        }

        [HttpPost("")] 
        public async Task<IActionResult> Post([FromBody]TripViewModel theTrip)
        {
            if (ModelState.IsValid)
            {
                var newTrip = Mapper.Map<Trip>(theTrip);

                newTrip.UserName = User.Identity.Name;

                _repository.AddTrip(newTrip);

                if (await _repository.SaveChangesAsync())
                {
                    return Created($"api/trips/{theTrip.Name}", Mapper.Map<TripViewModel>(newTrip)); 
                }
                else
                {
                    return BadRequest("Fail to save changes to the Database");
                }

            } 

            return BadRequest(ModelState);
        }



    }
}
