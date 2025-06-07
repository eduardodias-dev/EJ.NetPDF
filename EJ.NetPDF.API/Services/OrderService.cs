using EJ.NetPDF.API.Data.Interfaces;
using EJ.NetPDF.API.Models;
using EJ.NetPDF.API.Services.ExternalRepositories;
using Refit;

namespace EJ.NetPDF.API.Services;

public class OrderService : IOrderService
{
    private readonly ICustomerExternalRepository _customerExternalRepository;
    private readonly IOrderRepository _repo;
    private readonly ILogger<OrderService> _logger;
    private readonly IPaymentService _paymentService;
    private readonly ISubscriptionFactory _subscriptionFactory;
    private readonly IOrderFactory _orderFactory;

    public OrderService(IOrderRepository repo, 
        ICustomerExternalRepository customerExternalRepository, 
        ILogger<OrderService> logger,
        IPaymentService paymentService, 
        ISubscriptionFactory subscriptionFactory, 
        IOrderFactory orderFactory)
    {
        _repo = repo;
        _customerExternalRepository = customerExternalRepository;
        _logger = logger;
        _paymentService = paymentService;
        _subscriptionFactory = subscriptionFactory;
        _orderFactory = orderFactory;
    }

    public async Task<Order> CreateOrder(CreateOrderModel model)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(model);
            
            var order = await _orderFactory.CreateFromModelAsync(model);
            var addSubscriptionModel = _subscriptionFactory.Create(order);

            var addOrderTask = _repo.AddAsync(order);
            var createPaymentTask = _paymentService.CreateSubscription(addSubscriptionModel);

            order.SetPaymentId((await createPaymentTask).Id!);
            order.Process();

            await addOrderTask; //ensure insertion before updating it
            await _repo.UpdateAsync(order);

            return order;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<OrderModel> GetOrderById(Guid id)
    {
        var order = await _repo.GetByIdAsync(id);
        var subscriptionTask = _paymentService.GetSubscriptionById(order.PaymentId!);
        var paymentsTask = _paymentService.GetSubscriptionPayments(order.PaymentId!);
        var customerTask = _customerExternalRepository.GetCustomer(order.CustomerId!);
        
        var subscriptionData = await subscriptionTask;
        subscriptionData.Payments = await paymentsTask;
        
        return new OrderModel
        {
            Id = order.Id,
            SubscriptionData = subscriptionData,
            CustomerData = await customerTask,
            Product = new ProductModel(order.Product.Id, order.Product.Name!, order.Product.Description!, 
                order.Product.Price!, order.Product.Cycle!),
            Status = order.Status.ToString(),
        };
    }

    public async Task<OrderModel[]> GetOrdersByCustomerId(string customerId)
    {
        var data = await _repo.GetOrdersByCustomerId(customerId);
        
        var subscriptionTasks = data.Select(x => _paymentService.GetSubscriptionById(x.PaymentId!)).ToArray();
        var customerTasks = data.Select(x => _customerExternalRepository.GetCustomer(x.CustomerId!));
        
        var subscriptions = await Task.WhenAll(subscriptionTasks);
        var customers = await Task.WhenAll(customerTasks);
        
        return data.Select(o => new OrderModel
        {
            Id = o.Id,
            SubscriptionData = subscriptions.FirstOrDefault(s => s.Id == o.PaymentId),
            CustomerData = customers.FirstOrDefault(c => c.Id == o.CustomerId),
            Product = new ProductModel(o.Product.Id, o.Product.Name!, o.Product.Description!, 
                o.Product.Price!, o.Product.Cycle!),
            Status = o.Status.ToString(),
        }).ToArray();
    }
}