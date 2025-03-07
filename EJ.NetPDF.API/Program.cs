using EJ.NetPDF.API.Models;
using EJ.NetPDF.API.Services;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPaymentService, AsaasPaymentService>();

builder.Services.AddRefitClient<ICustomerExternalRepository>()
    .ConfigureHttpClient(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["Asaas:BaseAddress"]);
        client.DefaultRequestHeaders.Add("accept", "application/json");
        client.DefaultRequestHeaders.Add("access_token", builder.Configuration["Asaas:ApiKey"]);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var routeBuilder = app.MapGroup("api");

routeBuilder.MapGet("/customers", async (IPaymentService paymentService) => {
    var result = await paymentService.GetCustomers();
    
    return Results.Ok(result);
});

routeBuilder.MapGet("/customers/{id}", async (IPaymentService paymentService, string id) =>
{
    var result = await paymentService.GetCustomer(id);
    
    return Results.Ok(result);
});

routeBuilder.MapPost("/customers", async (IPaymentService paymentService, AddCustomerModel customer) =>
{
    var result = await paymentService.CreateCustomer(customer);
    
    return Results.Created($"/customers/{result.Id}", result);
});

routeBuilder.MapPut("/customers/{id}", async (IPaymentService paymentService, string id, UpdateCustomerModel customer) =>
{
        _ = await paymentService.UpdateCustomer(customer);
        
        return Results.NoContent(); 
});

routeBuilder.MapDelete("/customer/{id}", async (IPaymentService paymentService, string id) =>
{
    bool result = await paymentService.DeleteCustomer(id);
    
    return result ? Results.NoContent() : Results.NotFound();
});


app.UseExceptionHandler(app => 
    app.Run(async ctx => await Results.Problem().ExecuteAsync(ctx)
));

app.Run();