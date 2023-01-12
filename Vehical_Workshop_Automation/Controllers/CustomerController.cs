using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Vehical_Workshop_Automation.Models;
namespace RadioButtonMvcApp.Controllers
{
    namespace Vehical_Workshop_Automation.Controllers
    {
        public class CustomerController : Controller
        {
            public ActionResult Index()
            {
                return View();
            }
            public ActionResult Logout()
            {
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Customer");
            }
            public ActionResult Login()
            {
                return View();
            }
            public ActionResult ViewAppointments()
            {
                if (Session["U_MobileNo"] == null)
                {
                    return RedirectToAction("Login", "Customer");
                }

                string contact = Session["U_MobileNo"].ToString();
                    IList<Appointment> appointments = null;
                    var client = new HttpClient();
                    client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
                    var responseTask = client.GetAsync("GetAppointment/?contact=" + contact);
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
            public ActionResult Login(string mobileno, string password)
            {
                Customer login = new Customer();
                login.Mobile_No = mobileno;
                login.Password = password;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Customer>("Login", login);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        Customer customer = new Customer();
                        var readTask = result.Content.ReadAsAsync<Customer>();
                        readTask.Wait();

                        customer = readTask.Result;
                        if (customer.Role == "Customer")
                        {
                            Session["User_Name"] = customer.Name;
                            Session["U_MobileNo"] = customer.Mobile_No;
                            return RedirectToAction("CustomerVehicles");
                        } else if (customer.Role == "Admin")
                        {
                            Session["Admin_Name"] = customer.Name;
                            Session["Admin_MobileNo"] = customer.Mobile_No;
                            return RedirectToAction("AdminList", "Admin");
                        } else if (customer.Role == "Mechanic")
                        {
                            Session["Mechanic_Name"] = customer.Name;
                            Session["Mechanic_MobileNo"] = customer.Mobile_No;
                            return RedirectToAction("GetMechanicAppointment", "Mechanic");
                        } else if (customer.Role == "Surveyor")
                        {
                            Session["Surveyor_Name"] = customer.Name;
                            Session["Surveyor_MobileNo"] = customer.Mobile_No;
                            return RedirectToAction("GetCheckedAppointment", "Surveyor");
                        }
                        return View();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Mobile No. or Password.");
                        ViewBag.msg = "Invalid Mobile No. or Password.";
                        return View();
                    }
                }
            }

            public ActionResult CustomerSignup()
            {
                return View();
            }

