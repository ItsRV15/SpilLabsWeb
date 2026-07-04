namespace Domain.Entities;

public class Client
{
    public int ClientId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public string? Address1 { get; set; }

    public string? Address2 { get; set; }

    public string? Address3 { get; set; }

    public string? Suburb { get; set; }

    public string? State { get; set; }

    public string? PostCode { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    // Navigation Property
    public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
}