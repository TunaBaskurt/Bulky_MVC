using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NuGet.Versioning;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList=_unitOfWork.Product.GetAll(includeProperties:"Category");
            return View(productList);//View folderına gidiyor ve Indexe giriyor
        }
        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new()
            {
                Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category"),
                Count = 1,
                ProductId = productId
            };

            return View(cart);//View folderına gidiyor ve Indexe giriyor
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)//Add to Carta tıklayınca çalışır
        {
           var claimsIdentity=(ClaimsIdentity)User.Identity;
           var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
           shoppingCart.ApplicationUserId= userId;

            ShoppingCart cartFromDb=_unitOfWork.ShoppingCart.Get(u=>u.ApplicationUserId== userId &&u.ProductId==shoppingCart.ProductId);

            if (cartFromDb != null) {
                //shopping cart exists
                cartFromDb.Count += shoppingCart.Count;//kullanıcının eklediği sayıda ekleme yapılıyor
                _unitOfWork.ShoppingCart.Update(cartFromDb);//sayı güncelleniyor
            
            }else//add cart to record
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart); 
            }
            
            _unitOfWork.Save();


            return RedirectToAction(nameof(Index));//View folderına gidiyor ve Indexe giriyor
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
