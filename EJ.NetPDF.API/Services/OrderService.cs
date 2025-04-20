using EJ.NetPDF.API.Data.Interfaces;
using EJ.NetPDF.API.Models;
using EJ.NetPDF.API.Services.ExternalRepositories;
using Refit;

namespace EJ.NetPDF.API.Services;

public class OrderService : IOrderService
{
    private readonly ICustomerExternalRepository _customerExternalRepository;
    private readonly IRepository<Order> _repo;
    private readonly IRepository<Product> _productRepo;
    private readonly ILogger<OrderService> _logger;
    private readonly IPaymentService _paymentService;

    public OrderService(IRepository<Order> repo, 
        IRepository<Product> productRepo, 
        ICustomerExternalRepository customerExternalRepository, 
        ILogger<OrderService> logger,
        IPaymentService paymentService)
    {
        _repo = repo;
        _productRepo = productRepo;
        _customerExternalRepository = customerExternalRepository;
        _logger = logger;
        _paymentService = paymentService;
    }

    public async Task<Order> CreateOrder(CreateOrderModel model)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(model);
            var dueDate = GetDueDate();
            var product = await _productRepo.GetByIdAsync(model.ProductId.GetValueOrDefault());

            if (product == null)
                throw new InvalidOperationException($"Product {model.ProductId} not found.");

            var order = new Order(model.CustomerId!, product);
            var addSubscriptionModel =
                new AddSubscriptionModel(model.CustomerId!, model.PaymentType!, order.Total,
                    dueDate, product.Cycle!, order.Description);

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
            SubscriptionData = subscriptionData,
            CustomerData = await customerTask,
            Product = new ProductModel(order.Product.Id, order.Product.Name!, order.Product.Description!, order.Product.Price!, order.Product.Cycle!),
            Status = order.Status.ToString(),
        };
    }

    private static DateTime GetDueDate()
    {
        return DateTime.Today.AddDays(5);
    }
}