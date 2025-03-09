namespace EJ.NetPDF.API.Models;

public class Payment
{
    public string? Id { get; set; }
    public string? DateCreated { get; set; }
    public string? Customer  { get; set; }
    public string? Subscription { get; set; }
    public string? Installment { get; set; }
    public string? PaymentLink { get; set; }
    public decimal? Value { get; set; }
    public decimal? NetValue { get; set; }
    public decimal? OriginalValue { get; set; }
    public decimal? InterestValue { get; set; }
    public string? Description { get; set; }
    public string? BillingType { get; set; }
    public string? PixTransaction { get; set; }
    public string? PixQrCodeId { get; set; }
    public string? Status { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? OriginalDueDate { get; set; }
    public string? InvoiceUrl { get; set; }
    public string? InvoiceNumber { get; set; }
}