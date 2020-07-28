using challenge.Models;
using challenge.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace challenge.Controllers
{
    [Route("api/reportingstructure")]
    public class ReportingStructureController : Controller
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public ReportingStructureController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        [HttpGet("{id}", Name = "getReportingStructureById")]
        public IActionResult GetReportingStructureById(String id)
        {
            _logger.LogDebug($"Received reporting structure request for '{id}'");
            var employee = _employeeService.GetById(id);
            int count = 0;
            ReportingStructure return_ReportingStructure;
            if (employee == null)
                return NotFound();
            else
            {
                Queue<Employee> structureQueue = new Queue<Employee>();
                if ((employee.DirectReports != null) && (employee.DirectReports.Any()))
                {
                    foreach(Employee reporting_Employee in employee.DirectReports)
                    {
                        structureQueue.Enqueue(reporting_Employee);
                    }
                    while (structureQueue.Count != 0)
                    {
                        var next_Employee = structureQueue.Dequeue();
                        count++;
                        if ((next_Employee.DirectReports != null) && (next_Employee.DirectReports.Any()))
                        {
                            foreach(Employee reporting_Employee in next_Employee.DirectReports)
                            {
                                structureQueue.Enqueue(reporting_Employee);
                            }
                        }
                    }
                    return_ReportingStructure = new ReportingStructure()
                    {
                        Employee = employee,
                        NumberOfReports = count
                    };
                    return Ok(return_ReportingStructure);
                }
                else
                {
                    return_ReportingStructure = new ReportingStructure()
                    {
                        Employee = employee,
                        NumberOfReports = count
                    };
                    return Ok(return_ReportingStructure);
                }
            }
        }
    }
}
