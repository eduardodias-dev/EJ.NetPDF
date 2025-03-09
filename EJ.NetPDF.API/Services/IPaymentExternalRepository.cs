using EJ.NetPDF.API.Models;
using Refit;

namespace EJ.NetPDF.API.Services;

public interface IPaymentExternalRepository
{
    [Post("/payments")]
    Task<Payment> CreatePayment([Body] AddPaymentModel payment);
}