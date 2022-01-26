using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using Packt.Shared;

namespace NorthwindWeb.Pages
{
    public class SuppliersModel : PageModel
    {
        public IEnumerable<string> Suppliers {get; set;}
        private Northwind db;
        public SuppliersModel(Northwind injectedContext)
        {
            db = injectedContext;
        }

        [BindProperty]
        public Supplier Supplier {get; set;}

        public IActionResult OnPost()
        {
            if(ModelState.IsValid)
            {
                db.Suppliers.Add(Supplier);
                db.SaveChanges();
                return RedirectToPage("/suppliers");
            }
            return Page();
        }

        public void OnGet()
        {
            ViewData["Title"] = "Northwind Web site - Suppliers";

            Suppliers = db.Suppliers.Select(s => s.CompanyName);
        }
    }
}
