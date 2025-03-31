using BlackLagoon.Application.Common.Interfaces;
using BlackLagoon.Application.Common.Utility;
using BlackLagoon.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BlackLagoon.Controllers
{
    public class DashBoardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        static int previousMonth = DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1;
        private readonly DateTime previousMonthStartDate =new DateTime(DateTime.Now.Year, previousMonth, 1);
        private readonly DateTime currentMonthStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DashBoardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetTotalBookingRadialChartData()
        {
            var totalBookings = _unitOfWork.Bookings
            .GetAll(u => u.Status != SD.StatusPending
            || u.Status != SD.StatusCancelled);
            var countByCurrentMoth = 
            totalBookings.Count(u=>u.BookingDate>= currentMonthStartDate && u.BookingDate<=DateTime.Now);
            var countByPreviousMoth =
           totalBookings.Count(u => u.BookingDate >= previousMonthStartDate && u.BookingDate <= currentMonthStartDate);

            RadialBarChartVM radialBarChartVM = new RadialBarChartVM()
            ;
            int decreseRation = 100;
            if(countByPreviousMoth!=0)
            {
                decreseRation = Convert.ToInt32((countByCurrentMoth - countByPreviousMoth)/countByPreviousMoth*100);

            }
            radialBarChartVM.TotalCount = totalBookings.Count();
            radialBarChartVM.CountInCurrentMotnth=countByCurrentMoth;
            radialBarChartVM.HasRatioIncreased = currentMonthStartDate > previousMonthStartDate;
            radialBarChartVM.Series = new int[] { decreseRation };
            return Json(radialBarChartVM);
        }
        public async Task<IActionResult> GetRegisteredUsersRadialChartData()
        {
            var totalUsers = _unitOfWork.Users.GetAll();
            var countByCurrentMoth =
            totalUsers.Count(u => u.CreationOfUser >= currentMonthStartDate && u.CreationOfUser <= DateTime.Now);
            var countByPreviousMoth =
           totalUsers.Count(u => u.CreationOfUser >= previousMonthStartDate && u.CreationOfUser <= currentMonthStartDate);

            RadialBarChartVM radialBarChartVM = new RadialBarChartVM()
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
            return Json(GetRadialBarChartModel(totalUsers.Count(),countByCurrentMoth,countByPreviousMoth));
        }
        public async Task<IActionResult> GetRevenue()
        {
            var totalBookings = _unitOfWork.Bookings.GetAll(u => u.Status != SD.StatusPending
           || u.Status == SD.StatusCancelled);

            var totalRevenue = Convert.ToInt32(totalBookings.Sum(u => u.TotalCost));

            var countByCurrentMonth = totalBookings.Where(u => u.BookingDate >= currentMonthStartDate &&
            u.BookingDate <= DateTime.Now).Sum(u => u.TotalCost);

            var countByPreviousMonth = totalBookings.Where(u => u.BookingDate >= previousMonthStartDate &&
            u.BookingDate <= currentMonthStartDate).Sum(u => u.TotalCost);

            return Json(GetRadialBarChartModel(totalRevenue, countByCurrentMonth, countByPreviousMonth));
        }
        private static RadialBarChartVM GetRadialBarChartModel(int totalCount, double currentMothCount, double prevMounthCount)
        {
            RadialBarChartVM radialBarChart = new();
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
