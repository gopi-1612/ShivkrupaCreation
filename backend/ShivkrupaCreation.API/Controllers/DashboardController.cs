using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShivkrupaCreation.API.DTOs.Common;
using ShivkrupaCreation.API.DTOs.Dashboard;
using ShivkrupaCreation.API.Models;
using ShivkrupaCreation.API.Services.Interfaces;

namespace ShivkrupaCreation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = Roles.Admin)]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _dashboardService.GetStatsAsync();
            return Ok(ApiResponse<DashboardStatsDto>.SuccessResponse(stats));
        }
    }
}