            [HttpPost]
            public ActionResult CustomerSignup(string name, string mobileno, string email, string password)
            {
                Customer customer = new Customer();
                customer.Name = name;
                customer.Mobile_No = mobileno;
                customer.Email = email;
                customer.Password = password;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
                    var postTask = client.PostAsJsonAsync<Customer>("CustomerSignup", customer);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        Customer cust = new Customer();
                        var readTask = result.Content.ReadAsAsync<Customer>();
                        readTask.Wait();
                        //cust = readTask.Result;
                        //Session["User_Name"] = cust.Name;
                        //Session["U_MobileNo"] = cust.Mobile_No;
                        //return RedirectToAction("CustomerVehicles");
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Mobile No. or Password.");
                        ViewBag.msg = "Mobile No. already in use.";
                        return View();
                    }
                }
            }

            public ActionResult CustomerVehicles()
            {
                if (Session["U_MobileNo"]==null) 
                {
                    return RedirectToAction("Login","Customer");
                }
                var companies=Get_Companies();
                string mobileno = Session["U_MobileNo"].ToString();
                IList<Vehicle_Customer1> customerVehicles = null;
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
                var responseTask = client.GetAsync("CustomersVehicles/?mobileno=" + mobileno);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Vehicle_Customer1>>();
                    readTask.Wait();
                    customerVehicles = readTask.Result;
                }
                foreach (var item in customerVehicles)
                {
                    var one = companies.FirstOrDefault(c => c.id == int.Parse(item.Company));
                    item.Company = one==null? "":one.Name;
                }
                return View(customerVehicles);
            }

            [HttpPost]
            public ActionResult Edit(Vehicle_Customer1 vehicle_Customer1)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:44323/api/Customer/CustomerVehicle/");

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<Vehicle_Customer1>("EditUser", vehicle_Customer1);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("CustomerVehicles");
                    }
                }

                ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
                return View();
            }
            public ActionResult Delete(int id,string Reg_No)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
                        var deleteTask = client.DeleteAsync($"DeleteCustomerVehicle/?id={id}&Reg_No={Reg_No}");
                        deleteTask.Wait();

                        var result = deleteTask.Result;
                        if (result.IsSuccessStatusCode)
                        {

                            return RedirectToAction("CustomerVehicles");
                        }
                    }

                    return RedirectToAction("CustomerVehicles");

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Server Error. Please contact administrator.{ex}");
                    return RedirectToAction("CustomerVehicles");
                }
                
            }
            public JsonResult GetVehicles(int id)
            {
                IList<Vehicle_Name> vehicles = null;
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
                var responseTask = client.GetAsync("GetVehicleBasedOnCompany/?id=" + id);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Vehicle_Name>>();
                    readTask.Wait();

                    vehicles = readTask.Result;
                }
                return Json(vehicles,JsonRequestBehavior.AllowGet);
            }
            public ActionResult AddVehicle()
            {
                LoadModel();
                Get_Companies();
                return View();
            }
            [HttpPost]
            public ActionResult AddVehicle(string regNo, string color, string name, string model, string company, string miles)
            {
                Vehicle_Customer1 vehicleCustomer = new Vehicle_Customer1();
                vehicleCustomer.Customer_Mobile_No = Session["U_MobileNo"].ToString();
                vehicleCustomer.Reg_No = regNo;
                vehicleCustomer.Color = color;
                vehicleCustomer.Name = name;
                vehicleCustomer.Model = model;
                vehicleCustomer.Company = company;
                vehicleCustomer.Miles_Km = miles;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
                    var postTask = client.PostAsJsonAsync<Vehicle_Customer1>("AddVehicle", vehicleCustomer);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("CustomerVehicles");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Vehicle already added");
                        ViewBag.msg = "Vehicle already added.";
                        Get_Companies();
                        return View();
                    }
                }
            }
            public ActionResult GetDateTable(DateTime date)
            {
                if (date<DateTime.Today) 
                {
                    ViewBag.Invalid = "Date must not be less than today date";
                    return View("Appointments",GetSlots());
                }
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
                List<DateWiseSlots> dateWiseSlot = new List<DateWiseSlots>();
                var todayDate = date;
                int count = 7;
                for (int i = 1; i <= count; i++)
                {
                  if (todayDate.AddDays(i).DayOfWeek.ToString() == "Saturday" || todayDate.AddDays(i).DayOfWeek.ToString() == "Sunday")
                  {

                  }
                  else
                  {
                        DateWiseSlots dateviseslots = new DateWiseSlots();
                        dateviseslots.Id = todayDate.AddDays(i).ToString("MM/dd/yyyy") + "_Slot_1".ToString();
                        dateviseslots.Date = todayDate.AddDays(i).ToString("MM/dd/yyyy");
                        dateviseslots.Day = todayDate.AddDays(i).DayOfWeek.ToString();
                        //dateviseslots.Slot1 = todayDate.AddDays(i).ToString("MM/dd/yyyy") + "_Slot_1".ToString();
                        //dateviseslots.Slot2 = todayDate.AddDays(i).ToString("MM/dd/yyyy") + "_Slot_2".ToString();
                        //dateviseslots.Slot3 = todayDate.AddDays(i).ToString("MM/dd/yyyy") + "_Slot_3".ToString();
                        //dateviseslots.Slot4 = todayDate.AddDays(i).ToString("MM/dd/yyyy") + "_Slot_4".ToString();

                        dateviseslots.Slot1 = "Slot 1(9-11)";
                        dateviseslots.Slot2 = "Slot 2(11-1)";
                        dateviseslots.Slot3 = "Slot 3(2-5)";
                        dateviseslots.Slot4 = "Slot 4(5-7)";
                        dateWiseSlot.Add(dateviseslots);
                    }
                }
                foreach (var item in dateWiseSlot)
                {
                    var c = appointments.Where(a => a.Date == item.Date && a.Slot == item.Slot1).OrderByDescending(o => o.Bay_No).FirstOrDefault();
                    var c2 = appointments.Where(a => a.Date == item.Date && a.Slot == item.Slot2).OrderByDescending(o => o.Bay_No).FirstOrDefault();
                    var c3 = appointments.Where(a => a.Date == item.Date && a.Slot == item.Slot3).OrderByDescending(o => o.Bay_No).FirstOrDefault();
                    var c4 = appointments.Where(a => a.Date == item.Date && a.Slot == item.Slot4).OrderByDescending(o => o.Bay_No).FirstOrDefault();
                    if (c != null) {
                        if (c.Bay_No >= 4)
                        {
                            item.Checked1 = true;
                            item.SelectedSlot = c.Slot;
                        }
                        else
                        {
                            item.Checked1 = false;
                        }
                    }
                    if (c2 != null)
                    {
                        if (c2.Bay_No >= 4)
                        {
                            item.Checked2 = true;
                            item.SelectedSlot = c2.Slot;
                        }
                        else
                        {
                            item.Checked2 = false;
                        }
                    }
                    if (c3 != null)
                    {
                        if (c3.Bay_No >= 4)
                        {
                            item.Checked3 = true;
                            item.SelectedSlot = c3.Slot;
                        }
                        else
                        {
                            item.Checked3 = false;
                        }
                    }
                    if (c4 != null)
                    {
                        if (c4.Bay_No >= 4)
                        {
                            item.Checked4 = true;
                            item.SelectedSlot = c4.Slot;
                        }
                        else
                        {
                            item.Checked4 = false;
                        }
                    }
                }
                return View("Appointments", dateWiseSlot);
            }
            class GetDays
            {
                static void Main()
                {
                    for (int i = 5; i <= 5; i++)
                    {
                        DateTime today = DateTime.Now;
                        DateTime answer = today.AddDays(5);
                        Console.WriteLine("Today: {0:dddd}", today);
                        Console.WriteLine("36 days from today: {0:dddd}", answer);

                    }
                }

            }
            public ActionResult Services(string slot)
            {
                if (slot == null)
                {
                    return RedirectToAction("Appointments", "Customer");
                }
                else
                {
                    var sp = slot.Split(':');
                    Session["Slot"] = sp[0];
                    Session["Date"] = sp[1];
                    if (AppointmentExits(Session["RegNo"].ToString(), sp[0], sp[1]))
                    {
                        TempData["Error"] = "Appointment already exist in same slot please choose different one";
                        return RedirectToAction("CustomerVehicles", "Customer");

                    }
                    else
                    {
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
                        return View(sl);
                    }
                }
            }
            public ActionResult Appointments(string vehicle_Reg_No,string Miles_Km) 
            {
                    Session["RegNo"] = vehicle_Reg_No;
                    Session["MilesKm"] = Miles_Km;
                    var dateWiseSlots= GetSlots();
                    return View(dateWiseSlots);
            }
            [HttpPost]
            public ActionResult SaveAppointments(string slot) 
            {
                try
                {
                    if (slot==null)
                    {
                        return RedirectToAction("Appointments","Customer");
                    }
                    var sp = slot.Split(':');
                    Session["Slot"] = sp[0];
                    Session["Date"] = sp[1];
                    Appointment appoint = new Appointment();
                    appoint.Bay_No = 0;
                    appoint.Date = Session["Date"].ToString();
                    appoint.Slot = Session["Slot"].ToString();
                    appoint.Customer_Mobile_No = Session["U_MobileNo"].ToString();
                    appoint.Vehicle_Reg_No = Session["RegNo"].ToString();
                    appoint.Miles = Session["MilesKm"].ToString();
                    var client = new HttpClient();
                    client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
                    var responseTask = client.PostAsJsonAsync<Appointment>("AddAppointment", appoint);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<int>();
                        readTask.Wait();
                        Session["AppNo"]=readTask.Result.ToString();
                        return RedirectToAction("Services", "Customer");
                    }
                    else 
                    {
                        ViewBag.Error = "Appointment not saved somthing wrong";
                        return RedirectToAction("CustomerVehicles", "Customer");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.ToString();
                    return RedirectToAction("CustomerVehicles", "Customer");
                }
            }
            [HttpPost]
            public ActionResult SaveAppointmentService(ServiceList lst) 
            {
                string ap = SaveApp();
                List<App_Service> app_Services = new List<App_Service>();
                foreach (var item in lst.Services)
                {

                    if (item.Selected)
                    {
                        app_Services.Add(new App_Service { App_No =int.Parse(ap), Customer_Suggestion = item.Name,Status="Yes"});
                    }
                }
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
                var responseTask = client.PostAsJsonAsync<IList<App_Service>>("AddAppointmentService", app_Services);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("CustomerVehicles", "Customer");
                }
                else
                {
                    ViewBag.Error = "Appointment not saved somthing wrong";
                    return RedirectToAction("Services", "Customer");
                }
            }
            public ActionResult CheckIn(string date,string slot,string regno,string bayno,string apno)
            {
                if (!CheckInExits(regno,apno))
                {
                    Session["CheckDate"] = date.ToString();
                    Session["CheckSlot"] = slot.ToString();
                    Session["CheckReg"] = regno.ToString();
                    Session["CheckBay"] = bayno.ToString();
                    Session["ChkAppNo"] = apno.ToString();
                    string contact = Session["U_MobileNo"].ToString();
                    IList<Appointment> appointments = null;
                    var client = new HttpClient();
                    client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
                    var responseTask = client.GetAsync($"GetAppointment/?contact={contact}&Vehicle_Reg_No={regno}&Appointment_No={apno}");
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
                else
                {
                    TempData["Error"] = "Appointment Already Checked";
                    return RedirectToAction("ViewAppointments");

                }
            }
            [HttpPost]
            public ActionResult SaveCheckIn(string currentMiles)
            {
                Appointment aps = new Appointment();
                aps.Slot = Session["CheckSlot"].ToString();
                aps.Date = Session["CheckDate"].ToString();
                aps.Customer_Mobile_No = Session["U_MobileNo"].ToString();
                aps.CurrentMiles = currentMiles;
                aps.Vehicle_Reg_No = Session["CheckReg"].ToString();
                aps.Appointment_No = int.Parse(Session["ChkAppNo"].ToString());
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
                    var postTask = client.PostAsJsonAsync<Appointment>("UpdateAppointment", aps);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("CustomerVehicles");
                    }
                    return RedirectToAction("CustomerVehicles", "Customer");
                }
            }
            public ActionResult SurveyorSuggestion(int Appointment_No) 
            {
                var lst = GetSuggestion(Appointment_No);
                Session["Aps"] = Appointment_No;
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
                    if (lst.FirstOrDefault(a => a.Surveyor_Suggestion == item.Name) == null)
                    {
                        continue;
                    }
                    else 
                    {
                        services2.Add(item);
                    }
                }
                ServiceList sl = new ServiceList();
                sl.Services = services2.ToList();
                return View(sl);
            }
            [HttpPost]
            public ActionResult SaveSuggestion(ServiceList lst)
            {
                List<App_Service> app_Services = new List<App_Service>();
                var appServiceList = GetAppService(int.Parse(Session["Aps"].ToString()));
                foreach (var item in lst.Services)
                {

                    if (item.Selected)
                    {
                        var val = appServiceList.FirstOrDefault(a => a.Surveyor_Suggestion == item.Name && a.App_No == int.Parse(Session["Aps"].ToString()));
                        if (val!=null) 
                        {
                            app_Services.Add(new App_Service {Service_Id=val.Service_Id,App_No =val.App_No,Customer_Suggestion =item.Name,Status = "Yes" });
                        }

                    }
                }
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
                var responseTask = client.PostAsJsonAsync<IList<App_Service>>("UpdateAppointmentService", app_Services);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    if (SetSurveyChecked())
                    {
                        return RedirectToAction("ViewAppointments", "Customer");

                    }
                    else 
                    {
                        TempData["Error"] = "Not save changes somthing wrong";
                        return RedirectToAction("ViewAppointments", "Customer");
                    }
                }
                else
                {
                    TempData["Error"] = "Not save changes somthing wrong";
                    return RedirectToAction("ViewAppointments", "Customer");
                }
            }
            private bool AppointmentExits(string regno,string slot,string date) 
            {
                string contact = Session["U_MobileNo"].ToString();
                IList<Appointment> appointments = null;
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
                var responseTask = client.GetAsync("GetAppointment/?contact=" + contact);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Appointment>>();
                    readTask.Wait();
                    appointments = readTask.Result;
                }
                var check = appointments.FirstOrDefault(a => a.Vehicle_Reg_No == regno && a.Slot==slot && a.Date==date);
                if (check == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
            private bool CheckInExits(string regno,string apno)
            {
                string contact = Session["U_MobileNo"].ToString();
                IList<Appointment> appointments = null;
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
                var responseTask = client.GetAsync("GetAppointment/?contact=" + contact);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Appointment>>();
                    readTask.Wait();
                    appointments = readTask.Result;
                }
                var check = appointments.FirstOrDefault(a => a.Vehicle_Reg_No == regno && a.Appointment_No==int.Parse(apno));
                if (check == null)
                {
                    return false;
                }
                else
                {
                    if (check.Status == "Yes")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            }
            private string SaveApp()
            {
                try
                {
                    Appointment appoint = new Appointment();
                    appoint.Bay_No = 0;
                    appoint.Date = Session["Date"].ToString();
                    appoint.Slot = Session["Slot"].ToString();
                    appoint.Customer_Mobile_No = Session["U_MobileNo"].ToString();
                    appoint.Vehicle_Reg_No = Session["RegNo"].ToString();
                    appoint.Miles = Session["MilesKm"].ToString();
                    var client = new HttpClient();
                    client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
                    var responseTask = client.PostAsJsonAsync<Appointment>("AddAppointment", appoint);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<int>();
                        readTask.Wait();
                        Session["AppNo"] = readTask.Result.ToString();
                        return readTask.Result.ToString();
                    }
                    else
                    {
                        ViewBag.Error = "Appointment not saved somthing wrong";
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.ToString();
                    return null;
                }
            }
            public ActionResult SetServicesVehicle(int Appointment_No)
            {
                var lst = GetAppService(Appointment_No);
                var newlst = lst.Where(l => l.Customer_Suggestion != null);
                return View(newlst);
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
                var filter = services.Where(s => s.App_No == Appointment_No).ToList();
                return filter;
            }
            private IList<App_Service> GetSuggestion(int Appointment_No)
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
                var filter = services.Where(s => s.App_No == Appointment_No).ToList();
                return filter;
            }
            private IList<Vehicle_Company> Get_Companies() 
            {
                IList<Vehicle_Company> vehicles = null;
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
                var responseTask = client.GetAsync("GetVehicleCompany");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Vehicle_Company>>();
                    readTask.Wait();
                    vehicles = readTask.Result;
                    ViewBag.VehicleCompany = vehicles;
                    return (List<Vehicle_Company>)vehicles;
                }
                else 
                {
                    return null;
                }
            }
            public void LoadModel()
            {
                List<SelectListItem> lst = new List<SelectListItem>();
                for (int i = 1990; i <=2022; i++)
                {
                   
                    lst.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
                ViewBag.Model = lst;
            }
            public List<DateWiseSlots> GetSlots() 
            {
                List<DateWiseSlots> dateWiseSlots = new List<DateWiseSlots>();
                var todayDate = DateTime.Now;
                int count = 7;
                for (int i = 1; i <= count; i++)
                {
                    if (todayDate.AddDays(i).DayOfWeek.ToString() == "Saturday" || todayDate.AddDays(i).DayOfWeek.ToString() == "Sunday")
                    {

                    }
                    else
                    {
                        DateWiseSlots dateWise = new DateWiseSlots();
                        dateWise.Id = todayDate.AddDays(i).ToString("MM/dd/yyyy") + "_Slot_1".ToString();
                        dateWise.Date = todayDate.AddDays(i).ToString("MM/dd/yyyy");
                        dateWise.Slot1 = "Slot_1";
                        dateWise.Slot2 = "Slot_2";
                        dateWise.Slot3 = "Slot_3";
                        dateWise.Slot4 = "Slot_4";
                        dateWiseSlots.Add(dateWise);
                    }

                }
                return dateWiseSlots;
            }
            private bool SetSurveyChecked()
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:44323/api/Customer/");
                var responseTask = client.GetAsync($"SurveyorChecked/?aps={int.Parse(Session["Aps"].ToString())}");
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
}
