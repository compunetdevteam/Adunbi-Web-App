using System.ComponentModel.DataAnnotations;

namespace AdunbiKiddies.Models
{
    public class Deposit
    {
        public int ID { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }



        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Amount Paid")]
        public decimal PaidFee { get; set; }

        [Display(Name = "Remaining Fee")]
        public decimal Remaining
        {
            get
            {
                return TotalAmount - PaidFee;
            }
        }

        public virtual Product Product { get; set; }
        public virtual Sale Sale { get; set; }
    }
}