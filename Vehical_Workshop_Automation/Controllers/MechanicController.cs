using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Vehical_Workshop_Automation.Models;
namespace Vehical_Workshop_Automation.Controllers
{
    public class MechanicController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CheckUp(int Appointment_No)
        {
            Session["MApp"] = Appointment_No;
            var lst = GetAppService(Appointment_No);
            IList<Services> services = null;
            IList<Services> services2 = new List<Services>();
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
            var responseTask = client.GetAsync("GetServices");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<Services>>();
                readTask.Wait();
                services = readTask.Result;
            }
            foreach (var item in services)
            {
                if (lst.FirstOrDefault(a => a.Customer_Suggestion == item.Name) == null)
                {
                    continue;
                }
                else
                {
                    services2.Add(item);
                }
            }
            return View(services2);
        }
        public ActionResult AddMechanic(Mechanic mc)
        {
            return View(mc);
        }
        public ActionResult MechanicList()
        {
            IList<Mechanic> lst = new List<Mechanic>();
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
            var responseTask = client.GetAsync("AllMechanic/");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<Mechanic>>();
                readTask.Wait();
                lst = readTask.Result;
                return View(lst);
            }
            else 
            {
                return View(lst);
            }
        }
        public ActionResult DeleteMechanic(int id)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
            var postTask = client.DeleteAsync("DeleteMechanic/" + id);
            postTask.Wait();
            var result = postTask.Result;
            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction("MechanicList");
            }
            return RedirectToAction("MechanicList", "Mechanic");
        }
        [HttpPost]
        public ActionResult SaveMechanic(Mechanic mc)
        {
            if (mc.Id==0)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
                    var postTask = client.PostAsJsonAsync<Mechanic>("AddMechanic", mc);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("MechanicList");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Something went wrong");
                        ViewBag.msg = "Mechanic not added.";
                        return View();
                    }
                }

            }
            else
            {

                var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
                var postTask = client.PostAsJsonAsync("EditMechanic",mc);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("MechanicList");
                }
                return RedirectToAction("MechanicList");

            }

        }
        public ActionResult GetMechanicAppointment()
        {
            IList<Appointment> appointments = null;
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
            var responseTask = client.GetAsync("GetMechanicAppointment");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<Appointment>>();
                readTask.Wait();
                appointments = readTask.Result;
            }
            return View(appointments);

        }
        private IList<App_Service> GetAppService(int Appointment_No)
        {
            IList<App_Service> services = null;
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
            var responseTask = client.GetAsync("AllAppService");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<App_Service>>();
                readTask.Wait();
                services = readTask.Result;
            }
            var filter = services.Where(s => s.App_No == Appointment_No && s.Status=="Yes").ToList();
            return filter;
        }
        public ActionResult SetTotalBill(int amount)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
            var postTask = client.GetAsync($"SetTotalBill/?apno={int.Parse(Session["MApp"].ToString())}&amount={amount}");
            postTask.Wait();
            var result = postTask.Result;
            if (result.IsSuccessStatusCode)
            {
                if (SetMechanicChecked())
                {
                    ViewBag.ispost = "Thanks for visit Checkup finsih successfully";
                    return RedirectToAction("GetMechanicAppointment");
                }
                else 
                {
                    ViewBag.ispost = "Some thing wrong Checkup not save";
                    return RedirectToAction("GetMechanicAppointment");
                }
            }
            return RedirectToAction("GetMechanicAppointment", "Mechanic");
        }
        private bool SetMechanicChecked()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
            var responseTask = client.GetAsync($"MechanicChecked/?apno={Session["MApp"].ToString()}");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
