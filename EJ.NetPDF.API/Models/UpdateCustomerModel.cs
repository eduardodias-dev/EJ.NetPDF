namespace EJ.NetPDF.API.Models;

public class UpdateCustomerModel
{
    public string? Id { get; }
    public string? CpfCnpj { get; }
    public string? Email { get; }
    public string? Name { get; }

    public UpdateCustomerModel(string? id, string? cpfCnpj, string? email, string? name)
    {
        Id = id;
        CpfCnpj = cpfCnpj;
        Email = email;
        Name = name;
        
        if(string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));
    }
}