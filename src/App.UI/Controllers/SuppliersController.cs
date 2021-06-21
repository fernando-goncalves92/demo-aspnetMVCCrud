using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using App.UI.ViewModels;
using App.Domain.Interfaces;
using App.Domain.Entities;

namespace App.UI.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly ISupplierRepository _repository;
        private readonly IMapper _mapper;

        public SuppliersController(ISupplierRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            return View(_mapper.Map<IEnumerable<SupplierViewModel>>(await _repository.GetAll()));
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var supplierViewModel = _mapper.Map<SupplierViewModel>(await _repository.GetSupplierAddress(id));
            if (supplierViewModel == null)
                return NotFound();
            
            return View(supplierViewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierViewModel supplierViewModel)
        {
            if (!ModelState.IsValid)
                return View(supplierViewModel);

            await _repository.Add(_mapper.Map<Supplier>(supplierViewModel));
            
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var supplierViewModel = _mapper.Map<SupplierViewModel>(await _repository.GetSupplierAddressProducts(id));
            if (supplierViewModel == null)
                return NotFound();

            return View(supplierViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, SupplierViewModel supplierViewModel)
        {
            if (id != supplierViewModel.Id)            
                return NotFound();

            if (!ModelState.IsValid)            
                return View(supplierViewModel);

            await _repository.Update(_mapper.Map<Supplier>(supplierViewModel));

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var supplierViewModel = _mapper.Map<SupplierViewModel>(await _repository.GetSupplierAddress(id));
            if (supplierViewModel == null)            
                return NotFound();

            return View(supplierViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var supplierViewModel = _mapper.Map<SupplierViewModel>(await _repository.GetSupplierAddress(id));
            if (supplierViewModel == null)            
                return NotFound();
            
            await _repository.Delete(id);
            
            return RedirectToAction(nameof(Index));
        }
    }
}
