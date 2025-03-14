using EJ.NetPDF.API.ApiRoutes;
using EJ.NetPDF.API.Data;
using EJ.NetPDF.API.Data.Interfaces;
using EJ.NetPDF.API.Models;
using EJ.NetPDF.API.Services;
using EJ.NetPDF.API.Services.ExternalRepositories;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPaymentService, AsaasPaymentService>();
builder.Services.AddScoped<IRepository<Product>, MongoRepository<Product>>();

var httpClientConfigAction = (HttpClient client) =>
{
    client.BaseAddress = new Uri(builder.Configuration["Asaas:BaseAddress"]);
    client.DefaultRequestHeaders.Add("accept", "application/json");
    client.DefaultRequestHeaders.Add("access_token", builder.Configuration["Asaas:ApiKey"]);
};

builder.Services.AddRefitClient<ICustomerExternalRepository>()
    .ConfigureHttpClient(httpClientConfigAction);

builder.Services.AddRefitClient<IPaymentExternalRepository>()
    .ConfigureHttpClient(httpClientConfigAction);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGroup("api")
    .MapCustomerEndpoints();

app.MapGroup("api")
    .MapProductEndpoints();

app.MapGroup("api")
    .MapPost("/payments", async (IPaymentService paymentService, AddPaymentModel paymentData) =>
    {
        var result = await paymentService.CreatePayment(paymentData);

        return Results.Ok(result);
    }).WithTags("Payments");


app.UseExceptionHandler(app => 
    app.Run(async ctx => await Results.Problem().ExecuteAsync(ctx)
));

app.Run();