using System.Net.Http.Json;
using BlazorApp.Application.Customers.Commands.CreateCustomer;
using BlazorApp.Application.Customers.Commands.UpdateCustomer;
using BlazorApp.Application.Customers.Queries.GetCustomers;

namespace BlazorApp.Client.Services;

public class CustomerService
{
    private readonly HttpClient _httpClient;
    private const string ApiEndpoint = "api/customers";

    public CustomerService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<List<CustomerDto>> GetCustomersAsync()
        => await _httpClient.GetFromJsonAsync<List<CustomerDto>>(ApiEndpoint) ?? new();

    public async Task<CustomerDto> GetCustomerAsync(Guid id)
        => await _httpClient.GetFromJsonAsync<CustomerDto>($"{ApiEndpoint}/{id}")
           ?? throw new Exception($"Customer with ID {id} not found.");

    public async Task<Guid> CreateCustomerAsync(CreateCustomerCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync(ApiEndpoint, command);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<Guid>())!;
    }

    public async Task UpdateCustomerAsync(UpdateCustomerCommand command)
    {
        var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoint}/{command.Id}", command);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteCustomerAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"{ApiEndpoint}/{id}");
        response.EnsureSuccessStatusCode();
    }
}