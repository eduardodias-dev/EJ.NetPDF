namespace EJ.NetPDF.API.Models;

public class Customer
{
    public string? Id { get; set; }
    public string? CpfCnpj { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public bool Deleted { get; set; }
}