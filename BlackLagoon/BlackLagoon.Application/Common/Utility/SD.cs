using BlackLagoon.Domain.Entities;
using BlackLagoon.ViewModels;
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
        public static RadialBarChartDto GetRadialBarChartModel(int totalCount, double currentMothCount, double prevMounthCount)
        {
            RadialBarChartDto radialBarChart = new();
            int decreseRation = 0;

            // Pokud je předchozí měsíční hodnota větší než 0, provedeme výpočet
            if (prevMounthCount > 0)
            {
                double ratio = (currentMothCount - prevMounthCount) / prevMounthCount * 100;
                // Ošetření, zda hodnota nepřekračuje rozsah Int32
                if (ratio > int.MaxValue)
                    decreseRation = int.MaxValue;
                else if (ratio < int.MinValue)
                    decreseRation = int.MinValue;
                else
                    decreseRation = Convert.ToInt32(ratio);
            }
            else
            {
                // Pokud je prevMounthCount 0, můžeme definovat výchozí hodnotu, např. 0,
                // případně i jinou logiku podle obchodní logiky
                decreseRation = 0;
            }

            radialBarChart.TotalCount = totalCount;
            radialBarChart.CountInCurrentMotnth = (int)currentMothCount;
            radialBarChart.HasRatioIncreased = currentMothCount > prevMounthCount;
            radialBarChart.Series = new int[] { decreseRation };

            return radialBarChart;
        }
    }
}
