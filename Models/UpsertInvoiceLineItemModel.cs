using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Taniguchi_Final_Project.Models
{
    public class UpsertInvoiceLineItemModel
    {
        public InvoiceLineItem InvoiceLineItem { get; set; }
        public List<Invoice> Invoice { get; set; }
        public List<Product> Product { get; set; }
    }
}