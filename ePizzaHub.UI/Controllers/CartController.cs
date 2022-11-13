using ePizzaHub.DAL.Entities;
using ePizzaHub.Models;
using ePizzaHub.Services.Interfaces;
using ePizzaHub.UI.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Text.Json;


namespace ePizzaHub.UI.Controllers
{
    public class CartController : Controller
    {
        ICartService _cartService;
        IUserAccessor _userAccessor;
        public CartController(ICartService cartService, IUserAccessor userAccessor)
        {
            _cartService = cartService;
            _userAccessor = userAccessor;
        }
        Guid cartId
        {
            get
            {
                Guid Id;
                string CId = Request.Cookies["CId"];
                if (string.IsNullOrEmpty(CId))
                {
                    Id = Guid.NewGuid();
                    Response.Cookies.Append("CId", Id.ToString(), new CookieOptions { Expires = DateTime.Now.AddDays(1) });
                }
                else
                {
                    Id = Guid.Parse(CId);
                }
                return Id;
            }
        }
        UserModel CurrentUser
        {
            get
            {
                return _userAccessor.GetUser();
            }
        }

        public IActionResult Index()
        {
            CartModel cart = _cartService.GetCartDetails(cartId);
            return View(cart);
        }

        [Route("Cart/AddToCart/{ItemId}/{UnitPrice}/{Quantity}")]
        public IActionResult AddToCart(int ItemId, decimal UnitPrice, int Quantity)
        {
            int UserId = CurrentUser != null ? CurrentUser.Id : 0;

            if (ItemId > 0 && Quantity > 0)
            {
                Cart cart = _cartService.AddItem(UserId, cartId, ItemId, UnitPrice, Quantity);
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };
                var data = JsonSerializer.Serialize(cart, options);
                return Json(data);
            }
            else
            {
                return Json("");
            }
        }

        [Route("Cart/UpdateQuantity/{Id}/{Quantity}")]
        public IActionResult UpdateQuantity(int Id, int Quantity)
        {
            if (Quantity == 0)
            {
                int countDelete = _cartService.DeleteItem(cartId, Id);
                return Json(countDelete);
            }
            int count = _cartService.UpdateQuantity(cartId, Id, Quantity);
            return Json(count);
        }

        public IActionResult DeleteItem(int Id)
        {
            int count = _cartService.DeleteItem(cartId, Id);
            return Json(count);
        }
        public IActionResult GetCartCount()
        {
            int count = _cartService.GetCartCount(cartId);
            return Json(count);
        }

        public IActionResult CheckOut()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CheckOut(AddressModel model)
        {
            CartModel cart = _cartService.GetCartDetails(cartId);
            if (CurrentUser != null && cart != null && cart.UserId == 0)
            {
                _cartService.UpdateCart(cart.Id, CurrentUser.Id);
                cart.UserId = cart.UserId;
            }
            TempData.Set("Cart", cart);
            TempData.Set("Address", model);
            return RedirectToAction("Index", "Payment");
        }
    }
}
