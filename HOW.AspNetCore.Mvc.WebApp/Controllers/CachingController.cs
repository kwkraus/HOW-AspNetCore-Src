using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HOW.AspNetCore.Mvc.WebApp.Controllers
{
    public class CachingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}