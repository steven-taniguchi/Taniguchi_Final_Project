using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Taniguchi_Final_Project.Models;

namespace Taniguchi_Final_Project.Controllers
{
    public class InvoicesController : Controller
    {
        // GET: Invoices
        /// <summary>
        /// Displays the view for all invoices
        /// </summary>
        /// <param name="id">The search term</param>
        /// <param name="sortBy">0 = Id, 1 = CustomerID, 2 = Date, 3 = Product total, 4 = Sales tax, 5 = Shipping, 6 = Total</param>
        /// <param name="isDesc">Set ascending or descending on header click</param>
        /// <returns></returns>
        public ActionResult All(string id, int sortBy = 0, bool isDesc = false)
        {
            BooksEntities context = new BooksEntities();
            List<Invoice> invoices;

            switch (sortBy)
            {
                case 1:
                    {
                        if (isDesc)
                        {
                            invoices = context.Invoices.OrderByDescending(i => i.Customer_Id).ToList();
                        }
                        else
                        {
                            invoices = context.Invoices.OrderBy(i => i.Customer_Id).ToList();
                        }
                        break;
                    }
                case 2:
                    {
                        if (isDesc)
                        {
                            invoices = context.Invoices.OrderByDescending(i => i.Date).ToList();
                        }
                        else
                        {
                            invoices = context.Invoices.OrderBy(i => i.Date).ToList();
                        }
                        break;
                    }
                case 3:
                    {
                        if (isDesc)
                        {
                            invoices = context.Invoices.OrderByDescending(i => i.Product_Total).ToList();
                        }
                        else
                        {
                            invoices = context.Invoices.OrderBy(i => i.Product_Total).ToList();
                        }
                        break;
                    }
                case 4:
                    {
                        if (isDesc)
                        {
                            invoices = context.Invoices.OrderByDescending(i => i.Sales_Tax).ToList();
                        }
                        else
                        {
                            invoices = context.Invoices.OrderBy(i => i.Sales_Tax).ToList();
                        }
                        break;
                    }
                case 5:
                    {
                        if (isDesc)
                        {
                            invoices = context.Invoices.OrderByDescending(i => i.Shipping).ToList();
                        }
                        else
                        {
                            invoices = context.Invoices.OrderBy(i => i.Shipping).ToList();
                        }
                        break;
                    }
                case 6:
                    {
                        if (isDesc)
                        {
                            invoices = context.Invoices.OrderByDescending(i => i.Total).ToList();
                        }
                        else
                        {
                            invoices = context.Invoices.OrderBy(i => i.Total).ToList();
                        }
                        break;
                    }
                case 0:
                default:
                    {
                        if (isDesc)
                        {
                            invoices = context.Invoices.OrderByDescending(i => i.Id).ToList();
                        }
                        else
                        {
                            invoices = context.Invoices.OrderBy(i => i.Id).ToList();
                        }
                        break;
                    }
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Trim().ToLower();

                invoices = invoices.Where(i =>
                        i.Id.ToString().Contains(id) ||
                        i.Customer_Id.ToString().Contains(id) ||
                        i.Date.ToString().Contains(id) ||
                        i.Product_Total.ToString().Contains(id) ||
                        i.Sales_Tax.ToString().Contains(id) ||
                        i.Shipping.ToString().Contains(id) ||
                        i.Total.ToString().Contains(id)
                    ).ToList();
            }

            invoices = invoices.Where(i => i.Is_Deleted == false).ToList();

            return View(invoices);
        }

        /// <summary>
        /// Allows for edit of entry
        /// </summary>
        /// <param name="id">Id of invoice to edit</param>
        /// <returns>Invoice view</returns>
        [HttpGet]
        public ActionResult Upsert(int id = 0)
        {
            BooksEntities context = new BooksEntities();

            // If no invoice in the DB, Return a new instance of Invoice Object
            Invoice invoice = context.Invoices.Where(i => i.Id == id).FirstOrDefault() ?? new Invoice();
            List<Customer> customers = context.Customers.ToList();

            UpsertInvoiceModel viewModel = new UpsertInvoiceModel()
            {
                Invoice = invoice,
                Customer = customers
            };

            if (invoice.Is_Deleted)
            {
                return RedirectToAction("All");
            }
            return View(viewModel);
        }

        /// <summary>
        /// Upserts an invoice
        /// </summary>
        /// <param name="model">InvoiceDTO</param>
        /// <param name="customerId">CustomerID</param>
        /// <returns>Invoice view</returns>
        [HttpPost]
        public ActionResult Upsert(UpsertInvoiceModel model, string customerId)
        {
            Invoice newInvoice = model.Invoice;

            customerId = customerId.Split('-')[0].Trim();
            newInvoice.Customer_Id = Convert.ToInt32(customerId); 
            
            BooksEntities context = new BooksEntities();
            try
            {
                if (context.Invoices.Where(i => i.Id == newInvoice.Id).Count() > 0)
                {
                    var invoiceToSave = context.Invoices.Where(i => i.Id == newInvoice.Id).FirstOrDefault();
                    invoiceToSave.Customer_Id = newInvoice.Customer_Id;
                    invoiceToSave.Date = newInvoice.Date;
                    invoiceToSave.Product_Total = newInvoice.Product_Total;
                    invoiceToSave.Sales_Tax = newInvoice.Sales_Tax;
                    invoiceToSave.Shipping = newInvoice.Shipping;
                    invoiceToSave.Total = newInvoice.Total;
                }
                else
                {
                    context.Invoices.Add(newInvoice);
                }
                context.SaveChanges();
            }
            catch (DbEntityValidationException vex)
            {
                ViewBag.Error = $"<span class='error'>{vex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage}</span>";
                return View(newInvoice);
            }
            return RedirectToAction("All");
        }

        /// <summary>
        /// Deletes an invoice
        /// </summary>
        /// <param name="id">Invoice ID to edit</param>
        /// <returns>Invoice view</returns>
        [HttpGet]
        public ActionResult Delete(string id)
        {
            BooksEntities context = new BooksEntities();
            int invoiceId = 0;

            if (int.TryParse(id, out invoiceId))
            {
                try
                {
                    Invoice invoice = context.Invoices.Where(i => i.Id == invoiceId).FirstOrDefault();
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
                Success = true, Id = id, returnUrl = "/Invoices/All" }, JsonRequestBehavior.AllowGet);
        }
    }
}