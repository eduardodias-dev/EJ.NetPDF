namespace EJ.NetPDF.API.Models;

public class AddSubscriptionModel
{
    public string? Customer { get; set; }
    public string? BillingType { get; set; }
    public decimal Value { get; set; }
    public DateTime? NextDueDate { get; set; }
    public string? Cycle { get; set; }
    public string? Description { get; set; }

    public AddSubscriptionModel(string customer, string billingType, decimal value, DateTime? nextDueDate, string cycle, string? description = null)
    {
        Customer = customer;
        BillingType = billingType;
        Value = value;
        NextDueDate = nextDueDate;
        Description = description;
        Cycle = cycle;
        
        if(string.IsNullOrWhiteSpace(Customer)) throw new ArgumentNullException(nameof(customer));
        
        if(string.IsNullOrWhiteSpace(BillingType)) throw new ArgumentNullException(nameof(billingType));
        if(!_billingTypes.Contains(BillingType)) throw new ArgumentOutOfRangeException(nameof(billingType), billingType, "Invalid Billing Type");
        
        if(string.IsNullOrWhiteSpace(Cycle)) throw new ArgumentNullException(nameof(cycle));
        if(!_cycles.Contains(Cycle)) throw new ArgumentOutOfRangeException(nameof(cycle), cycle, "Invalid Cycle");
        
        if(Value <= decimal.Zero) throw new ArgumentOutOfRangeException(nameof(value), value, "Value must be greater than zero");
        if(NextDueDate < DateTime.Today) throw new ArgumentOutOfRangeException(nameof(nextDueDate), nextDueDate, "Due date must be greater than today");
    }
    
    private readonly string[] _billingTypes = [ "PIX", "CREDIT_CARD", "BOLETO" ];
    private readonly string[] _cycles = [ "MONTHLY", "YEARLY" ];
}