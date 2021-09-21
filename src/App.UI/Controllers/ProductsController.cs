using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using App.UI.ViewModels;
using App.Domain.Entities;
using App.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace App.UI.Controllers
{
    [Route("products")]
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;

        public ProductsController(IProductRepository productRepository, ISupplierRepository supplierRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }
                
        public async Task<IActionResult> Index()
        {
            return View(_mapper.Map<IEnumerable<ProductViewModel>>(await _productRepository.GetProductsSuppliers()));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var productViewModel = await GetProduct(id);
            if (productViewModel == null)
                return NotFound();

            return View(productViewModel);
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            var productViewModel = new ProductViewModel();

            productViewModel.Suppliers = await GetSuppliers();

            return View(productViewModel);
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel productViewModel)
        {
            productViewModel.Suppliers = await GetSuppliers();

            if (!ModelState.IsValid)
                return View(productViewModel);

            var imagePrefix = $"{Guid.NewGuid()}_";

            if (!await UploadImage(productViewModel.ImageUpload, imagePrefix))
                return View(productViewModel);

            productViewModel.Image = $"{imagePrefix}{productViewModel.ImageUpload.FileName}";
            productViewModel.RegisterDate = DateTime.Now;

            await _productRepository.Add(_mapper.Map<Product>(productViewModel));

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var productViewModel = await GetProduct(id);
            if (productViewModel == null)
                return NotFound();

            return View(productViewModel);
        }

        [HttpPost("edit/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ProductViewModel productViewModel)
        {
            if (id != productViewModel.Id)            
                return NotFound();

            var productFromDb = await GetProduct(id);

            productViewModel.Supplier = productFromDb.Supplier;
            productViewModel.Image = productFromDb.Image;
            productViewModel.RegisterDate = productFromDb.RegisterDate;

            if (!ModelState.IsValid)
                return View(productViewModel);

            if (productViewModel.ImageUpload != null)
            {
                var imagePrefix = $"{Guid.NewGuid()}_";

                if (!await UploadImage(productViewModel.ImageUpload, imagePrefix))
                    return View(productViewModel);

                productViewModel.Image = $"{imagePrefix}{productViewModel.ImageUpload.FileName}";
            }

            productFromDb.Name = productViewModel.Name;
            productFromDb.Description = productViewModel.Description;
            productFromDb.Value = productViewModel.Value;
            productFromDb.Active = productViewModel.Active;

            await _productRepository.Update(_mapper.Map<Product>(productFromDb));

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var productViewModel = await GetProduct(id);
            if (productViewModel == null)
                return NotFound();

            return View(productViewModel);
        }

        [HttpPost("delete/{id:guid}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var productViewModel = await GetProduct(id);
            if (productViewModel == null)
                return NotFound();

            await _productRepository.Delete(id);

            return RedirectToAction(nameof(Index));
        }

        private async Task<ProductViewModel> GetProduct(Guid id)
        {
            var product = _mapper.Map<ProductViewModel>(await _productRepository.GetProductSupplier(id));
            
            product.Suppliers = _mapper.Map<IEnumerable<SupplierViewModel>>(await _supplierRepository.GetAll());
            
            return product;
        }

        private async Task<IEnumerable<SupplierViewModel>> GetSuppliers()
        {
            return _mapper.Map<IEnumerable<SupplierViewModel>>(await _supplierRepository.GetAll());
        }

        private async Task<bool> UploadImage(IFormFile file, string imagePrefix)
        {
            if (file.Length <= 0)
                return false;

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", $"{imagePrefix}{file.FileName}");

            if (System.IO.File.Exists(path))            
                ModelState.AddModelError(string.Empty, "Já existe um arquivo com este nome.");

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return true;
        }
    }
}
