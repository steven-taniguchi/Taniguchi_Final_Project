using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Taniguchi_Final_Project.Models;

namespace Taniguchi_Final_Project.Controllers
{
    public class InvoiceLineItemsController : Controller
    {
        // GET: InvoiceLineItems
        public ActionResult All(string id, int sortBy = 0, bool isDesc = false)
        {
            BooksEntities context = new BooksEntities();
            List<InvoiceLineItem> invoiceLineItems;

            switch (sortBy)
            {
                case 1:
                    {
                        if (isDesc)
                        {
                            invoiceLineItems = context.InvoiceLineItems.OrderByDescending(i => i.Product_Code).ToList();
                        }
                        else
                        {
                            invoiceLineItems = context.InvoiceLineItems.OrderBy(i => i.Product_Code).ToList();
                        }
                        break;
                    }
                case 2:
                    {
                        if (isDesc)
                        {
                            invoiceLineItems = context.InvoiceLineItems.OrderByDescending(i => i.Unit_Price).ToList();
                        }
                        else
                        {
                            invoiceLineItems = context.InvoiceLineItems.OrderBy(i => i.Unit_Price).ToList();
                        }
                        break;
                    }
                case 3:
                    {
                        if (isDesc)
                        {
                            invoiceLineItems = context.InvoiceLineItems.OrderByDescending(i => i.Quantity).ToList();
                        }
                        else
                        {
                            invoiceLineItems = context.InvoiceLineItems.OrderBy(i => i.Quantity).ToList();
                        }
                        break;
                    }
                case 4:
                    {
                        if (isDesc)
                        {
                            invoiceLineItems = context.InvoiceLineItems.OrderByDescending(i => i.Item_Total).ToList();
                        }
                        else
                        {
                            invoiceLineItems = context.InvoiceLineItems.OrderBy(i => i.Item_Total).ToList();
                        }
                        break;
                    }
                case 0:
                default:
                    {
                        if (isDesc)
                        {
                            invoiceLineItems = context.InvoiceLineItems.OrderByDescending(i => i.Invoice_Id).ToList();
                        }
                        else
                        {
                            invoiceLineItems = context.InvoiceLineItems.OrderBy(i => i.Invoice_Id).ToList();
                        }
                        break;
                    }
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Trim().ToLower();

                invoiceLineItems = invoiceLineItems.Where(i =>
                        i.Invoice_Id.ToString().Contains(id) ||
                        i.Product_Code.ToString().Contains(id) ||
                        i.Unit_Price.ToString().Contains(id) ||
                        i.Quantity.ToString().Contains(id) ||
                        i.Item_Total.ToString().Contains(id)
                    ).ToList();
            }

            invoiceLineItems = invoiceLineItems.Where(i => i.Is_Deleted == false).ToList();

            return View(invoiceLineItems);
        }

        [HttpGet]
        public ActionResult Upsert(int id = 0, string code = "")
        {
            BooksEntities context = new BooksEntities();

            // If no invoiceLineItem in the DB, Return a new instance of InvoiceLineItem Object
            InvoiceLineItem invoiceLineItem = context.InvoiceLineItems.Where(i => i.Invoice_Id == id && i.Product_Code == code).FirstOrDefault() ?? new InvoiceLineItem();
            List<Product> products = context.Products.ToList();
            List<Invoice> invoices = context.Invoices.ToList();

            UpsertInvoiceLineItemModel viewModel = new UpsertInvoiceLineItemModel()
            {
                InvoiceLineItem = invoiceLineItem,
                Product = products,
                Invoice = invoices
            };

            if (invoiceLineItem.Is_Deleted)
            {
                return RedirectToAction("All");
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Upsert(UpsertInvoiceLineItemModel model, string stateCode)
        {

            InvoiceLineItem newInvoiceLineItem = model.InvoiceLineItem;

            stateCode = stateCode.Split('-')[0].Trim();
            //newInvoiceLineItem.State = stateCode;

            BooksEntities context = new BooksEntities();
            try
            {
                if (context.InvoiceLineItems.Where(i => i.Invoice_Id == newInvoiceLineItem.Invoice_Id && i.Product_Code == newInvoiceLineItem.Product_Code).Count() > 0)
                {
                    var invoiceLineItemToSave = context.InvoiceLineItems.Where(i => i.Invoice_Id == newInvoiceLineItem.Invoice_Id && i.Product_Code == newInvoiceLineItem.Product_Code).FirstOrDefault();
                    invoiceLineItemToSave.Invoice_Id = newInvoiceLineItem.Invoice_Id;
                    invoiceLineItemToSave.Product_Code = newInvoiceLineItem.Product_Code;
                    invoiceLineItemToSave.Unit_Price = newInvoiceLineItem.Unit_Price;
                    invoiceLineItemToSave.Quantity = newInvoiceLineItem.Quantity;
                    invoiceLineItemToSave.Item_Total = newInvoiceLineItem.Item_Total;
                }
                else
                {
                    context.InvoiceLineItems.Add(newInvoiceLineItem);
                }
                context.SaveChanges();
            }
            catch (DbEntityValidationException vex)
            {
                ViewBag.Error = $"<span class='error'>{vex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage}</span>";
                return View(newInvoiceLineItem);
            }

            return RedirectToAction("All");
        }

        [HttpGet]
        public ActionResult Delete(int invoiceId, string productCode)
        {
            BooksEntities context = new BooksEntities();

            try
            {
                InvoiceLineItem invoiceLineItem = context.InvoiceLineItems.Where(i => i.Invoice_Id == invoiceId && i.Product_Code == productCode).FirstOrDefault();
                invoiceLineItem.Is_Deleted = true;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Id = invoiceId,
                    ProductCode = productCode,
                    Message = ex.Message
                });
            }

            return Json(new
            {
                Success = true,
                Id = invoiceId,
                ProductCode = productCode,
                returnUrl = "/InvoiceLineItems/All"
            }, JsonRequestBehavior.AllowGet);
        }
    }
    }
}