using AdunbiKiddies.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Excel = Microsoft.Office.Interop.Excel;

namespace AdunbiKiddies.Controllers
{
    //[Authorize]
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Items
        public ActionResult Index(string category, string sortOrder, string currentFilter, string searchString, string barString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PriceSortParm = sortOrder == "Price" ? "price_desc" : "Price";

            if (searchString != null || barString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var items = from i in db.Products
                        select i;

            if (!String.IsNullOrEmpty(barString))
            {
                items = items.Where(s => s.Barcode.Equals(barString.Trim()));
            }
            else if (!String.IsNullOrEmpty(searchString))
            {
                items = items.Where(s => s.Name.ToUpper().Contains(searchString.ToUpper())
                                       || s.Catagorie.Name.ToUpper().Contains(searchString.ToUpper())
                                       || s.Barcode.Equals(searchString.Trim()));
            }
            else if (!String.IsNullOrEmpty(category))
            {
                items = items.Where(s => s.Catagorie.Name.ToUpper().Contains(category.ToUpper()));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    items = items.OrderByDescending(s => s.Name);
                    break;
                case "Price":
                    items = items.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    items = items.OrderByDescending(s => s.Price);
                    break;
                default:  // Name ascending 
                    items = items.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(items.ToPagedList(pageNumber, pageSize));
            ;


            //var items = db.Items.Include(i => i.Catagorie);
            //return View(await items.ToListAsync());
        }

        public async Task<ActionResult> ItemLeft()
        {
            var items = from i in db.Products
                        select i;

            items = items.Where(s => s.StockQuantity.Value <= 3);


            return View(await items.ToListAsync());



            //var items = db.Items.Include(i => i.Catagorie);
            //return View(await items.ToListAsync());
        }

        public ActionResult UploadProducts()
        {
            //ViewBag.CourseName = new SelectList(db.Courses, "CourseName", "CourseName");
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> UploadProducts(HttpPostedFileBase excelfile)
        {
            if (excelfile == null || excelfile.ContentLength == 0)
            {
                ViewBag.Error = "Please Select a excel file <br/>";
                return View("Index");
            }
            else
            {
                if (excelfile.FileName.EndsWith("xls") || excelfile.FileName.EndsWith("xlsx"))
                {
                    string path = Server.MapPath("~/Content/ExcelUploadedFile/" + excelfile.FileName);
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                    excelfile.SaveAs(path);

                    // Read data from excel file
                    Excel.Application application = new Excel.Application();
                    Excel.Workbook workbook = application.Workbooks.Open(path);
                    Excel.Worksheet worksheet = workbook.ActiveSheet;
                    Excel.Range range = worksheet.UsedRange;

                    List<Product> listSavingsMaintenance = new List<Product>();

                    for (int row = 2; row <= range.Rows.Count; row++)
                    {
                        var myProduct = new Product
                        {
                            CategoriesId = ((Excel.Range)range.Cells[row, 1]).Text,
                            Name = double.Parse(((Excel.Range)range.Cells[row, 2]).Text),
                            Barcode = ((Excel.Range)range.Cells[row, 3]).Text,
                            Price = double.Parse(((Excel.Range)range.Cells[row, 4]).Text),
                            ItemPictureUrl = double.Parse(((Excel.Range)range.Cells[row, 5]).Text),

                        };
                        db.Products.Add(myProduct);
                        await db.SaveChangesAsync();
                        //listSavingsMaintenance.Add(mySavingMaintenance);
                    }
                    workbook.Close(0);
                    application.Quit();
                    ViewBag.Message = "Success";
                    return View();
                }
                else
                {
                    ViewBag.Error = "File type is Incorrect <br/>";
                    return View("Index");
                }
            }
        }

        public PartialViewResult Menu()
        {
            IEnumerable<string> categories = db.Categories.Select(s => s.Name)
                                                            .OrderBy(s => s);
            return PartialView(categories);
        }
        // GET: Items/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product item = await db.Products.FindAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }


        public async Task<ActionResult> PrintBarCode(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product item = await db.Products.FindAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }
        // GET: Items/Create
        //[Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.CategoriesId = new SelectList(db.Categories, "ID", "Name").ToList();
            return View();
        }

        // POST: Items/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(Product product)
        {
            string name = product.Name.Replace(" ", "");
            string cat = product.CategoriesId.ToString();
            string price = product.Price.ToString();
            string GeneratedBarcode = "Ad" + name + cat + price;


            Bitmap bitmap = new Bitmap(GeneratedBarcode.Length * 40, 150);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                Font gFont = new System.Drawing.Font("IDAutomationHC39M", 20);

                PointF point = new PointF(2f, 2f);
                SolidBrush black = new SolidBrush(Color.Black);
                SolidBrush white = new SolidBrush(Color.White);
                graphics.FillRectangle(white, 0, 0, bitmap.Width, bitmap.Height);
                graphics.DrawString(GeneratedBarcode, gFont, black, point);

            }

            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                //picture_bar.Image = bitmap;
                //picture_bar.Height = bitmap.Height;
                //picture_bar.Width = bitmap.Width;
                Product myProduct = new Product()
                {
                    CategoriesId = product.CategoriesId,
                    Name = product.Name,
                    Price = product.Price,
                    InternalImage = product.InternalImage,
                    ItemPictureUrl = product.ItemPictureUrl,
                    Barcode = GeneratedBarcode,
                    BarcodeImage = ms.ToArray()
                };
                db.Products.Add(myProduct);
                await db.SaveChangesAsync();

            }

            ViewBag.CategoriesId = new SelectList(db.Categories, "ID", "Name", product.CategoriesId);
            return RedirectToAction("Index");

        }

