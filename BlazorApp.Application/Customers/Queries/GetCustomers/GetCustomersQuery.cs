using System;
using System.Collections.Generic;
using MediatR;

namespace BlazorApp.Application.Customers.Queries.GetCustomers
{
    public class GetCustomersQuery : IRequest<List<CustomerDto>>
    {
    }
}