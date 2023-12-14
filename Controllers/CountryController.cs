using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using SqlDemo.Models;
using System.Diagnostics;
using System.Web.Mvc;
using Controller = Microsoft.AspNetCore.Mvc.Controller;
using Microsoft.Azure.Services.AppAuthentication;


namespace SqlDemo.Controllers
{
    [Authorize]
    public class CountryController : Controller
    {
        private readonly ILogger<CountryController> _logger;
        private readonly IConfiguration _configuration;

        private string? _connectionString = string.Empty;//   @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=WAFDB;Integrated Security=True;Connect Timeout=30;";

        public CountryController(ILogger<CountryController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _connectionString = configuration["ConnectionStrings:SqlCon"];
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Secret"] = _configuration["mySecret2"];

            var capitals = new List<CountryInfo>();
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlCommand = new SqlCommand("SELECT Country, Capital FROM CountryInfo", sqlConnection);

                var accessToken = await (new AzureServiceTokenProvider()).GetAccessTokenAsync("https://database.windows.net");
                sqlConnection.AccessToken = accessToken;

                sqlConnection.Open();

                var reader = sqlCommand.ExecuteReader();

                while (reader.Read())
                {
                    capitals.Add(new CountryInfo
                    {
                        Country = (string)reader["Country"],
                        Capital = (string)reader["Capital"]
                    }
                    );
                }

                sqlConnection.Close();
                _logger.LogInformation("Successfully retrieved data from database.");
            }

            return View(capitals);
        }

        public IActionResult Create()
        {
            return View();
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        [ValidateInput(false)]
        public async Task<IActionResult> Create(IFormCollection collection)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var accessToken = await (new AzureServiceTokenProvider()).GetAccessTokenAsync("https://database.windows.net");
                sqlConnection.AccessToken = accessToken;

                sqlConnection.Open();


                var insertCommand = $"INSERT INTO CountryInfo (Country, Capital) " +
                                    $"VALUES ('{collection["Country"]}', '{collection["Capital"]}')";
                var sqlCommand = new SqlCommand(insertCommand, sqlConnection);

                sqlCommand.ExecuteNonQuery();

                sqlConnection.Close();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}