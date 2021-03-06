using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Taniguchi_Final_Project.Models;

namespace Taniguchi_Final_Project.Controllers
{
    public class ProductsController : Controller
    {
        /// <summary>
        /// Displays the view for all products
        /// </summary>
        /// <param name="id">The search term</param>
        /// <param name="sortBy">0 = Code, 1 = Description, 2 = Unit price, 3 = on hand quantity, 4 = State, 5 =  Zip code</param>
        /// <param name="isDesc">Set ascending or descending on header click</param>
        /// <returns>Product view</returns>
        public ActionResult All(string id, int sortBy = 0, bool isDesc = false)
        {
            BooksEntities context = new BooksEntities();
            List<Product> products;

            switch (sortBy)
            {
                case 1:
                    {
                        if (isDesc)
                        {
                            products = context.Products.OrderByDescending(p => p.Description).ToList();
                        }
                        else
                        {
                            products = context.Products.OrderBy(p => p.Description).ToList();
                        }
                        break;
                    }
                case 2:
                    {
                        if (isDesc)
                        {
                            products = context.Products.OrderByDescending(p => p.Unit_Price).ToList();
                        }
                        else
                        {
                            products = context.Products.OrderBy(p => p.Unit_Price).ToList();
                        }
                        break;
                    }
                case 3:
                    {
                        if (isDesc)
                        {
                            products = context.Products.OrderByDescending(p => p.On_Hand_Quantity).ToList();
                        }
                        else
                        {
                            products = context.Products.OrderBy(p => p.On_Hand_Quantity).ToList();
                        }
                        break;
                    }
                case 0:
                default:
                    {
                        if (isDesc)
                        {
                            products = context.Products.OrderByDescending(p => p.Code).ToList();
                        }
                        else
                        {
                            products = context.Products.OrderBy(p => p.Code).ToList();
                        }
                        break;
                    }
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Trim().ToLower();

                products = products.Where(p =>
                        p.Code.ToLower().Contains(id) ||
                        p.Description.ToLower().Contains(id) ||
                        p.Unit_Price.ToString().Contains(id) ||
                        p.On_Hand_Quantity.ToString().Contains(id)
                    ).ToList();
            }

            products = products.Where(p => p.Is_Deleted == false).ToList();

            return View(products);
        }

        /// <summary>
        /// Allows for edit of entry
        /// </summary>
        /// <param name="id">Product Code</param>
        /// <returns>Product view</returns>
        [HttpGet]
        public ActionResult Upsert(string id = "")
        {
            BooksEntities context = new BooksEntities();

            // If no product in the DB, Return a new instance of Product Object
            Product product = context.Products.Where(p => p.Code == id).FirstOrDefault() ?? new Product();

            if (product.Is_Deleted)
            {
                return RedirectToAction("All");
            }
            return View(product);
        }

        /// <summary>
        /// Upserts a product
        /// </summary>
        /// <param name="newProduct">New product to add to database</param>
        /// <returns>All view</returns>
        [HttpPost]
        public ActionResult Upsert(Product newProduct)
        {
            BooksEntities context = new BooksEntities();
            try
            {
                if (context.Products.Where(p => p.Code == newProduct.Code).Count() > 0)
                {
                    var productToSave = context.Products.Where(p => p.Code == newProduct.Code).FirstOrDefault();
                    productToSave.Code = newProduct.Code;
                    productToSave.Description= newProduct.Description;
                    productToSave.Unit_Price= newProduct.Unit_Price;
                    productToSave.On_Hand_Quantity = newProduct.On_Hand_Quantity;
                }
                else
                {
                    context.Products.Add(newProduct);
                }
                context.SaveChanges();
            }
            catch (DbEntityValidationException vex)
            {
                ViewBag.Error = $"<span class='error'>{vex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage}</span>";
                return View(newProduct);
            }
            return RedirectToAction("All");
        }

        /// <summary>
        /// Deletes a product
        /// </summary>
        /// <param name="code">Product code to delete</param>
        /// <returns>Product view</returns>
        [HttpGet]
        public ActionResult Delete(string code)
        {
            BooksEntities context = new BooksEntities();
            try
            {
                Product product = context.Products.Where(p => p.Code == code).FirstOrDefault();
                product.Is_Deleted = true;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Code = code,
                    Message = ex.Message
                });
            }
            return Json(new
            {
                Success = true,
                Code = code,
                returnUrl = "/Products/All"
            }, JsonRequestBehavior.AllowGet);    
        }
    }
}