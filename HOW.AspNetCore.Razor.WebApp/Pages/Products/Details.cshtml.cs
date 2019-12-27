﻿using HOW.AspNetCore.Data.Entities;
using HOW.AspNetCore.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace HOW.AspNetCore.Razor.WebApp.Pages.Products
{
    public class DetailsModel : PageModel
    {
        private readonly IProductService _productSvc;

        public DetailsModel(IProductService productService)
        {
            _productSvc = productService;
        }

        public Product Product { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            Product = await _productSvc.GetProductAsync(id.GetValueOrDefault());

            if (Product == null)
                return NotFound();

            return Page();
        }
    }
}
