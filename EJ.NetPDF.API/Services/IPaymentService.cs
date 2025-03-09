using EJ.NetPDF.API.Models;
using Refit;

namespace EJ.NetPDF.API.Services;

public interface IPaymentService
{
    Task<IEnumerable<Customer>> GetCustomers();
    Task<Customer> GetCustomer(string id);
    Task<Customer> CreateCustomer(AddCustomerModel addCustomer);
    Task<Customer> UpdateCustomer(UpdateCustomerModel updateCustomer);
    Task<bool> DeleteCustomer(string id);
    Task<Payment> CreatePayment(AddPaymentModel addPayment);
}