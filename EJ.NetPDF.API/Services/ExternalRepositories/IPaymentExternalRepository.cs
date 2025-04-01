using EJ.NetPDF.API.Models;
using Refit;

namespace EJ.NetPDF.API.Services.ExternalRepositories;

public interface IPaymentExternalRepository
{
    [Post("/payments")]
    Task<Payment> CreatePayment([Body] AddPaymentModel payment);
    
    [Get("/payments/{id}")]
    Task<Payment> GetPayment(string id);

    [Post("/subscriptions")]
    Task<Subscription> CreateSubscription([Body] AddSubscriptionModel subscription);
    
    [Get("/subscriptions/{id}")]
    Task<Subscription> GetSubscription(string id);
    
    [Get("/subscriptions/{id}/payments")]
    Task<AsaasResponseDTO<Payment[]>> GetSubscriptionPayments(string id);
}