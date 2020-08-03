using challenge.Controllers;
using challenge.Data;
using challenge.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using code_challenge.Tests.Integration.Extensions;

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using code_challenge.Tests.Integration.Helpers;
using System.Text;

namespace code_challenge.Tests.Integration
{
    [TestClass]
    public class CompensationControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<TestServerStartup>()
                .UseEnvironment("Development"));

            _httpClient = _testServer.CreateClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }
        [TestMethod]
        public void CreateCompensation_Returns_Created()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedSalary = "100000";
            var expectedEffectiveDate = Convert.ToDateTime("2017-09-08T19:01:55.714942+03:00");

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}");
            var employee_Response = getRequestTask.Result;            

            var employee = employee_Response.DeserializeContent<Employee>();


            Compensation compensation = new Compensation()
            {
                Employee = employee,
                Salary = expectedSalary,
                EffectiveDate = expectedEffectiveDate
            };

            var requestContent = new JsonSerialization().ToJson(compensation);
            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(newCompensation);
            Assert.AreEqual(compensation.Employee.EmployeeId, newCompensation.Employee.EmployeeId);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
            Assert.AreEqual(compensation.EffectiveDate, newCompensation.EffectiveDate);
        }

        [TestMethod]
        public void GetCompensationById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedSalary = "100000";
            var expectedEffectiveDate = Convert.ToDateTime("2017-09-08T19:01:55.714942+03:00");


            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var new_Compensation = response.DeserializeContent<Compensation>();
            Assert.AreEqual(expectedSalary, new_Compensation.Salary);
            Assert.AreEqual(expectedEffectiveDate, new_Compensation.EffectiveDate);
        }
    }
}
