using BlazorApp.Application.Customers.Queries.GetCustomers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorApp.Client.Components.Pages.Customers;

public partial class CustomerList
{
    private List<CustomerDto>? customers;
    private List<CustomerDto>? filteredCustomers;
    private bool isLoading = true;
    private string? errorMessage;
    private string searchTerm = "";
    private CustomerDto? selectedCustomer;

    protected override async Task OnInitializedAsync() => await LoadCustomers();

    private async Task LoadCustomers()
    {
        isLoading = true;
        errorMessage = null;

        try
        {
            customers = await CustomerService.GetCustomersAsync();
            FilterCustomers();
        }
        catch (Exception ex)
        {
            errorMessage = $"Error loading customers: {ex.Message}";
            customers = new();
            filteredCustomers = new();
        }
        finally
        {
            isLoading = false;
        }
    }

    private void FilterCustomers()
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            filteredCustomers = customers;
            return;
        }

        var s = searchTerm.Trim().ToLower();
        filteredCustomers = customers?
            .Where(c =>
                c.FirstName.ToLower().Contains(s) ||
                c.LastName.ToLower().Contains(s) ||
                c.Email.ToLower().Contains(s) ||
                (c.PhoneNumber?.ToLower().Contains(s) ?? false) ||
                (c.Address?.ToLower().Contains(s) ?? false))
            .ToList();
    }

    private void ClearSearch()
    {
        searchTerm = string.Empty;
        FilterCustomers();
    }

    private void NavigateToCreate() => NavigationManager.NavigateTo("/customers/create");
    private void NavigateToEdit(Guid id) => NavigationManager.NavigateTo($"/customers/edit/{id}");

    private async Task ViewCustomerDetails(Guid id)
    {
        try
        {
            isLoading = true;
            selectedCustomer = await CustomerService.GetCustomerAsync(id);
        }
        catch (Exception ex)
        {
            errorMessage = $"Error loading customer details: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }

    private void CloseModal() => selectedCustomer = null;

    private async Task DeleteCustomer(Guid id)
    {
        var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this customer?");
        if (!confirmed) return;

        try
        {
            isLoading = true;
            await CustomerService.DeleteCustomerAsync(id);
            await LoadCustomers();
        }
        catch (Exception ex)
        {
            errorMessage = $"Error deleting customer: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }
}