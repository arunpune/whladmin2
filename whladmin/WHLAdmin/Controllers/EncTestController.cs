using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System;
using System.Diagnostics.CodeAnalysis;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Controllers
{
    [AllowAnonymous]
    [ExcludeFromCodeCoverage]
    public class EncTestController : Controller
    {
        private readonly string _dbConnString;

        public EncTestController(IConfiguration configuration)
        {
            _dbConnString = configuration.GetConnectionString("AdminDbConnString");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var dbConn = new SqlConnection(_dbConnString);
                var result = await dbConn.QueryAsync<EncTest>("SELECT [Name], SSN, DOB FROM dbo.tblEncTest");
                return View(result);
            }
            catch (Exception exception)
            {
                return View("Error", new ErrorViewModel() { Code = "EX000", Message = "Exception when retrieving", Details = exception.ToString() });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] EncTest item)
        {
            try
            {
                var dbConn = new SqlConnection(_dbConnString);
                var queryParams = new DynamicParameters();
                queryParams.Add("@Name", item.Name, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 50);
                queryParams.Add("@SSN", item.SSN, System.Data.DbType.AnsiString, System.Data.ParameterDirection.Input, 9);
                queryParams.Add("@DOB", item.DOB, System.Data.DbType.Date, System.Data.ParameterDirection.Input);
                var result = await dbConn.ExecuteAsync("INSERT INTO dbo.tblEncTest ([Name], SSN, DOB) VALUES (@Name, @SSN, @DOB)", queryParams);
                if (result > 0) return RedirectToAction("Index");
                return View("Error", new ErrorViewModel() { Code = "ADD", Message = "Failed to add" });
            }
            catch (Exception exception)
            {
                return View("Error", new ErrorViewModel() { Code = "EX000", Message = "Exception while adding", Details = exception.ToString() });
            }
        }
    }
}

namespace WHLAdmin.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class EncTest
    {
        public string Name { get; set; }
        public string SSN { get; set; }
        public DateTime DOB { get; set; }
    }
}
