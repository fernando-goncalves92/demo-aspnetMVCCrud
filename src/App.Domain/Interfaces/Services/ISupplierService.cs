using App.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace App.Domain.Interfaces.Services
{
    public interface ISupplierService : IDisposable
    {
        Task Add(Supplier supplier);
        Task Update(Supplier supplier);
        Task Delete(Guid id);
        Task UpdateAddress(Address address);
    }
}
