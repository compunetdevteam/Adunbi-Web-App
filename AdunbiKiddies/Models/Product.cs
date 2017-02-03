using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Web;

namespace AdunbiKiddies.Models
{
    //[Bind(Exclude = "ID")]
    public class Product
    {
        //private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();


        [Key]
        [ScaffoldColumn(false)]
        public int ID { get; set; }

        [DisplayName("Categories")]
        public int CategoriesId { get; set; }

        //[Required(ErrorMessage = "BarCode Input is required")]
        //public string BarcodeInput { get; set; }

        [Required(ErrorMessage = "An Item Name is required")]
        [StringLength(160)]
        public string Name { get; set; }



        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 999999.99, ErrorMessage = "Price must be between 0.01 and 999999.99")]
        public decimal Price { get; set; }

        public byte[] InternalImage { get; set; }

        [Display(Name = "Local file")]
        [NotMapped]
        public HttpPostedFileBase File
        {
            get
            {
                return null;
            }

            set
            {
                try
                {
                    MemoryStream target = new MemoryStream();

                    if (value.InputStream == null)
                        return;

                    value.InputStream.CopyTo(target);
                    InternalImage = target.ToArray();
                }
                catch (Exception ex)
                {
                    //logger.Error(ex.Message);
                    //logger.Error(ex.StackTrace);

                }
            }
        }

        [DisplayName("Item Picture URL")]
        [StringLength(1024)]
        public string ItemPictureUrl { get; set; }

        public int? StockQuantity { get; set; }

        public byte[] BarcodeImage { get; set; }
        //public string Username
        //{
        //    get
        //    {
        //        return string.Format("{0} {1}", this.FirstName, this.LastName);
        //    }
        //}

        public string Barcode { get; set; }

        //public string ImageUrl { get; set; }

        public virtual Categories Catagorie { get; set; }
        public virtual List<SaleDetail> OrderDetails { get; set; }
        public virtual Stock Stock { get; set; }
        public virtual ICollection<Supplier> Suppliers { get; set; }
    }
}