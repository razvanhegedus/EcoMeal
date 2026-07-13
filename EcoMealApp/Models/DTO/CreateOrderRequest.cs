namespace EcoMealApp.Models.DTO;

public class CreateOrderRequest
{
    public Guid BusinessId { get; set; }
    public List<OrderPackageDto> Packages { get; set; } = new();
    public Guid UserId { get; set; }
}