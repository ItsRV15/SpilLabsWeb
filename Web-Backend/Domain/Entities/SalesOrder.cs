namespace Domain.Entities;

public class SalesOrder
{
    public int OrderId { get; set; }

    public int ClientId { get; set; }

    public string InvoiceNo { get; set; } = string.Empty;

    public DateTime InvoiceDate { get; set; }

    public string? ReferenceNo { get; set; }

    public string? Note { get; set; }

    public decimal TotalExcl { get; set; }

    public decimal TotalTax { get; set; }

    public decimal TotalIncl { get; set; }

    // Navigation Properties- This lets Entity Framework automatically load the related client.Instead of manually joining tables
    public Client? Client { get; set; }

    public ICollection<SalesOrderDetail> SalesOrderDetails { get; set; } = new List<SalesOrderDetail>();
}