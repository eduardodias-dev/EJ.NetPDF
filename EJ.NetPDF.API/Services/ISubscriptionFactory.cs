using EJ.NetPDF.API.Models;

namespace EJ.NetPDF.API.Services;

public interface ISubscriptionFactory
{
    AddSubscriptionModel Create(Order createOrderModel);
}