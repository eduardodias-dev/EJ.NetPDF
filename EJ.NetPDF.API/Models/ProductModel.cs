namespace EJ.NetPDF.API.Models;

public class ProductModel
{
    public Guid? Id { get; private set; }
    public string? Name { get; private set; }
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public string? Cycle { get; private set; }

    public ProductModel(Guid? id, string name, string description, decimal price, string cycle) : this(name, description, price, cycle)
    {
        Id = id;
    }
    
    private ProductModel(string name, string description, decimal price, string cycle)
    {
        Name = name;
        Description = description;
        Price = price;
        Cycle = cycle?.ToUpper();
        
        if(string.IsNullOrWhiteSpace(Name)) throw new ArgumentNullException(nameof(cycle));
        if(string.IsNullOrWhiteSpace(Description)) throw new ArgumentNullException(nameof(cycle));
        if(price <= Decimal.Zero) throw new ArgumentOutOfRangeException(nameof(price), price, "Price must be greater than zero");
        if(string.IsNullOrWhiteSpace(cycle)) throw new ArgumentNullException(nameof(cycle));
        if(!_cycles.Contains(Cycle)) throw new ArgumentOutOfRangeException(nameof(cycle), cycle, "Invalid Cycle");

    }
    
    private readonly string[] _cycles = [ "MONTHLY", "YEARLY" ];
}