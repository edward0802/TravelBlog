using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheWorldProject.Models
{
    public interface IWorldRepository
    {
        IEnumerable<Trip> GetAllTrips();
        IEnumerable<Trip> GetTripsByUserName(string userName);
        void AddTrip(Trip trip);
        Task<bool> SaveChangesAsync();
        Trip GetTripByName(string thripName);
        Trip GetUserTripByName(string tripName, string userName);
        void AddStop(string tripName, Stop newStop, string userName);
        
    }
}