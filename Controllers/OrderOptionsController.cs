//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using Taniguchi_Final_Project.Models;

//namespace Taniguchi_Final_Project.Controllers
//{
//    public class OrderOptionsController : Controller
//    {
//        // GET: OrderOptions
//        public ActionResult All(string id, int sortBy = 0, bool isDesc = false)
//        {
//            BooksEntities context = new BooksEntities();
//            List<OrderOption> orderOptions;

//            switch (sortBy)
//            {
//                case 1:
//                    {
//                        if (isDesc)
//                        {
//                            orderOptions = context.OrderOptions.OrderByDescending(oo => oo.Sales_Tax_Rate).ToList();
//                        }
//                        else
//                        {
//                            orderOptions = context.OrderOptions.OrderBy(oo => oo.Sales_Tax_Rate).ToList();
//                        }
//                        break;
//                    }
//                case 2:
//                    {
//                        if (isDesc)
//                        {
//                            orderOptions = context.OrderOptions.OrderByDescending(oo => oo.First_Book_Ship_Charge).ToList();
//                        }
//                        else
//                        {
//                            orderOptions = context.OrderOptions.OrderBy(oo => oo.First_Book_Ship_Charge).ToList();
//                        }
//                        break;
//                    }
//                case 3:
//                    {
//                        if (isDesc)
//                        {
//                            orderOptions = context.OrderOptions.OrderByDescending(oo => oo.Additional_Book_Ship_Charge).ToList();
//                        }
//                        else
//                        {
//                            orderOptions = context.OrderOptions.OrderBy(oo => oo.Additional_Book_Ship_Charge).ToList();
//                        }
//                        break;
//                    }
//                case 0:
//                default:
//                    {
//                        if (isDesc)
//                        {
//                            orderOptions = context.OrderOptions.OrderByDescending(oo => oo.Id).ToList();
//                        }
//                        else
//                        {
//                            orderOptions = context.OrderOptions.OrderBy(oo => oo.Id).ToList();
//                        }
//                        break;
//                    }
//            }

//            if (!string.IsNullOrWhiteSpace(id))
//            {
//                id = id.Trim().ToLower();

//                orderOptions = orderOptions.Where(oo =>
//                        oo.Sales_Tax_Rate.ToString().Contains(id) ||
//                        oo.First_Book_Ship_Charge.ToString().Contains(id) ||
//                        oo.Additional_Book_Ship_Charge.ToString().Contains(id)
//                    ).ToList();
//            }

//            orderOptions = orderOptions.Where(oo => oo.Is_Deleted == false).ToList();

//            return View(orderOptions);
//        }

//        [HttpGet]
//        public ActionResult Upsert(int id = 0)
//        {
//            BooksEntities context = new BooksEntities();

//            // If no invoice in the DB, Return a new instance of OrderOption Object
//            OrderOption invoice = context.OrderOptions.Where(oo => oo.Id == id).FirstOrDefault() ?? new OrderOption();
//            List<Customer> customers = context.Customers.ToList();

//            UpsertInvoiceModel viewModel = new UpsertInvoiceModel()
//            {
//                OrderOption = invoice,
//                Customer = customers
//            };

//            if (invoice.Is_Deleted)
//            {
//                return RedirectToAction("All");
//            }
//            return View(viewModel);
//        }

//        [HttpPost]
//        public ActionResult Upsert(UpsertInvoiceModel model, string customerId)
//        {
//            OrderOption newInvoice = model.OrderOption;

//            customerId = customerId.Split('-')[0];
//            newInvoice.Customer_Id = Convert.ToInt32(customerId);

//            BooksEntities context = new BooksEntities();
//            try
//            {

//                //TODO decide if need to keep or implment elsewhere
//                //if (context.OrderOptions.Where(oo => oo.Email == newInvoice.Email && oo.Id != newInvoice.Id).Count() > 0)
//                //{
//                //    // TODO Add message to user
//                //    return RedirectToAction("All");
//                //}

//                if (context.OrderOptions.Where(oo => oo.Id == newInvoice.Id).Count() > 0)
//                {
//                    var invoiceToSave = context.OrderOptions.Where(oo => oo.Id == newInvoice.Id).FirstOrDefault();
//                    invoiceToSave.Customer_Id = newInvoice.Customer_Id;
//                    invoiceToSave.Date = newInvoice.Date;
//                    invoiceToSave.Product_Total = newInvoice.Product_Total;
//                    invoiceToSave.Sales_Tax = newInvoice.Sales_Tax;
//                    invoiceToSave.Shipping = newInvoice.Shipping;
//                    invoiceToSave.Total = newInvoice.Total;
//                }
//                else
//                {
//                    context.OrderOptions.Add(newInvoice);
//                }
//                context.SaveChanges();
//            }
//            catch (DbEntityValidationException vex)
//            {
//                ViewBag.Error = $"<span class='error'>{vex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage}</span>";
//                return View(newInvoice);
//            }
//            catch (Exception ex)
//            {
//                ViewBag.Error = ex.Message;
//                return View(newInvoice);
//            }
//            return RedirectToAction("All");
//        }

//        [HttpGet]
//        public ActionResult Delete(string id)
//        {
//            BooksEntities context = new BooksEntities();
//            int invoiceId = 0;

//            if (int.TryParse(id, out invoiceId))
//            {
//                try
//                {
//                    OrderOption invoice = context.OrderOptions.Where(oo => oo.Id == invoiceId).FirstOrDefault();
//                    invoice.Is_Deleted = true;
//                    context.SaveChanges();
//                }
//                catch (Exception ex)
//                {
//                    return Json(new
//                    {
//                        Success = false,
//                        Id = id,
//                        Message = ex.Message
//                    });
//                }
//            }
//            else
//            {
//                //Parsing not successful
//            }
//            return Json(new
//            {
//                Success = true,
//                Id = id,
//                returnUrl = "/OrderOptions/All"
//            }, JsonRequestBehavior.AllowGet);
//        }
//    }
//}