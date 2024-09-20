
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
    //[Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnviorment;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            
        }
        public IActionResult Index()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();

            return View(objCompanyList);
        }
        public IActionResult Upsert(int? id)
        {

          
            if (id == null || id == 0)
            {//create
                return View(new Company());
            }
            else
            {//update
                Company companyObj = _unitOfWork.Company.Get(u => u.Id == id);
                return View(companyObj);
            }

        }
        [HttpPost]
        public IActionResult Upsert(Company CompanyObj, IFormFile? file)
        {

            if (ModelState.IsValid)
            {
                
               
                if (CompanyObj.Id == 0)
                {//adding

                    _unitOfWork.Company.Add(CompanyObj);
                }
                else
                {//update
                    _unitOfWork.Company.Update(CompanyObj);

                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
               
                return View(CompanyObj);
            }

        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(int id)
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = objCompanyList });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {

            var CompanyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);


            if (CompanyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error is occured while deleting" });
            }


   

            _unitOfWork.Company.Remove(CompanyToBeDeleted);
            _unitOfWork.Save();



            return Json(new { success = true, message = "Delete Successfull" });

        }


        #endregion
    }
}

