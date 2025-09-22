using BlazorApp.Application.Customers.Commands.UpdateCustomer;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Client.Components.Pages.Customers;

public partial class CustomerEdit
{
    [Parameter] public Guid Id { get; set; }

    private UpdateCustomerCommand command = new();
    private bool isLoading = true;
    private bool isSubmitting = false;
    private string? errorMessage;

    protected override async Task OnInitializedAsync() => await LoadCustomerData();

    private async Task LoadCustomerData()
    {
        isLoading = true; errorMessage = null;

        try
        {
            var customer = await CustomerService.GetCustomerAsync(Id);
            command = new UpdateCustomerCommand
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber ?? string.Empty,
                Address = customer.Address ?? string.Empty
            };
        }
        catch (Exception ex)
        {
            errorMessage = $"Error loading customer: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task HandleValidSubmit()
    {
        if (isSubmitting) return;
        isSubmitting = true; errorMessage = null;

        try
        {
            await CustomerService.UpdateCustomerAsync(command);
            NavigationManager.NavigateTo("/"); // changed from /customers
        }
        catch (Exception ex)
        {
            errorMessage = $"Error updating customer: {ex.Message}";
        }
        finally
        {
            isSubmitting = false;
        }
    }

    private void NavigateBack() => NavigationManager.NavigateTo("/");
}