using BlackLagoon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackLagoon.Application.Common.Utility
{
    public static class SD
    {
        // Roles
        public const string Role_Customer = "User";
        public const string Role_Admin = "Admin";

        // Booking Status
        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusCheckIn = "CheckedIn";
        public const string StatusCompletted = "Completed";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefuned = "Refunded";

        public static int VillaRoomsAvaibleCount(int villaId, List<VillaNumber> villaNumbersList, DateOnly checkInDate, int nights,
           List<Booking> bookings)
        {
            List<int> bookinDate = new();
            int finalAvaibleRoom = int.MaxValue;
            var roomsInVilla = villaNumbersList.Where(u => u.VillaId == villaId).Count();
            for (int i = 0; i < nights; i++)
            {
                var villasBooked = bookings.Where(u => u.CheckInDate <= checkInDate.AddDays(i) && u.CheckOutDate >= checkInDate.AddDays(i) && u.VillaId == villaId);
                foreach (var booking in villasBooked)
                {
                    if (!bookinDate.Contains(booking.VillaNumber))
                    {
                        bookinDate.Add(booking.VillaNumber);
                    }
                }
                var totalAvaibleRooms=roomsInVilla - bookinDate.Count();
                if (totalAvaibleRooms == 0)
                {
                    return 0;
                }
                else
                {
                    if (finalAvaibleRoom > totalAvaibleRooms)
                    {
                        finalAvaibleRoom = totalAvaibleRooms;

                    }
                }
                   
            }
            return finalAvaibleRoom;
        }
    }
}
