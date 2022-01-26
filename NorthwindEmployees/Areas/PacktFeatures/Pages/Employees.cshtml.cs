using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Packt.Shared;

namespace PacktFeatures.Pages
{
    public class EmployeesPageModel : PageModel
    {
        public IEnumerable<Employee> Employees {get; set;}
        private Northwind db;
        public EmployeesPageModel(Northwind injectedContext)
        {
            db = injectedContext;
        }
        
        public void OnGet()
        {
            Employees = db.Employees.ToArray();
        }
    }
}
