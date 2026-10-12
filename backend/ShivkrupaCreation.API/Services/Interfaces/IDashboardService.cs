using ShivkrupaCreation.API.DTOs.Dashboard;

namespace ShivkrupaCreation.API.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetStatsAsync();
    }
}
