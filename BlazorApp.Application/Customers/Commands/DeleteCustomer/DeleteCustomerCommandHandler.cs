using System;
using System.Threading;
using System.Threading.Tasks;
using BlazorApp.Application.Common.Interfaces;
using MediatR;

namespace BlazorApp.Application.Customers.Commands.DeleteCustomer
{
    public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand>
    {
        private readonly IApplicationDbContext _context;

        public DeleteCustomerCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Customers.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity == null)
            {
                throw new Exception($"Customer with ID {request.Id} not found.");
            }

            _context.Customers.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}