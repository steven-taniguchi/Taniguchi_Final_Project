using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Taniguchi_Final_Project.Models;

namespace Taniguchi_Final_Project.Controllers
{
    public class CustomersController : Controller
    {
        // GET: Customers
        /// <summary>
        /// Displays the view for all customers
        /// </summary>
        /// <param name="id">The search term</param>
        /// <param name="sortBy">0 = Id, 1 = Name, 2 = Address, 3 = City, 4 = State, 5 =  Zip code</param>
        /// <param name="isDesc">Set ascending or descending on header click</param>
        /// <returns>Customer view</returns>
        public ActionResult All(string id, int sortBy = 0, bool isDesc = false)
        {
            BooksEntities context = new BooksEntities();
            List<Customer> customers;

            switch (sortBy)
            {
                case 1:
                    {
                        if (isDesc)
                        {
                            customers = context.Customers.OrderByDescending(c => c.Name).ToList();
                        }
                        else
                        {
                            customers = context.Customers.OrderBy(c => c.Name).ToList();
                        }
                        break;
                    }
                case 2:
                    {
                        if (isDesc)
                        {
                            customers = context.Customers.OrderByDescending(c => c.Address).ToList();
                        }
                        else
                        {
                            customers = context.Customers.OrderBy(c => c.Address).ToList();
                        }
                        break;
                    }
                case 3:
                    {
                        if (isDesc)
                        {
                            customers = context.Customers.OrderByDescending(c => c.City).ToList();
                        }
                        else
                        {
                            customers = context.Customers.OrderBy(c => c.City).ToList();
                        }
                        break;
                    }
                case 4:
                    {
                        if (isDesc)
                        {
                            customers = context.Customers.OrderByDescending(c => c.State).ToList();
                        }
                        else
                        {
                            customers = context.Customers.OrderBy(c => c.State).ToList();
                        }
                        break;
                    }
                case 5:
                    {
                        if (isDesc)
                        {
                            customers = context.Customers.OrderByDescending(c => c.Zip_Code).ToList();
                        }
                        else
                        {
                            customers = context.Customers.OrderBy(c => c.Zip_Code).ToList();
                        }
                        break;
                    }
                case 0:
                default:
                    {
                        if (isDesc)
                        {
                            customers = context.Customers.OrderByDescending(c => c.Id).ToList();
                        }
                        else
                        {
                            customers = context.Customers.OrderBy(c => c.Id).ToList();
                        }
                        break;
                    }
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Trim().ToLower();

                customers = customers.Where(c =>
                        c.Id.ToString().Contains(id) ||
                        c.Name.ToLower().Contains(id) ||
                        c.Address.ToLower().Contains(id) ||
                        c.City.ToLower().Contains(id) ||
                        c.State.ToLower().Contains(id) ||
                        c.Zip_Code.ToLower().Contains(id)
                    ).ToList();
            }

            customers = customers.Where(c => c.Is_Deleted == false).ToList();

            return View(customers);
        }

        /// <summary>
        /// Allows for edit of entry
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <returns>Customer view</returns>
        [HttpGet]
        public ActionResult Upsert(int id = 0)
        {
            BooksEntities context = new BooksEntities();

            // If no customer in the DB, Return a new instance of Customer Object
            Customer customer = context.Customers.Where(c => c.Id == id).FirstOrDefault() ?? new Customer();

            if (customer.Is_Deleted)
            {
                return RedirectToAction("All");
            }
            return View(customer);
        }

        /// <summary>
        /// Upserts a customer
        /// </summary>
        /// <param name="newCustomer">New customer to add to database</param>
        /// <returns>All view</returns>
        [HttpPost]
        public ActionResult Upsert(Customer newCustomer)
        {
            BooksEntities context = new BooksEntities();
            try
            {
                if (context.Customers.Where(c => c.Id == newCustomer.Id).Count() > 0)
                {
                    var customerToSave = context.Customers.Where(c => c.Id == newCustomer.Id).FirstOrDefault();
                    customerToSave.Id = newCustomer.Id;
                    customerToSave.Name = newCustomer.Name;
                    customerToSave.Address = newCustomer.Address;
                    customerToSave.City = newCustomer.City;
                    customerToSave.State = newCustomer.State;
                    customerToSave.Zip_Code = newCustomer.Zip_Code;
                }
                else
                {
                    context.Customers.Add(newCustomer);
                }
                context.SaveChanges();
            }
            catch (DbEntityValidationException vex)
            {
                ViewBag.Error = $"<span class='error'>{vex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage}</span>";
                return View(newCustomer);
            }
           
            return RedirectToAction("All");
        }

        /// <summary>
        /// Deletes a customer
        /// </summary>
        /// <param name="id">Customer ID to delete</param>
        /// <returns>Returns JSON object redirecting to all customer view</returns>
        [HttpGet]
        public ActionResult Delete(string id)
        {
            BooksEntities context = new BooksEntities();
            int customerId = 0;

            if (int.TryParse(id, out customerId))
            {
                try
                {
                    Customer customer = context.Customers.Where(c => c.Id == customerId).FirstOrDefault();
                    customer.Is_Deleted = true;
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
                Success = true, Id = id, returnUrl = "/Customers/All" }, JsonRequestBehavior.AllowGet);
        }
    }
}