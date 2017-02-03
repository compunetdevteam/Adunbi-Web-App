using AdunbiKiddies.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AdunbiKiddies.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StocksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Stocks
        public async Task<ActionResult> Index(string val1, string val2)
        {
            ViewBag.Message1 = val1;
            ViewBag.Message2 = val2;
            return View(await db.Stocks.ToListAsync());
        }

        // GET: Stocks/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stock stock = await db.Stocks.FindAsync(id);
            if (stock == null)
            {
                return HttpNotFound();
            }
            return View(stock);
        }

        // GET: Stocks/Create
        public ActionResult Create()
        {
            ViewBag.Name = new SelectList(db.Products, "ID", "Name");
            ViewBag.StaffName = User.Identity.GetUserName().ToString();
            return View();
        }

        // POST: Stocks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Name,Quantity,Date,StaffName,Status")] Stock stock)
        {
            string messag1 = "";
            string messag2 = "";
            if (ModelState.IsValid)
            {
                db.Stocks.Add(stock);
                //User myUser = myDBContext.Users.SingleOrDefault(user => user.Username == username);
                //var user = db.Users.Where(c => c.Email.Equals(model.Email)).SingleOrDefault();
                //Product product = await db.Products.FindAsync(int.Parse(stock.Name));
                Product product = await db.Products.SingleOrDefaultAsync(s => s.Barcode.Equals(stock.Name));
                if (product == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    int incomingValue = stock.Quantity;

                    if (product.StockQuantity == null)
                    {
                        product.StockQuantity = 0;
                    }

                    if (stock.Status.Equals(PopUp.Status.Add))
                    {
                        product.StockQuantity += incomingValue;
                    }
                    else if (stock.Status.Equals(PopUp.Status.Remove))
                    {
                        product.StockQuantity -= incomingValue;

                    }


                }
                if (product.StockQuantity < 0)
                {
                    messag1 = "Sorry stock Removal is UNSUCCESSFUL";
                    messag2 = "You should have more item to subtract from";
                    return RedirectToAction("Index", new { val1 = messag1, val2 = messag2 });
                    //return RedirectToAction("Details", "Consultants", new { id = pescription.ConsultantID });
                }
                else
                {
                    messag1 = "Successfully Updated";
                    db.Entry(product).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", new { val1 = messag1 });
                }


            }

            return View(stock);
        }


        //// GET: Stocks/Edit/5
        //public async Task<ActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Stock stock = await db.Stocks.FindAsync(id);
        //    if (stock == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(stock);
        //}

        //// POST: Stocks/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "ID,Name,Quantity,Date,StaffName")] Stock stock)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(stock).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    return View(stock);
        //}

        //// GET: Stocks/Delete/5
        //public async Task<ActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Stock stock = await db.Stocks.FindAsync(id);
        //    if (stock == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(stock);
        //}

        //// POST: Stocks/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{
        //    Stock stock = await db.Stocks.FindAsync(id);
        //    db.Stocks.Remove(stock);
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

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
