using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using Vehical_Workshop_Automation.Models;

namespace Vehical_Workshop_Automation.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GenerateReport(DateTime? date)
        {
            IList<Appointment> appointments = null;
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
            var responseTask = client.GetAsync("AllAppointments");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<Appointment>>();
                readTask.Wait();
                appointments = readTask.Result;
                if (date != null)
                {
                    appointments = appointments.Where(a => a.MCheck == "Yes" && DateTime.Parse(a.Date) == date).ToList();
                }
                else 
                {
                    appointments = appointments.Where(a => a.MCheck == "Yes").ToList();
                }
                
            }
            return View(appointments);
        }
        public ActionResult AdminList()
        {
            IList<Admin> lst = new List<Admin>();
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
            var responseTask = client.GetAsync("AllAdmin/");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<Admin>>();
                readTask.Wait();
                lst = readTask.Result;
                return View(lst);
            }
            else
            {
                return View(lst);
            }
        }
        public ActionResult AddAdmin(Admin ada) 
        {
            return View(ada);
        }
        [HttpPost]
        public ActionResult SaveAdmin(Admin ada)
        {
            if (ada.Id == 0)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
                    var postTask = client.PostAsJsonAsync<Admin>("AddAdmin", ada);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("AdminList");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Something went wrong");
                        ViewBag.msg = "Admin not added.";
                        return View();
                    }
                }
            }
            else
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
                var postTask = client.PostAsJsonAsync<Admin>("EditAdmin", ada);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("AdminList");
                }
                return RedirectToAction("AdminList");
            }
        }
        public ActionResult DeleteAdmin(int id) 
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
            var postTask = client.DeleteAsync("DeleteAdmin/" + id);
            postTask.Wait();
            var result = postTask.Result;
            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction("AdminList");
            }
            return RedirectToAction("AdminList","Admin");
        }
        public ActionResult AllComingAppointments()
        {
            IList<Appointment> appointments = null;
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
            var responseTask = client.GetAsync("AllAppointments");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<Appointment>>();
                readTask.Wait();
                appointments = readTask.Result;
                var today = DateTime.Today.ToShortDateString();
                appointments = appointments.Where(a => DateTime.Parse(a.Date) > DateTime.Parse(today)).ToList();
            }

            return View(appointments);
        }
        public ActionResult AllPreviousAppointments()
        {
            IList<Appointment> appointments = null;
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
            var responseTask = client.GetAsync("AllAppointments");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<Appointment>>();
                readTask.Wait();
                appointments = readTask.Result;
                var today = DateTime.Today.ToShortDateString();
                appointments = appointments.Where(a => DateTime.Parse(a.Date) < DateTime.Parse(today)).ToList();
            }

            return View(appointments);
        }
        public ActionResult CurrentDateAppointments()
        {
            IList<Appointment> appointments = null;
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
            var responseTask = client.GetAsync("AllAppointments");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<Appointment>>();
                readTask.Wait();
                appointments = readTask.Result;
                appointments = appointments.Where(a => DateTime.Parse(a.Date).ToShortDateString().Equals(DateTime.Now.ToShortDateString())).ToList();
            }

            return View(appointments);
        }
        public ActionResult CheckedInVehicles()
        {
            IList<Appointment> appointments = null;
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
            var responseTask = client.GetAsync("AllAppointments");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<Appointment>>();
                readTask.Wait();
                appointments = readTask.Result;
                appointments = appointments.Where(a=>a.Status=="Yes").ToList();
            }

            return View(appointments);
        }
        public ActionResult AllCustomers() 
        {
            IList<Customer> customers = null;
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
            var responseTask = client.GetAsync("AllCustomers");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<Customer>>();
                readTask.Wait();
                customers = readTask.Result;
            }

            return View(customers);

        }
        public ActionResult AllAppointments()
        {
            IList<Appointment> appointments = null;
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
            var responseTask = client.GetAsync("AllAppointments");
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
        public ActionResult AllAppointments(string regno)
        {
            IList<Appointment> appointments = null;
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
            var responseTask = client.GetAsync("AllAppointments");
            responseTask.Wait();
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<Appointment>>();
                readTask.Wait();
                appointments = readTask.Result;
                appointments = appointments.Where(a => a.Vehicle_Reg_No == regno).ToList();
            }
            return View(appointments);
        }
        public void ExportToExcel(List<Appointment> aps) 
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");
            int rowStart = 2;
            ws.Cells["A1"].Value = "Vehicle-Reg-No";
            ws.Cells["B1"].Value = "Dated";
            ws.Cells["C1"].Value = "Total_Bill";
            foreach (var item in aps)
            {
                ws.Cells[string.Format("A{0}", rowStart)].Value = item.Vehicle_Reg_No;
                ws.Cells[string.Format("B{0}", rowStart)].Value = item.Date;
                ws.Cells[string.Format("C{0}", rowStart)].Value = item.Total_Bill;
                rowStart++;
            }
            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition","attachement: filename="+"EcelReport.xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }
    }
}