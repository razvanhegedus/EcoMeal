namespace EcoMealApp.Models.DTO.BusinessManager;

public class BusinessStatsDto
{
    public int PendingOrdersCount { get; set; }
    public int ActivePackagesCount { get; set; }
    public double TodaysRevenue { get; set; }
}