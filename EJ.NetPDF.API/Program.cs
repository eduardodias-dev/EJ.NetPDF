using EJ.NetPDF.API.ApiRoutes;
using EJ.NetPDF.API.Extensions;
using Keycloak.AuthServices.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddKeycloakWebApi(builder.Configuration, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "preferred_username",
            ValidateAudience = false,
            ValidateIssuer = true
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("admin"));
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSerilog(cfg =>
    cfg.ReadFrom.Configuration(builder.Configuration));

builder.Services.AddInternalServices();
builder.Services.AddRefitConfiguration(builder.Configuration);
builder.Services.AddCors(o => o.AddDefaultPolicy(b =>
{
    var allowedOrigins = builder.Configuration
        .GetSection("Cors:AllowedOrigins")
        .Get<string[]>() ?? throw new InvalidOperationException("Invalid Cors Configuration.");

    var allowedMethods = builder.Configuration
        .GetSection("Cors:AllowedMethods")
        .Get<string[]>() ?? throw new InvalidOperationException("Invalid Cors Configuration.");

    b.WithOrigins(allowedOrigins);
    b.WithMethods(allowedMethods);
    b.AllowAnyHeader();
}));

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Starting web api...");

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "API Online");

app.MapGroup("api")
    .MapCustomerEndpoints();  

app.MapGroup("api")
    .MapProductEndpoints();

app.MapGroup("api")
    .MapOrderEndpoints()
    .RequireAuthorization();

app.UseExceptionHandler(b => 
    b.Run(async ctx => await Results.Problem().ExecuteAsync(ctx)
));

app.Run();