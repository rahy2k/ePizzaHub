﻿using Microsoft.AspNetCore.Mvc;

namespace ePizzaHub.UI.Areas.User.Controllers
{
   
    public class DashboardController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
