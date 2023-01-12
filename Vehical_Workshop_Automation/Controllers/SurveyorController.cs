using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Vehical_Workshop_Automation.Models;

namespace Vehical_Workshop_Automation.Controllers
{
    public class SurveyorController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddSurveyor(Surveyor su)
        {
            return View(su);
        }
        public ActionResult SurveyorDashboard()
        {
            string mobileno = Session["Surveyor_MobileNo"].ToString();
            IList<Vehicle_Customer1> customerVehicles = null;
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
            var responseTask = client.GetAsync("CustomersVehicles/?mobileno=" + "03408149083");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<Vehicle_Customer1>>();
                readTask.Wait();
                customerVehicles = readTask.Result;
            }
            return View(customerVehicles);
        }
        public ActionResult Suggestions(int Appointment_No)
        {
            var existList = GetAppService(Appointment_No);
            Session["AppNo"] = Appointment_No;
            IList<Services> services = null;
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
            ServiceList sl = new ServiceList();
            sl.Services = services.ToList();
            if (existList.Count() > 0) 
            {
                foreach (var item in sl.Services)
                {
                    var find=existList.FirstOrDefault(e => e.Customer_Suggestion == item.Name);
                    if (!(find == null))
                    {
                        item.Selected = true;

                    }
                }

            }
            return View(sl);
        }
        public ActionResult SurveyorList()
        {
            IList<Surveyor> lst = new List<Surveyor>();
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
            var responseTask = client.GetAsync("AllSurveyor/");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<Surveyor>>();
                readTask.Wait();
                lst = readTask.Result;
                return View(lst);
            }
            else
            {
                return View(lst);
            }
        }
        public ActionResult DeleteSurveyor(int id)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
            var postTask = client.DeleteAsync("DeleteSurveyor/" + id);
            postTask.Wait();
            var result = postTask.Result;
            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction("SurveyorList");
            }
            return RedirectToAction("SurveyorList", "Surveyor");
        }
        [HttpPost]
        public ActionResult SaveSurveyor(Surveyor sy)
        {
            if (sy.Id == 0)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
                    var postTask = client.PostAsJsonAsync<Surveyor>("AddSurveyor", sy);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("SurveyorList");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Something went wrong");
                        ViewBag.msg = "Surveyor not added.";
                        return View();
                    }
                }
            }
            else
            {

                var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
                var postTask = client.PostAsJsonAsync("EditSurveyor",sy);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("SurveyorList");
                }
                return RedirectToAction("SurveyorList");

            }

        }
        public ActionResult GetCheckedAppointment()
        {
            IList<Appointment> appointments = null;
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
            var responseTask = client.GetAsync("GetSurveyorAppointment");
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
        [HttpPost]
        public ActionResult SaveSuggestion(ServiceList lst)
        {
            var existList = GetAppService(int.Parse(Session["AppNo"].ToString()));
            List<App_Service> app_Services = new List<App_Service>();
            foreach (var item in lst.Services)
            {
                if (item.Selected)
                {
                    var find = existList.FirstOrDefault(e => e.Customer_Suggestion == item.Name);
                    if (find == null)
                    {
                        app_Services.Add(new App_Service { App_No = int.Parse(Session["AppNo"].ToString()), Surveyor_Suggestion = item.Name });
                    }
                }
            }
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
            var responseTask = client.PostAsJsonAsync<IList<App_Service>>("AddAppointmentService", app_Services);
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction("GetCheckedAppointment", "Surveyor");
            }
            else
            {
                ViewBag.Error = "Appointment not saved somthing wrong";
                return RedirectToAction("GetCheckedAppointment", "Surveyor");
            }
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
            var filter = services.Where(s=>s.App_No== Appointment_No).ToList();
            return filter;
        }
    }
}