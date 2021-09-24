using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using App.UI.ViewModels;
using App.Domain.Entities;
using App.Domain.Interfaces.Repositories;
using App.Domain.Interfaces.Services;
using App.Domain.Notifications.Interfaces;
using Microsoft.AspNetCore.Authorization;
using App.UI.Authorize.Attributes;

namespace App.UI.Controllers
{
    [Route("suppliers")]
    [Authorize]
    public class SuppliersController : BaseController
    {
        private readonly ISupplierService _supplierService;
        private readonly IAddressRepository _addressRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;

        public SuppliersController(
            ISupplierRepository supplierRepository, 
            ISupplierService supplierService, 
            INotifier notifier,
            IMapper mapper) : base(notifier)
        {
            _supplierRepository = supplierRepository;
            _supplierService = supplierService;
            _mapper = mapper;
        }

        [HttpGet()]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(_mapper.Map<IEnumerable<SupplierViewModel>>(await _supplierRepository.GetAll()));
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid id)
        {
            var supplierViewModel = _mapper.Map<SupplierViewModel>(await _supplierRepository.GetSupplierAddress(id));
            if (supplierViewModel == null)
                return NotFound();
            
            return View(supplierViewModel);
        }

        [HttpGet("create")]
        [ClaimsAuthorize("Supplier", "Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("create")]        
        [ClaimsAuthorize("Supplier", "Create")]
        public async Task<IActionResult> Create(SupplierViewModel supplierViewModel)
        {
            if (!ModelState.IsValid)
                return View(supplierViewModel);

            await _supplierService.Add(_mapper.Map<Supplier>(supplierViewModel));

            if (!ValidOperation())
                return View(supplierViewModel);

            TempData["Sucess"] = "Fornecedor cadastrado com sucesso!";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("edit/{id:guid}")]
        [ClaimsAuthorize("Supplier", "Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var supplierViewModel = _mapper.Map<SupplierViewModel>(await _supplierRepository.GetSupplierAddressProducts(id));
            if (supplierViewModel == null)
                return NotFound();

            return View(supplierViewModel);
        }

        [HttpPost("edit/{id:guid}")]        
        [ClaimsAuthorize("Supplier", "Edit")]
        public async Task<IActionResult> Edit(Guid id, SupplierViewModel supplierViewModel)
        {
            if (id != supplierViewModel.Id)            
                return NotFound();

            if (!ModelState.IsValid)            
                return View(supplierViewModel);

            await _supplierService.Update(_mapper.Map<Supplier>(supplierViewModel));

            if (!ValidOperation())
                return View(_mapper.Map<SupplierViewModel>(await _supplierRepository.GetSupplierAddressProducts(id)));

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("delete/{id:guid}")]
        [ClaimsAuthorize("Supplier", "Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var supplierViewModel = _mapper.Map<SupplierViewModel>(await _supplierRepository.GetSupplierAddress(id));
            if (supplierViewModel == null)            
                return NotFound();

            return View(supplierViewModel);
        }

        [HttpPost("delete/{id:guid}"), ActionName("Delete")]        
        [ClaimsAuthorize("Supplier", "Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var supplierViewModel = _mapper.Map<SupplierViewModel>(await _supplierRepository.GetSupplierAddressProducts(id));
            if (supplierViewModel == null)            
                return NotFound();

            await _supplierService.Delete(id);

            if (!ValidOperation())
                return View(supplierViewModel);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("{id:guid}/address")]        
        [AllowAnonymous]
        public async Task<IActionResult> GetAddress(Guid id)
        {   
            var supplier = _mapper.Map<SupplierViewModel>(await _supplierRepository.GetSupplierAddress(id));
            if (supplier == null)            
                return NotFound();            

            return PartialView("_AddressDetails", supplier);
        }

        [HttpGet("{id:guid}/update-address")]
        [ClaimsAuthorize("Supplier", "Edit")]
        public async Task<IActionResult> UpdateAddress(Guid id)
        {
            var supplier = _mapper.Map<SupplierViewModel>(await _supplierRepository.GetSupplierAddress(id));
            if (supplier == null)            
                return NotFound();

            return PartialView("_AddressUpdate", new SupplierViewModel { Address = supplier.Address });
        }

        [HttpPost("{id:guid}/update-address")]
        [ClaimsAuthorize("Supplier", "Edit")]
        public async Task<IActionResult> UpdateAddress(SupplierViewModel supplierViewModel)
        {
            ModelState.Remove("Name");
            ModelState.Remove("Document");

            if (!ModelState.IsValid) 
                return PartialView("_AddressUpdate", supplierViewModel);

            await _supplierService.UpdateAddress(_mapper.Map<Address>(supplierViewModel.Address));

            if (!ValidOperation())
                return PartialView("_AddressUpate", supplierViewModel);

            var url = Url.Action("GetAddress", "Suppliers", new { id = supplierViewModel.Address.SupplierId });
            
            return Json(new { success = true, url });
        }    
    }
}
