using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AdunbiKiddies.Models
{
    [Bind(Exclude = "OrderId")]
    public partial class Sale
    {
        public int SaleId { get; set; }

        public DateTime SaleDate { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [DisplayName("First Name")]
        [StringLength(160)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [DisplayName("Last Name")]
        [StringLength(160)]
        public string LastName { get; set; }

        public string SalesRepName { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [StringLength(24)]
        public string Phone { get; set; }


        public decimal Total { get; set; }
        public List<SaleDetail> SaleDetails { get; set; }


    }
}