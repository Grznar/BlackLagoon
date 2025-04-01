using BlackLagoon.Application.Common.Interfaces;
using BlackLagoon.Application.Common.Utility;
using BlackLagoon.Application.Services.Interface;
using BlackLagoon.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlackLagoon.Controllers
{
    [Authorize(Roles =SD.Role_Admin)]
    public class DashBoardController : Controller
    {
        IDashBoardService _dashBoardService;
        public DashBoardController(IDashBoardService dashBoardService)
        {
            _dashBoardService = dashBoardService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetTotalBookingRadialChartData()
        {
            return Json(await _dashBoardService.GetTotalBookingRadialChartData());
        }
        public async Task<IActionResult> GetRegisteredUsersRadialChartData()
        {
            return Json(await _dashBoardService.GetRegisteredUsersRadialChartData());
        }
        public async Task<IActionResult> GetRevenue()
        {
            return Json( await _dashBoardService.GetRevenue());
        }
        public async Task<IActionResult> GetBookingPieChartData()
        {
            return Json(await _dashBoardService.GetBookingPieChartData());
        }
       
    }
}
