namespace E_project_DVD_Shop.Models.Entities;

public class Supplier
{
    public int SupplierId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Contact { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<PurchasingInvoice> PurchasingInvoices { get; set; } = new List<PurchasingInvoice>();
}
