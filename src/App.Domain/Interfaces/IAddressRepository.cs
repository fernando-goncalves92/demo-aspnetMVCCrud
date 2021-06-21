using System;
using System.Threading.Tasks;
using App.Domain.Entities;

namespace App.Domain.Interfaces
{
    public interface IAddressRepository : IRepository<Address>
    {
        Task<Address> GetAddressBySupplier(Guid supplierId);        
    }
}
