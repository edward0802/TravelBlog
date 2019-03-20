using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheWorldProject.Models
{
    public class WorldRepository : IWorldRepository
    {
        private WorldContext _context;
        private ILogger<WorldRepository> _logger;

        public WorldRepository(WorldContext context, ILogger<WorldRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void AddStop(string tripName, Stop newStop, string userName)
        {
            var trip = GetUserTripByName(tripName, userName);
            if (trip != null)
            {
                trip.Stops.Add(newStop); 
            }
        }

        public void AddTrip(Trip trip)
        {
            _context.Add(trip);
        }

        public IEnumerable<Trip> GetAllTrips()
        {
            _logger.LogInformation("Getting All Trips from the DataBase");
            return _context.Trips.ToList();
        }

        public Trip GetTripByName(string tripName)
        {
            return _context.Trips
                .Where(trip => trip.Name == tripName)
                .Include(t => t.Stops) 
                .FirstOrDefault();
        }

        public IEnumerable<Trip> GetTripsByUserName(string name)
        {
            return from trips in _context.Trips.Include(trip => trip.Stops).ToList() where trips.UserName == name select trips;
        }

        public Trip GetUserTripByName(string tripName, string userName)
        {
            return _context.Trips
                .Where(trip => trip.Name == tripName && trip.UserName == userName)
                .Include(t => t.Stops)
                .FirstOrDefault();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}