using EJ.NetPDF.API.Services.ExternalRepositories;
using Refit;

namespace EJ.NetPDF.API.Extensions;

public static class RefitConfigExtension
{
    public static void AddRefitConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var httpClientConfigAction = (HttpClient client) =>
        {
            client.BaseAddress = new Uri(configuration["Asaas:BaseAddress"]!);
            client.DefaultRequestHeaders.Add("accept", "application/json");
            client.DefaultRequestHeaders.Add("access_token", configuration["Asaas:ApiKey"]);
        };

        services.AddRefitClient<ICustomerExternalRepository>()
            .ConfigureHttpClient(httpClientConfigAction);
        
        services.AddRefitClient<IPaymentExternalRepository>()
            .ConfigureHttpClient(httpClientConfigAction);
    }
}