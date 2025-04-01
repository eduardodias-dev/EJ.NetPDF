namespace EJ.NetPDF.API.Models;

public class Subscription
{
    public string? Id { get; set; }
    public string? DateCreated { get; set; }
    public string? Customer  { get; set; }
    public string? PaymentLink { get; set; }
    public string? BillingType { get; set; }
    public string? Cycle { get; set; }
    public decimal? Value { get; set; }
    public DateTime? NextDueDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public bool Deleted { get; set; }
    public Payment[]? Payments { get; set; }
}