using AdunbiKiddies.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AdunbiKiddies.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        ApplicationDbContext storeDB = new ApplicationDbContext();
        //AppConfigurations appConfig = new AppConfigurations();

        //public List<String> CreditCardTypes { get { return appConfig.CreditCardType; } }


        public ActionResult Index()
        {
            return View();
        }


        //GET: /Checkout/AddressAndPayment
        public ActionResult AddressAndPayment()
        {
            ViewBag.SalesRepName = User.Identity.GetUserName().ToString();
            return View();
        }

        //
        // POST: /Checkout/AddressAndPayment
        [HttpPost]
        public async Task<ActionResult> AddressAndPayment(FormCollection values)
        {
            //ViewBag.CreditCardTypes = CreditCardTypes;
            // string result = values[9];

            var sale = new Sale();
            TryUpdateModel(sale);
            //order.CreditCard = result;

            try
            {
                sale.SalesRepName = User.Identity.Name;
                sale.SaleDate = DateTime.Now;
                var currentUserId = User.Identity.GetUserId();



                //Save Order
                storeDB.Sales.Add(sale);
                await storeDB.SaveChangesAsync();
                //Process the order
                var cart = ShoppingCart.GetCart(this.HttpContext);
                sale = cart.CreateOrder(sale);




                //ModelState.Clear();
                //ViewBag.Message = "Thank you for Contacting us ";



                //CheckoutController.SendOrderMessage(sale.FirstName, "New Order: " + sale.SaleId, sale.ToString(sale));

                return RedirectToAction("Complete",
                        new { id = sale.SaleId });

            }
            catch
            {
                //Invalid - redisplay with errors
                return View(sale);
            }
        }

        //
        // GET: /Checkout/Complete
        public ActionResult Complete(int id)
        {
            // Validate customer owns this order
            bool isValid = storeDB.Sales.Any(
                o => o.SaleId == id &&
                o.SalesRepName == User.Identity.Name);

            if (isValid)
            {
                return RedirectToAction("PrintDetails",
                      new { id = id });
            }
            else
            {
                return View("Error");
            }
        }

        public async Task<ActionResult> PrintDetails(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sale sales = await storeDB.Sales.FindAsync(id);
            var saleDetails = storeDB.SaleDetails.Where(x => x.SaleId == id);

            sales.SaleDetails = await saleDetails.ToListAsync();
            if (sales == null)
            {
                return HttpNotFound();
            }
            return View(sales);
        }


        //private static RestResponse SendOrderMessage(String toName, String subject, String body, String destination)
        //{
        //    RestClient client = new RestClient();
        //    //fix this we have this up top too
        //    AppConfigurations appConfig = new AppConfigurations();
        //    client.BaseUrl = "https://api.mailgun.net/v2";
        //    client.Authenticator =
        //           new HttpBasicAuthenticator("api",
        //                                      appConfig.EmailApiKey);
        //    RestRequest request = new RestRequest();
        //    request.AddParameter("domain",
        //                        appConfig.DomainForApiKey, ParameterType.UrlSegment);
        //    request.Resource = "{domain}/messages";
        //    request.AddParameter("from", appConfig.FromName + " <" + appConfig.FromEmail + ">");
        //    request.AddParameter("to", toName + " <" + destination + ">");
        //    request.AddParameter("subject", subject);
        //    request.AddParameter("html", body);
        //    request.Method = Method.POST;
        //    IRestResponse executor = client.Execute(request);
        //    return executor as RestResponse;
        //}
    }
}