using EJ.NetPDF.API.Data.Interfaces;
using EJ.NetPDF.API.Models;
using EJ.NetPDF.API.Services;
using EJ.NetPDF.API.Services.ExternalRepositories;
using EJ.NetPDF.API.Tests.Fakes;
using EJ.NetPDF.API.Tests.Util;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace EJ.NetPDF.API.Tests.Services;

[TestFixture]
public class OrderServiceTests
{
    private OrderService _sut;
    private ICustomerExternalRepository _customerExternalRepository;
    private IRepository<Order> _orderRepository;
    private ILogger<OrderService> _logger;
    private IPaymentService _paymentService; 
    private ISubscriptionFactory _subscriptionFactory;
    private IOrderFactory _orderFactory;
    
    [SetUp]
    public void Setup()
    {
        _customerExternalRepository = A.Fake<ICustomerExternalRepository>(x => x.Strict());
        _orderRepository = A.Fake<IRepository<Order>>(x => x.Strict());
        _subscriptionFactory = A.Fake<ISubscriptionFactory>(x => x.Strict());
        _orderFactory = A.Fake<IOrderFactory>(x => x.Strict());
        _logger = new FakeLogger<OrderService>();
        _paymentService = A.Fake<IPaymentService>(x => x.Strict());
        
        _sut = new OrderService(_orderRepository, _customerExternalRepository, _logger, _paymentService, 
            _subscriptionFactory, _orderFactory);
    }

    [Test]
    public void CreateOrder_ShouldThrowExceptionWhen_ModelIsNull()
    {
        //Arrange
        CreateOrderModel model = null;
        
        //Act/Assert
        Assert.That(async () => await _sut.CreateOrder(model), Throws.ArgumentNullException);
    }
    
    [Test]
    public async Task CreateOrder_ShouldCreateInPaymentServiceAndDatabase_WhenDataIsValid()
    {
        //Arrange
        var productId = Guid.NewGuid();
        var customerId = GenericStringGenerator.Generate(10);
        var model = new CreateOrderModel(customerId, "PIX", productId);
        var subscriptionId = GenericStringGenerator.Generate(10);
        
        A.CallTo(() => _orderFactory.CreateFromModelAsync(model))
            .Returns(Task.FromResult(new Order(model.CustomerId, model.PaymentType, new Product()
            {
                Id = productId,
                Description = "Generic Product Description",
                Name = "Generic Product",
                Cycle = "MONTHLY",
                Price = 9.9M
            })));
        
        A.CallTo(() => _subscriptionFactory.Create(A<Order>.Ignored))
            .Returns(new AddSubscriptionModel(customerId, "PIX", 9.9M,
                DateTime.Today.AddDays(3),
                "MONTHLY", default(string)));
        
        A.CallTo(() => _orderRepository.AddAsync(A<Order>.Ignored))
            .Returns(Task.CompletedTask);
        
        A.CallTo(() => _orderRepository.UpdateAsync(A<Order>.Ignored))
            .Returns(Task.CompletedTask);

        A.CallTo(() => _paymentService.CreateSubscription(A<AddSubscriptionModel>.That.Matches(
            m => m.Cycle == "MONTHLY"
                 && m.BillingType == "PIX"
                 && m.Customer == customerId
                 && m.Value == 9.9M)))
            .Returns(Task.FromResult<Subscription>(new Subscription()
            {
                Id = subscriptionId,
                Value = 9.9m
            }));
        
        //Act
        var result = await _sut.CreateOrder(model);
        
        //Assert
        A.CallTo(() => 
                _orderRepository.AddAsync(A<Order>.That.Matches(o => o.CustomerId == customerId 
                                                                     && o.Product.Id == productId
                                                                     && o.PaymentId == subscriptionId)))
            .MustHaveHappenedOnceExactly();
        
        A.CallTo(() => _orderRepository.UpdateAsync(A<Order>.That.Matches(o => o.CustomerId == customerId 
                                                                               && o.Product.Id == productId
                                                                               && o.Status == OrderStatus.Processing
                                                                               && o.PaymentId == subscriptionId)))
            .MustHaveHappenedOnceExactly();
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result.PaymentId, Is.EqualTo(subscriptionId));
        Assert.That(result.CustomerId, Is.EqualTo(customerId));
        Assert.That(result.Status, Is.EqualTo(OrderStatus.Processing));
    }
}