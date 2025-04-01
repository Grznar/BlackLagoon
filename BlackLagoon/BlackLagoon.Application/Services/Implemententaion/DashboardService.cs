using BlackLagoon.Application.Common.Interfaces;
using BlackLagoon.Application.Common.Utility;
using BlackLagoon.Application.Services.Interface;
using BlackLagoon.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackLagoon.Application.Services.Implemententaion
{
    public class DashboardService : IDashBoardService
    {
        private readonly IUnitOfWork _unitOfWork;
        static int previousMonth = DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1;
        private readonly DateTime previousMonthStartDate = new DateTime(DateTime.Now.Year, previousMonth, 1);
        private readonly DateTime currentMonthStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<PieChartDto> GetBookingPieChartData()
        {
            var totalBookings = _unitOfWork.Bookings
            .GetAll(u => u.BookingDate >= DateTime.Now.AddDays(-30) && (u.Status != SD.StatusPending || u.Status != SD.StatusCancelled));
            var customerWithOneBooking = totalBookings.GroupBy(b => b.UserId).Where(x => x.Count() == 1).Select(x => x.Key).ToList();
            int bookingByNewCustomer = customerWithOneBooking.Count();

            int bookingByReturningCustomer = totalBookings.Count() - bookingByNewCustomer;

            var countByCurrentMoth =
            totalBookings.Count(u => u.BookingDate >= currentMonthStartDate && u.BookingDate <= DateTime.Now);
            var countByPreviousMoth =
           totalBookings.Count(u => u.BookingDate >= previousMonthStartDate && u.BookingDate <= currentMonthStartDate);


            int decreseRation = 100;
            if (countByPreviousMoth != 0)
            {
                decreseRation = Convert.ToInt32((countByCurrentMoth - countByPreviousMoth) / countByPreviousMoth * 100);

            }
            PieChartDto pieChart = new PieChartDto()
            {
                Labels = new string[] { "New Customer", "Returning Customer" },
                Series = new decimal[] { bookingByNewCustomer, bookingByReturningCustomer }
            };

            return pieChart;
        }

        public async Task<RadialBarChartDto> GetRegisteredUsersRadialChartData()
        {

            var totalUsers = _unitOfWork.Users.GetAll();
            var countByCurrentMoth =
            totalUsers.Count(u => u.CreationOfUser >= currentMonthStartDate && u.CreationOfUser <= DateTime.Now);
            var countByPreviousMoth =
           totalUsers.Count(u => u.CreationOfUser >= previousMonthStartDate && u.CreationOfUser <= currentMonthStartDate);

            RadialBarChartDto radialBarChartVM = new RadialBarChartDto()
            ;
            int decreseRation = 100;
            if (countByPreviousMoth != 0)
            {
                decreseRation = Convert.ToInt32((countByCurrentMoth - countByPreviousMoth) / countByPreviousMoth * 100);

            }
            radialBarChartVM.TotalCount = totalUsers.Count();
            radialBarChartVM.CountInCurrentMotnth = countByCurrentMoth;
            radialBarChartVM.HasRatioIncreased = currentMonthStartDate > previousMonthStartDate;
            radialBarChartVM.Series = new int[] { decreseRation };
            return SD.GetRadialBarChartModel(totalUsers.Count(), countByCurrentMoth, countByPreviousMoth);
        }

        public async Task<RadialBarChartDto> GetRevenue()
        {
            var totalBookings = _unitOfWork.Bookings.GetAll(u => u.Status != SD.StatusPending
            || u.Status == SD.StatusCancelled);

            var totalRevenue = Convert.ToInt32(totalBookings.Sum(u => u.TotalCost));

            var countByCurrentMonth = totalBookings.Where(u => u.BookingDate >= currentMonthStartDate &&
            u.BookingDate <= DateTime.Now).Sum(u => u.TotalCost);

            var countByPreviousMonth = totalBookings.Where(u => u.BookingDate >= previousMonthStartDate &&
            u.BookingDate <= currentMonthStartDate).Sum(u => u.TotalCost);

            return  (SD.GetRadialBarChartModel(totalRevenue, countByCurrentMonth, countByPreviousMonth));
        }

        public async Task<RadialBarChartDto> GetTotalBookingRadialChartData()
        {
            var totalBookings = _unitOfWork.Bookings
           .GetAll(u => u.Status != SD.StatusPending
           || u.Status != SD.StatusCancelled);
            var countByCurrentMoth =
            totalBookings.Count(u => u.BookingDate >= currentMonthStartDate && u.BookingDate <= DateTime.Now);
            var countByPreviousMoth =
           totalBookings.Count(u => u.BookingDate >= previousMonthStartDate && u.BookingDate <= currentMonthStartDate);

            RadialBarChartDto radialBarChartVM = new RadialBarChartDto()
            ;
            int decreseRation = 100;
            if (countByPreviousMoth != 0)
            {
                decreseRation = Convert.ToInt32((countByCurrentMoth - countByPreviousMoth) / countByPreviousMoth * 100);

            }
            radialBarChartVM.TotalCount = totalBookings.Count();
            radialBarChartVM.CountInCurrentMotnth = countByCurrentMoth;
            radialBarChartVM.HasRatioIncreased = currentMonthStartDate > previousMonthStartDate;
            radialBarChartVM.Series = new int[] { decreseRation };
            return radialBarChartVM;
        }
        

        
    }
}
