using EJ.NetPDF.API.Models;
using EJ.NetPDF.API.Services.ExternalRepositories;
using Refit;

namespace EJ.NetPDF.API.Services;

public class AsaasPaymentService : IPaymentService
{
    private readonly ICustomerExternalRepository _customersRepository;
    private readonly IPaymentExternalRepository _paymentRepository;
    private readonly ILogger<AsaasPaymentService> _logger;
    
    public AsaasPaymentService(ICustomerExternalRepository customersRepository, 
        IPaymentExternalRepository paymentRepository, ILogger<AsaasPaymentService> logger)
    {
        _customersRepository = customersRepository;
        _paymentRepository = paymentRepository;
        _logger = logger;
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
            _logger.LogError(e, "An error occured while getting the customer.");
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
            _logger.LogError(e, "An error occured while creating the customer.");
            throw;
        }
    }

    public async Task<Customer> UpdateCustomer(UpdateCustomerModel updateCustomer)
    {
        var existingCustomer = await _customersRepository.GetCustomer(updateCustomer.Id!);
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
            _logger.LogError(e, "An error occured while updating the customer.");
            throw;
        }
    }

    public async Task<bool> DeleteCustomer(string id)
    {
        var result = await _customersRepository.DeleteCustomer(id);
        
        return result is { IsSuccessful: true, IsSuccessStatusCode: true };
    }

    public async Task<Subscription> CreateSubscription(AddSubscriptionModel subscriptionData)
    {
        try
        {
            var subscription = await _paymentRepository.CreateSubscription(subscriptionData);
            
            return subscription;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Error while adding subscription.");
            throw;
        }
    }

    public async Task<Subscription> GetSubscriptionById(string id)
    {
        try
        {
            return await _paymentRepository.GetSubscription(id);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "An error occured while getting the subscription.");
            throw;
        }
    }

    public async Task<Payment[]> GetSubscriptionPayments(string subscriptionId)
    {
        try
        {
            var response = await _paymentRepository.GetSubscriptionPayments(subscriptionId);

            return response.Data;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "An error occured while getting the subscription.");
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured while getting the subscription.");
            throw;
        }
    }
}