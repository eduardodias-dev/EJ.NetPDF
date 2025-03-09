namespace EJ.NetPDF.API.Models;

public class AddPaymentModel
{
    public string Customer { get; }
    public string BillingType { get; }
    public decimal Value { get; }
    public DateTime DueDate { get; }
    public string? Description { get; }

    public AddPaymentModel(string customer, string billingType, decimal value, DateTime dueDate, string? description = null)
    {
        Customer = customer;
        BillingType = billingType;
        Value = value;
        DueDate = dueDate;
        Description = description;
        
        if(string.IsNullOrWhiteSpace(Customer)) throw new ArgumentNullException(nameof(customer));
        if(string.IsNullOrWhiteSpace(BillingType)) throw new ArgumentNullException(nameof(billingType));
        if(!_billingTypes.Contains(BillingType)) throw new ArgumentOutOfRangeException(nameof(billingType), billingType, "Invalid Billing Type");
        if(Value <= decimal.Zero) throw new ArgumentOutOfRangeException(nameof(value), value, "Value must be greater than zero");
        if(DueDate < DateTime.Today) throw new ArgumentOutOfRangeException(nameof(dueDate), dueDate, "Due date must be greater than today");
    }

    private readonly string[] _billingTypes = [ "PIX", "CREDIT_CARD", "BOLETO" ];
}