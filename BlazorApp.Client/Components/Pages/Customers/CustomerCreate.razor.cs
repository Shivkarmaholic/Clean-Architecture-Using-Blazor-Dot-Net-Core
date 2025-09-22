using BlazorApp.Application.Customers.Commands.CreateCustomer;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Client.Components.Pages.Customers;

public partial class CustomerCreate
{
    private CreateCustomerCommand command = new();
    private bool isSubmitting = false;
    private string? errorMessage;

    private async Task HandleValidSubmit()
    {
        if (isSubmitting) return;
        isSubmitting = true; errorMessage = null;

        try
        {
            await CustomerService.CreateCustomerAsync(command);
            NavigationManager.NavigateTo("/"); // changed from /customers
        }
        catch (Exception ex)
        {
            errorMessage = $"Error creating customer: {ex.Message}";
        }
        finally
        {
            isSubmitting = false;
        }
    }

    private void NavigateBack() => NavigationManager.NavigateTo("/"); // changed from /customers
}