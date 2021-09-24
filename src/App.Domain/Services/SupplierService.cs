using App.Domain.Entities;
using App.Domain.Entities.Validators;
using App.Domain.Interfaces.Repositories;
using App.Domain.Interfaces.Services;
using App.Domain.Notifications.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.Services
{
    public class SupplierService : BaseService, ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IAddressRepository _addressRepository;

        public SupplierService(ISupplierRepository supplierRepository, IAddressRepository addressRepository, INotifier notifier) : base (notifier)
        {
            _supplierRepository = supplierRepository;
            _addressRepository = addressRepository;
        }

        public async Task Add(Supplier supplier)
        {
            if (!RunValidator(new SupplierValidator(), supplier) ||
                !RunValidator(new AddressValidator(), supplier.Address))
                return;

            if (_supplierRepository.Get(s => s.Document == supplier.Document).Result.Any())
            {
                Notify("Já existe um fornecedor com o documento informado!");

                return;
            }

            await _supplierRepository.Add(supplier);
        }

        public async Task Delete(Guid id)
        {
            if (_supplierRepository.GetSupplierAddressProducts(id).Result.Products.Any())
            {
                Notify("O fornecedor possui produtos cadastrados!");

                return;
            }

            await _supplierRepository.Delete(id);
        }

        public async Task Update(Supplier supplier)
        {
            if (!RunValidator(new SupplierValidator(), supplier))
                return;

            if (_supplierRepository.Get(s => s.Document == supplier.Document && s.Id != supplier.Id).Result.Any())
            {
                Notify("Já existe um fornecedor com o documento informado!");

                return;
            }

            await _supplierRepository.Update(supplier);
        }

        public async Task UpdateAddress(Address address)
        {
            if (!RunValidator(new AddressValidator(), address))
                return;

            await _addressRepository.Update(address);
        }

        public void Dispose()
        {
            _supplierRepository?.Dispose();
            _addressRepository?.Dispose();
        }
    }
}
