using System;
using MediatR;
using BlazorApp.Application.Customers.Queries.GetCustomers;

namespace BlazorApp.Application.Customers.Queries.GetCustomer
{
    public class GetCustomerQuery : IRequest<CustomerDto>
    {
        public Guid Id { get; set; }
    }
}