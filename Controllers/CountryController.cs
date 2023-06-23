﻿using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using SqlDemo.Models;
using System.Diagnostics;
using System.Web.Mvc;
using Controller = Microsoft.AspNetCore.Mvc.Controller;

namespace SqlDemo.Controllers
{
    public class CountryController : Controller
    {
        private readonly ILogger<CountryController> _logger;
        private readonly IConfiguration _configuration;
 
        private string? _connectionString = string.Empty;// @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=WAFDB;Integrated Security=True;Connect Timeout=30;";

        public CountryController(ILogger<CountryController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _connectionString = _configuration["ConnectionStrings:SqlCon"];
        }

        public IActionResult Index()
        {
            var capitals = new List<CountryInfo>();

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlCommand = new SqlCommand("SELECT Country, Capital FROM CountryInfo", sqlConnection);

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

        // GET: Country/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Country/Create
        [Microsoft.AspNetCore.Mvc.HttpPost]
        [ValidateInput(false)]
        public IActionResult Create(IFormCollection collection)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var insertCommand = $"INSERT INTO CountryInfo (Country, Capital) " +
                                    $"VALUES ('{collection["Country"]}', '{collection["Capital"]}')";
                var sqlCommand = new SqlCommand(insertCommand, sqlConnection);

                sqlConnection.Open();

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