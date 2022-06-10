using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Taniguchi_Final_Project.Models;

namespace Taniguchi_Final_Project.Controllers
{
    public class OrderOptionsController : Controller
    {
        // GET: OrderOptions
        /// <summary>
        /// Displays a view for all orderoptions
        /// </summary>
        /// <param name="id">The search term</param>
        /// <param name="sortBy">0 = Id, 1 = Sales_Tax_Rate, 2 = First_Book_Ship_Charge, 3 = Additional_Book_Ship_Charge</param>
        /// <param name="isDesc">Set ascending or descending on header click</param>
        /// <returns>Order option view</returns>
        public ActionResult All(string id, int sortBy = 0, bool isDesc = false)
        {
            BooksEntities context = new BooksEntities();
            List<OrderOption> orderOptions;

            switch (sortBy)
            {
                case 1:
                    {
                        if (isDesc)
                        {
                            orderOptions = context.OrderOptions.OrderByDescending(oo => oo.Sales_Tax_Rate).ToList();
                        }
                        else
                        {
                            orderOptions = context.OrderOptions.OrderBy(oo => oo.Sales_Tax_Rate).ToList();
                        }
                        break;
                    }
                case 2:
                    {
                        if (isDesc)
                        {
                            orderOptions = context.OrderOptions.OrderByDescending(oo => oo.First_Book_Ship_Charge).ToList();
                        }
                        else
                        {
                            orderOptions = context.OrderOptions.OrderBy(oo => oo.First_Book_Ship_Charge).ToList();
                        }
                        break;
                    }
                case 3:
                    {
                        if (isDesc)
                        {
                            orderOptions = context.OrderOptions.OrderByDescending(oo => oo.Additional_Book_Ship_Charge).ToList();
                        }
                        else
                        {
                            orderOptions = context.OrderOptions.OrderBy(oo => oo.Additional_Book_Ship_Charge).ToList();
                        }
                        break;
                    }
                case 0:
                default:
                    {
                        if (isDesc)
                        {
                            orderOptions = context.OrderOptions.OrderByDescending(oo => oo.Id).ToList();
                        }
                        else
                        {
                            orderOptions = context.OrderOptions.OrderBy(oo => oo.Id).ToList();
                        }
                        break;
                    }
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Trim().ToLower();

                orderOptions = orderOptions.Where(oo =>
                        oo.Sales_Tax_Rate.ToString().Contains(id) ||
                        oo.First_Book_Ship_Charge.ToString().Contains(id) ||
                        oo.Additional_Book_Ship_Charge.ToString().Contains(id)
                    ).ToList();
            }

            orderOptions = orderOptions.Where(oo => oo.Is_Deleted == false).ToList();

            return View(orderOptions);
        }

        /// <summary>
        /// Allows for edit of entry
        /// </summary>
        /// <param name="id">Order option Id to edit</param>
        /// <returns>Order option view</returns>
        [HttpGet]
        public ActionResult Upsert(int id = 0)
        {
            BooksEntities context = new BooksEntities();

            // If no invoice in the DB, Return a new instance of OrderOption Object
            OrderOption orderOption = context.OrderOptions.Where(oo => oo.Id == id).FirstOrDefault() ?? new OrderOption();
            

            if (orderOption.Is_Deleted)
            {
                return RedirectToAction("All");
            }
            return View(orderOption);
        }

        /// <summary>
        /// Upserts an orderoption
        /// </summary>
        /// <param name="newOrderOption">New order option to add to database</param>
        /// <returns>All view</returns>
        [HttpPost]
        public ActionResult Upsert(OrderOption newOrderOption)
        {
            BooksEntities context = new BooksEntities();
            try
            {
                if (context.OrderOptions.Where(oo => oo.Id == newOrderOption.Id).Count() > 0)
                {
                    var orderOptionToSave = context.OrderOptions.Where(oo => oo.Id == newOrderOption.Id).FirstOrDefault();
                    orderOptionToSave.Id = newOrderOption.Id;
                    orderOptionToSave.Sales_Tax_Rate = newOrderOption.Sales_Tax_Rate;
                    orderOptionToSave.First_Book_Ship_Charge = newOrderOption.First_Book_Ship_Charge;
                    orderOptionToSave.Additional_Book_Ship_Charge = newOrderOption.Additional_Book_Ship_Charge;
                }
                else
                {
                    context.OrderOptions.Add(newOrderOption);
                }
                context.SaveChanges();
            }
            catch (DbEntityValidationException vex)
            {
                ViewBag.Error = $"<span class='error'>{vex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage}</span>";
                return View(newOrderOption);
            }
            return RedirectToAction("All");
        }

        /// <summary>
        /// Deletes an order option
        /// </summary>
        /// <param name="id">Order option Id to delete</param>
        /// <returns>OrderOption view</returns>
        [HttpGet]
        public ActionResult Delete(string id)
        {
            BooksEntities context = new BooksEntities();
            int invoiceId = 0;

            if (int.TryParse(id, out invoiceId))
            {
                try
                {
                    OrderOption invoice = context.OrderOptions.Where(oo => oo.Id == invoiceId).FirstOrDefault();
                    invoice.Is_Deleted = true;
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        Success = false,
                        Id = id,
                        Message = ex.Message
                    });
                }
            }
            else
            {
                //Parsing not successful
            }
            return Json(new
            {
                Success = true,
                Id = id,
                returnUrl = "/OrderOptions/All"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}