using FirstProject.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using FirstProject.Models;
using FirstProject.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using FirstProject.Models.ViewModels;
using FirstProject.Utility;
using Microsoft.AspNetCore.Authorization;

namespace first_project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return View(objCompanyList);
        }
        public IActionResult Details(int? id)
        {
            var Company= _unitOfWork.Company.Get(u => u.Id == id);
            return View(Company);
        }
        //get
//        public IActionResult Create()
        public IActionResult Upsert(int? id) // update edit
        {
            if (id == null || id == 0)
            {
                return View(new Company());
            }
            else // update
            {
                Company companyobj = _unitOfWork.Company.Get(u => u.Id == id);
                return View(companyobj);
            }
            
        }
        //create
        [HttpPost]
        public IActionResult Upsert(Company Compayobj)
        {
            if (ModelState.IsValid)
            {
                if (Compayobj.Id == 0 || Compayobj.Id == null)
                {
                    _unitOfWork.Company.Add(Compayobj);
                }
                else
                {
                    _unitOfWork.Company.Update(Compayobj);
                }
                _unitOfWork.Save();
                TempData["success"] = "Created successfully!";
                return RedirectToAction("Index", "Company");
            }
            return View(Compayobj);
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var Company = _unitOfWork.Company.Get(u => u.Id == id);
            if (Company == null)
            {
                return NotFound();
            }
            return View(Company);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _unitOfWork.Company.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Company.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Deleted successfully!";
            return RedirectToAction("Index", "Company");
        }
    }
}
