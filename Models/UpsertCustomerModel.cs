using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Taniguchi_Final_Project.Models
{
    public class UpsertCustomerModel
    {
        public Customer Customer { get; set; }
        public List<State> States { get; set; }
    }
}