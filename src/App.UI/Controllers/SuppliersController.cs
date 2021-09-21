using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using App.UI.ViewModels;
using App.Domain.Interfaces;
using App.Domain.Entities;
using System.Linq;

namespace App.UI.Controllers
{   
    [Route("suppliers")]
    public class SuppliersController : Controller
    {
        private readonly IAddressRepository _addressRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;

        public SuppliersController(ISupplierRepository supplierRepository, IAddressRepository addressRepository, IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _addressRepository = addressRepository;
            _mapper = mapper;
        }

        [HttpGet()]
        public async Task<IActionResult> Index()
        {
            return View(_mapper.Map<IEnumerable<SupplierViewModel>>(await _supplierRepository.GetAll()));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var supplierViewModel = _mapper.Map<SupplierViewModel>(await _supplierRepository.GetSupplierAddress(id));
            if (supplierViewModel == null)
                return NotFound();
            
            return View(supplierViewModel);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]        
        public async Task<IActionResult> Create(SupplierViewModel supplierViewModel)
        {
            if (!ModelState.IsValid)
                return View(supplierViewModel);

            await _supplierRepository.Add(_mapper.Map<Supplier>(supplierViewModel));
            
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var supplierViewModel = _mapper.Map<SupplierViewModel>(await _supplierRepository.GetSupplierAddressProducts(id));
            if (supplierViewModel == null)
                return NotFound();

            return View(supplierViewModel);
        }

        [HttpPost("edit/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, SupplierViewModel supplierViewModel)
        {
            if (id != supplierViewModel.Id)            
                return NotFound();

            if (!ModelState.IsValid)            
                return View(supplierViewModel);

            await _supplierRepository.Update(_mapper.Map<Supplier>(supplierViewModel));

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var supplierViewModel = _mapper.Map<SupplierViewModel>(await _supplierRepository.GetSupplierAddress(id));
            if (supplierViewModel == null)            
                return NotFound();

            return View(supplierViewModel);
        }

        [HttpPost("delete/{id:guid}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var supplierViewModel = _mapper.Map<SupplierViewModel>(await _supplierRepository.GetSupplierAddressProducts(id));
            if (supplierViewModel == null)            
                return NotFound();

            // TODO: Colocar mensagem informando que há produtos vinculados ao forncedor
            if (supplierViewModel.Products != null && supplierViewModel.Products.Count() > 0)
                return View(supplierViewModel); 

            await _addressRepository.Delete(supplierViewModel.Address.Id);
            await _supplierRepository.Delete(id);
            
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("{id:guid}/address")]        
        public async Task<IActionResult> GetAddress(Guid id)
        {   
            var supplier = _mapper.Map<SupplierViewModel>(await _supplierRepository.GetSupplierAddress(id));
            if (supplier == null)            
                return NotFound();            

            return PartialView("_AddressDetails", supplier);
        }

        [HttpGet("{id:guid}/update-address")]
        public async Task<IActionResult> UpdateAddress(Guid id)
        {
            var supplier = _mapper.Map<SupplierViewModel>(await _supplierRepository.GetSupplierAddress(id));
            if (supplier == null)            
                return NotFound();

            return PartialView("_AddressUpdate", new SupplierViewModel { Address = supplier.Address });
        }

        [HttpPost("{id:guid}/update-address")]
        public async Task<IActionResult> UpdateAddress(SupplierViewModel supplierViewModel)
        {
            ModelState.Remove("Name");
            ModelState.Remove("Document");

            if (!ModelState.IsValid) 
                return PartialView("_AddressUpdate", supplierViewModel);

            await _addressRepository.Update(_mapper.Map<Address>(supplierViewModel.Address));

            var url = Url.Action("GetAddress", "Suppliers", new { id = supplierViewModel.Address.SupplierId });
            
            return Json(new { success = true, url });
        }    
    }
}
