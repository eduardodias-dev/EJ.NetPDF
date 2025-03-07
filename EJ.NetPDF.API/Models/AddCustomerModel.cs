namespace EJ.NetPDF.API.Models;

public class AddCustomerModel
{
    public string? CpfCnpj { get; }
    public string? Email { get; }
    public string? Name { get; }

    public AddCustomerModel(string? cpfCnpj, string? email, string? name)
    {
        CpfCnpj = cpfCnpj;
        Email = email;
        Name = name;
        
        if(string.IsNullOrWhiteSpace(cpfCnpj)) throw new ArgumentNullException(nameof(cpfCnpj));
        if(string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));
        if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
    }
}