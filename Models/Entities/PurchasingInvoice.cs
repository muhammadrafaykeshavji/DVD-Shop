using System.ComponentModel.DataAnnotations;

namespace E_project_DVD_Shop.Models.Entities;

public class PurchasingInvoice
{
    public int PurchasingInvoiceId { get; set; }

    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}
