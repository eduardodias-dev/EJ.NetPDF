using EJ.NetPDF.API.Models;
using Refit;

namespace EJ.NetPDF.API.Services;

public class AsaasPaymentService : IPaymentService
{
    private readonly ICustomerExternalRepository _customersRepository;
    private readonly IPaymentExternalRepository _paymentRepository;
    public AsaasPaymentService(ICustomerExternalRepository customersRepository, 
        IPaymentExternalRepository paymentRepository)
    {
        _customersRepository = customersRepository;
        _paymentRepository = paymentRepository;
    }
    public async Task<IEnumerable<Customer>> GetCustomers()
    {
        var response = await _customersRepository.GetCustomers();
        
        return response.Data;
    }

    public async Task<Customer> GetCustomer(string customerId)
    {
        try
        {
            var result = await _customersRepository.GetCustomer(customerId);
            
            return result;
        }
        catch (ApiException e)
        {
            //Log
            throw;
        }
    }

    public async Task<Customer> CreateCustomer(AddCustomerModel addCustomer)
    {
        var existingCustomers = await _customersRepository.GetCustomers(addCustomer.CpfCnpj);
        if (existingCustomers?.Data?.Any() ?? false)
        {
            throw new InvalidOperationException("Customer with same cpf cnpj already exists");
        }

        try
        {
            var result = await _customersRepository.CreateCustomer(addCustomer);
            
            return result;
        }
        catch (ApiException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<Customer> UpdateCustomer(UpdateCustomerModel updateCustomer)
    {
        var existingCustomer = _customersRepository.GetCustomer(updateCustomer.Id!);
        if (existingCustomer == null)
        {
            throw new InvalidOperationException("Customer was not found.");
        }

        try
        {
            var result = await _customersRepository.UpdateCustomer(updateCustomer.Id!, updateCustomer);
            
            return result;
        }
        catch (ApiException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> DeleteCustomer(string id)
    {
        var result = await _customersRepository.DeleteCustomer(id);
        
        return result is { IsSuccessful: true, IsSuccessStatusCode: true };
    }

    public async Task<Payment> CreatePayment(AddPaymentModel addPayment)
    {
        try
        {
            var result = await _paymentRepository.CreatePayment(addPayment);

            return result;
        }
        catch (ApiException e)
        {
            throw;
        }
    }
}