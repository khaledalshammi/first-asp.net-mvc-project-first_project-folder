using Castle.Core.Internal;
using FirstProject.DataAccess.Data;
using FirstProject.DataAccess.Repository;
using FirstProject.DataAccess.Repository.IRepository;
using FirstProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Docs.Samples;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Localization;

namespace first_project.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class TestController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<TestController> _localizer;
        public TestController(ApplicationDbContext db, IUnitOfWork unitOfWork, IStringLocalizer<TestController> localizer)
        {
            _db = db;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }
        public IActionResult Index(string toSearch)
        {
            IEnumerable<Product> products = _db.Products.Where(u => u.Title!.Contains(toSearch));
            ViewData["count"] = products.Count();
            return View(products);
        }
        [HttpGet]
        [Route("All", Name = "All")]
        public ActionResult All()
        { 
            List<Company> companies = _unitOfWork.Company.GetAll().ToList();
            return View("All", companies);
        }
        public ActionResult Details(int id)
        {
            if (id != null || id != 0)
            {
                Company company = _unitOfWork.Company.Get(u => u.Id == id);
                return View(company);
            }
            return NotFound();
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (id !=  0 || id != null)
            {
               Company company = _unitOfWork.Company.Get(u=>u.Id == id);
               return View(company);
            }
            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Company company)
        {
            if (company != null)
            {
                _unitOfWork.Company.Update(company);
                _unitOfWork.Save();
                //return RedirectToAction(nameof(All));
                return RedirectToRoute("All");
            }
            return View(company);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Company company)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Company.Add(company);
                _unitOfWork.Save();
                return RedirectToAction(nameof(All), "Test");
                // return RedirectToAction(nameof(Edit), "Test", {id = 1});
            }
            return View();
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id != 0 || id != null )
            {
                Company company = _unitOfWork.Company.Get(i=>i.Id == id);
                return View(company);
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Company company)
        {
            if (company != null)
            {
                _unitOfWork.Company.Remove(company);
                _unitOfWork.Save();
                return RedirectToAction(nameof(All));
            }
            return View();
        }
        public IActionResult Detail(int id)
        {
            return ControllerContext.MyDisplayRouteInfo(id);
        }
        public IActionResult TestIt()
        {
            var fullname = typeof(TestController).FullName;
            var template =
                ControllerContext.ActionDescriptor.AttributeRouteInfo?.Template;
            var path = Request.Path.Value;

            return Content($"Path: {path} fullname: {fullname}  template:{template}");
        }
        [HttpGet]
        public IActionResult Translate()
        {
            return View();
        }
        public IActionResult Googlemap()
        {
            return View();
        }
        public IActionResult Googlemaptow()
        {
            return View();
        }
        public int TestAdding(int a, int b)
        {
            return a + b;
        }
    }
}