        // GET: Items/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product item = await db.Products.FindAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoriesId = new SelectList(db.Categories, "ID", "Name", item.CategoriesId);
            return View(item);
        }

        // POST: Items/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(Product product)
        {
            string name = product.Name.Replace(" ", "");
            string cat = product.CategoriesId.ToString();
            string price = product.Price.ToString();
            string GeneratedBarcode = "Ad" + name + cat + price;

            if (ModelState.IsValid)
            {
                Bitmap bitmap = new Bitmap(GeneratedBarcode.Length * 40, 150);

                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    Font gFont = new System.Drawing.Font("IDAutomationHC39M", 20);

                    PointF point = new PointF(2f, 2f);
                    SolidBrush black = new SolidBrush(Color.Black);
                    SolidBrush white = new SolidBrush(Color.White);
                    graphics.FillRectangle(white, 0, 0, bitmap.Width, bitmap.Height);
                    graphics.DrawString(GeneratedBarcode, gFont, black, point);

                }

                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    //picture_bar.Image = bitmap;
                    //picture_bar.Height = bitmap.Height;
                    //picture_bar.Width = bitmap.Width;
                    Product myProduct = await db.Products.FindAsync(product.ID);

                    myProduct.CategoriesId = product.CategoriesId;
                    myProduct.Name = product.Name;
                    myProduct.Price = product.Price;
                    myProduct.InternalImage = product.InternalImage;
                    myProduct.ItemPictureUrl = product.ItemPictureUrl;
                    myProduct.Barcode = GeneratedBarcode;
                    myProduct.BarcodeImage = ms.ToArray();

                    //db.Products.Add(myProduct);
                    await db.SaveChangesAsync();

                    //db.Entry(product).State = EntityState.Modified;
                    //await db.SaveChangesAsync();
                    //return RedirectToAction("Index");
                }
                ViewBag.CategoriesId = new SelectList(db.Categories, "ID", "Name", product.CategoriesId);
            }
            return RedirectToAction("Index");
        }

        // GET: Items/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product item = await db.Products.FindAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Product item = await db.Products.FindAsync(id);
            db.Products.Remove(item);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> RenderImage(int id)
        {
            Product item = await db.Products.FindAsync(id);

            byte[] photoBack = item.InternalImage;

            return File(photoBack, "image/png");
        }

        public async Task<ActionResult> RenderBarcode(int id)
        {
            Product item = await db.Products.FindAsync(id);

            byte[] photoBack = item.BarcodeImage;

            return File(photoBack, "image/png");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
