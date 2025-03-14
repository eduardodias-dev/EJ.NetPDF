using EJ.NetPDF.API.Models;

namespace EJ.NetPDF.API.Tests.Models;

[TestFixture]
public class AddPaymentModelTests
{
    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void Ctor_ThrowsArgumentNullException_WhenCustomerIdIsNullOrWhitespace(string? customerId)
    {
        //Arrange/Act
        var action = () => new AddPaymentModel(customerId!, string.Empty,
            decimal.Zero, default, string.Empty);
        
        //Assert
        Assert.That(action, Throws.ArgumentNullException);
    }
    
    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void Ctor_ThrowsArgumentNullException_WhenBillingTypeIsNullOrWhitespace(string? billingType)
    {
        //Arrange/Act
        var action = () => new AddPaymentModel("Test", billingType!,
            decimal.Zero, default, string.Empty);
        
        //Assert
        Assert.That(action, Throws.ArgumentNullException);
    }

    [Test]
    [TestCase("OTHER")]
    [TestCase("BIT")]
    [TestCase("CASH")]
    public void Ctor_ThrowsArgumentException_WhenBillingType(string? billingType)
    {
        //Arrange / Act
        var result = () => new AddPaymentModel("test", billingType!,
            decimal.Zero, default, string.Empty);
        
        //Assert
        Assert.That(result, Throws.TypeOf<ArgumentOutOfRangeException>(), "Invalid Billing Type");
    }

    [Test]
    [TestCase(0)]
    [TestCase(-10.9)]
    [TestCase(-1.2)]
    public void Ctor_ThrowsArgumentException_WhenValueIsLessThanOrEqualToZero(decimal value)
    {
        //Arrange / Act
        var result = () => new AddPaymentModel("test", "PIX",
            value, default, string.Empty);
        
        //Assert
        Assert.That(result, Throws.TypeOf<ArgumentOutOfRangeException>(), "Value must be greater than zero");
    }

    [Test]
    public void Ctor_ThrowsArgumentException_WhenDueDateIsOlderThanToday()
    {
        //Arrange
        var dueDate = DateTime.Now.AddDays(-1);
        
        //Act
        var result = () => new AddPaymentModel("test", "PIX",
            1, dueDate, string.Empty);
        
        //Assert
        Assert.That(result, Throws.TypeOf<ArgumentOutOfRangeException>(), "Due date must be greater than today");
    }

    [Test]
    public void Ctor_ShouldSetPropertiesCorrectly_WhenValidDataIsPassed()
    {
        //Arrange
        var customerId = Guid.NewGuid().ToString();
        var billingType = "PIX";
        var dueDate = DateTime.Today.AddDays(1);
        var value = 99.99M;
        var description = "this is a test";
        
        //Act
        var result = new AddPaymentModel(customerId, billingType, value, dueDate, description);
        
        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Customer, Is.EqualTo(customerId));
        Assert.That(result.BillingType, Is.EqualTo(billingType));
        Assert.That(result.Value, Is.EqualTo(value));
        Assert.That(result.Description, Is.EqualTo(description));
    }
}