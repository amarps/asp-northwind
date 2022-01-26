using Microsoft.AspNetCore.Mvc;
using Piranha;
using Piranha.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Packt.Shared;
using NorthwindCms.Models;
using Microsoft.EntityFrameworkCore;

namespace NorthwinCms.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ImportController : Controller
    {
        private readonly IApi _api;
        private readonly Northwind db;

        public ImportController(IApi api, Northwind injectedContext)
        {
            _api = api;
            db = injectedContext;
        }

        [Route("/impurt")]
        public IActionResult impirt()
        {
            return Redirect("~/manager");
        }

        [Route("/import")]
        public async Task<IActionResult> import()
        {
            int importCount = 0;
            int existCount = 0;

            var site = await _api.Sites.GetDefaultAsync();

            var catalog = await _api.Pages
                .GetBySlugAsync<NorthwindCms.Models.CatalogPage>("catalog");

            foreach (Category c in db.Categories.Include(c => c.Products))
            {
                CategoryPage cp = await _api.Pages.GetBySlugAsync<CategoryPage>(
                    $"catalog/{c.CategoryName.ToLower().Replace(' ', '-')}");

                if (cp == null)
                {
                    importCount++;
                    cp = await CategoryPage.CreateAsync(_api);

                    
                    cp.Id = Guid.NewGuid();
                    cp.SiteId = site.Id;
                    cp.ParentId = catalog.Id;
                    cp.CategoryDetail.CategoryID = c.CategoryID;
                    cp.CategoryDetail.CategoryName = c.CategoryName;
                    cp.CategoryDetail.Description = c.Description;

                    // find the media folder named categories
                    Guid categoriesFolderId = (await _api.Media.GetAllFoldersAsync())
                        .First(folder => folder.Name == "Categories").Id;

                    // find
                    var image = (await _api.Media.GetAllByFolderIdAsync(categoriesFolderId))
                        .First(media => media.Type == MediaType.Image &&
                               media.Filename == $"category{c.CategoryID}.jpeg");

                    cp.CategoryDetail.CategoryImage = image;

                    if (cp.Products.Count == 0)
                    {
                        // convert the products for this category into
                        // a list of instances of ProductRegion
                        cp.Products = c.Products
                            .Select(p => new ProductRegion
                            {
                                ProductID = p.ProductID,
                                ProductName = p.ProductName,
                                UnitPrice = p.UnitPrice.HasValue
                                ? p.UnitPrice.Value.ToString() : "n/a",
                                UnitsInStock = p.UnitsInStock ?? 0
                            }).ToList();
                    }

                    cp.Title = c.CategoryName;
                    cp.MetaDescription = c.Description;
                    cp.NavigationTitle = c.CategoryName;
                    cp.Published = DateTime.Now;

                    await _api.Pages.SaveAsync(cp);
                }
                    
                else
                {
                    existCount++;
                }
            }
            TempData["import_message"] = $"{existCount} categories already existed. {importCount} new categories imported";
            return Redirect("~/");
        }
    }
}
