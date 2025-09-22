using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<BlazorApp.Domain.Entities.Customer> Customers { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}