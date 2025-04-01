using BlackLagoon.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackLagoon.Application.Services.Interface
{
    public interface IDashBoardService
    {
        Task<RadialBarChartDto> GetTotalBookingRadialChartData();
        Task<RadialBarChartDto> GetRegisteredUsersRadialChartData();
        Task<RadialBarChartDto> GetRevenue();
        Task<PieChartDto> GetBookingPieChartData();
        


    }
}
