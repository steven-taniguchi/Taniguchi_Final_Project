using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Taniguchi_Final_Project.Models;

namespace Taniguchi_Final_Project.Controllers
{
    public class StatesController : Controller
    {
        // GET: States
        public ActionResult All(string id, int sortBy = 0, bool isDesc = false)
        {
            BooksEntities context = new BooksEntities();
            List<State> states;

            switch (sortBy)
            {
                case 1:
                    {
                        if (isDesc)
                        {
                            states = context.States.OrderByDescending(s => s.Name).ToList();
                        }
                        else
                        {
                            states = context.States.OrderBy(s => s.Name).ToList();
                        }
                        break;
                    }
                case 0:
                default:
                    {
                        if (isDesc)
                        {
                            states = context.States.OrderByDescending(s => s.Code).ToList();
                        }
                        else
                        {
                            states = context.States.OrderBy(s => s.Code).ToList();
                        }
                        break;
                    }
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Trim().ToLower();

                states = states.Where(s =>
                        s.Code.ToLower().Contains(id) ||
                        s.Name.ToLower().Contains(id)).ToList();
            }

            states = states.Where(s => s.Is_Deleted == false).ToList();

            return View(states);
        }

        [HttpGet]
        public ActionResult Upsert(string code = "")
        {
            BooksEntities context = new BooksEntities();

            // If no state in the DB, Return a new instance of Customer Object
            State state = context.States.Where(s => s.Code == code).FirstOrDefault() ?? new State();

            if (state.Is_Deleted)
            {
                return RedirectToAction("All");
            }
            return View(state);
        }

        [HttpPost]
        public ActionResult Upsert(State newState)
        {
            BooksEntities context = new BooksEntities();
            try
            {

                //TODO decide if need to keep or implment elsewhere
                //if (context.Customers.Where(c => c.Email == newCustomer.Email && c.Id != newCustomer.Id).Count() > 0)
                //{
                //    // TODO Add message to user
                //    return RedirectToAction("All");
                //}

                if (context.States.Where(s => s.Code == newState.Code).Count() > 0)
                {
                    var stateToSave = context.States.Where(s => s.Code == newState.Code).FirstOrDefault();
                    stateToSave.Code = newState.Code;
                    stateToSave.Name = newState.Name;
                }
                else
                {
                    context.States.Add(newState);
                }
                context.SaveChanges();
            }
            catch (DbEntityValidationException vex)
            {
                ViewBag.Error = $"<span class='error'>{vex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage}</span>";
                return View(newState);
            }
           
            return RedirectToAction("All");
        }

        [HttpGet]
        public ActionResult Delete(string code)
        {
            BooksEntities context = new BooksEntities();
            try
            {
                State state = context.States.Where(s => s.Code == code).FirstOrDefault();
                state.Is_Deleted = true;
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
                returnUrl = "/States/All"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}