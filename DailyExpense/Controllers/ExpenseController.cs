using DailyExpense.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Text;

namespace DailyExpense.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly ILogger<ExpenseController> _logger;
        Uri baseAddress = new Uri("https://localhost:44328/api");
        HttpClient client;

        public ExpenseController(ILogger<ExpenseController> logger)
        {
            client = new HttpClient();
            client.BaseAddress = baseAddress;
        }
        public IActionResult Index()
        {
            List<Expense> modelList = new List<Expense>();
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                modelList = JsonConvert.DeserializeObject<List<Expense>>(data);
                ViewData["Detail"] = modelList;
            }

            return View();
        }

        [HttpPost]
        public ActionResult Search(Expense exp)
        {
            string query = ToQueryString(exp);
            List<Expense> modelList = new List<Expense>();
            //HttpResponseMessage response = client.GetAsync(client.BaseAddress + "/" + exp).Result;
            HttpResponseMessage response = client.GetAsync(client.BaseAddress + "?" + query).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                modelList = JsonConvert.DeserializeObject<List<Expense>>(data);
                ViewData["Detail"] = modelList;

            }

            return PartialView("_GridView");

        }

        // Helper function to convert the object to a query string
        string ToQueryString(object obj)
        {
            var properties = from property in obj.GetType().GetProperties()
                             let value = property.GetValue(obj)
                             where value != null
                             select $"{property.Name}={Uri.EscapeDataString(value.ToString())}";
            return string.Join("&", properties);
        }

        public ActionResult Edit(int id)
        {
            List<Expense> modelList = new List<Expense>()
            {
                new Expense (){ errorMessage = "" }
            };

            HttpResponseMessage response = client.GetAsync(client.BaseAddress + "?" + "id="+ id).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                modelList = JsonConvert.DeserializeObject<List<Expense>>(data);
            }

            ViewData["Detail"] = modelList;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult<String>> Edit(Expense exp)
        {
            string data = JsonConvert.SerializeObject(exp);
            string query = ToQueryString(exp);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PutAsync(client.BaseAddress + "?" + query, content).Result;
            if (response.IsSuccessStatusCode)
            {
                List<Expense> modelList = new List<Expense>();
                HttpResponseMessage response2 = client.GetAsync(client.BaseAddress).Result;
                data = response2.Content.ReadAsStringAsync().Result;
                modelList = JsonConvert.DeserializeObject<List<Expense>>(data);
                ViewData["Detail"] = modelList;
                return View("Index");
            }
            return View();
        }

        public ActionResult Create(int id)
		{
			List<Expense> modelList = new List<Expense>()
			{
				new Expense (){ errorMessage = "" }
			};

            

            ViewData["Detail"] = modelList;
			return View();
		}



        [HttpPost]
        public async Task<ActionResult<String>> Create(Expense model)
        {
            DateTime result;
            List<Expense> modelList = new List<Expense>()

            {
                new Expense { errorMessage = "" }
            };

            if (string.IsNullOrEmpty(model.tanggal_pengeluaran))
            {
                model.tanggal_pengeluaran = "" + DateTime.Now;
            }

            if (!DateTime.TryParse(model.tanggal_pengeluaran, out result))
            {

                modelList[0].errorMessage = "Date format is not valid";
                ViewData["Detail"] = modelList;
                return View();
            }

            Expense obj = new Expense()
            {
                id = model.id,
                nama_pengeluaran = model.nama_pengeluaran,
                jumlah_pengeluaran = model.jumlah_pengeluaran,
                jumlah_pengeluaran2 = model.jumlah_pengeluaran2,
                tanggal_pengeluaran = model.tanggal_pengeluaran,
                tanggal_pengeluaran2 = model.tanggal_pengeluaran2,
                catatan = model.catatan
            };



            string json = JsonConvert.SerializeObject(obj);

            // Create a StringContent with JSON data
            StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            // Send the POST request
            HttpResponseMessage response = await client.PostAsync(client.BaseAddress, content);

            

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}
