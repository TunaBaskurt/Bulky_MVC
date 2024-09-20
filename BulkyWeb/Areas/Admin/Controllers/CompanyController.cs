
using Bulky.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;

using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Bulky.Utiliy;
using Microsoft.AspNetCore.Authorization;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
   // [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnviorment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnviorment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnviorment = webHostEnviorment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();

            return View(objProductList);
        }
        public IActionResult Upsert(int? id)
        {

            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category
                .GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            if (id == null || id == 0)
            {//create
                return View(productVM);
            }
            else
            {//update
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }

        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {

            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnviorment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {//Delete the old ımage
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }
                if (productVM.Product.Id == 0)
                {//adding

                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {//update
                    _unitOfWork.Product.Update(productVM.Product);

                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category
               .GetAll().Select(u => new SelectListItem
               {
                   Text = u.Name,
                   Value = u.Id.ToString()
               });
                return View(productVM);
            }

        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(int id)
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {

            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);


            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error is occured while deleting" });
            }


            var oldImagePath = Path.Combine(_webHostEnviorment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();



            return Json(new { success = true, message = "Delete Successfull" });

        }


        #endregion
    }
}

