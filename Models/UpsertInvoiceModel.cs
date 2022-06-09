using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Taniguchi_Final_Project.Models
{
    public class UpsertInvoiceModel
    {
        public Invoice Invoice { get; set; }
        public List<Customer> Customer { get; set; }
    }
}