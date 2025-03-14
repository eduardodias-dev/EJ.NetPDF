using EJ.NetPDF.API.Models;
using Refit;

namespace EJ.NetPDF.API.Services.ExternalRepositories;

public interface ICustomerExternalRepository
{
    [Get("/customers")]
    Task<AsaasResponseDTO<Customer[]>> GetCustomers([Query] string cpfCnpj = null);

    [Get("/customers/{id}")]
    Task<Customer> GetCustomer(string id);
    
    [Post("/customers")]
    Task<Customer> CreateCustomer([Body] AddCustomerModel addCustomer);
    
    [Put("/customers/{id}")]
    Task<Customer> UpdateCustomer(string id, [Body] UpdateCustomerModel updateCustomer);
    
    [Delete("/customers/{id}")]
    Task<ApiResponse<dynamic>> DeleteCustomer(string id);
}