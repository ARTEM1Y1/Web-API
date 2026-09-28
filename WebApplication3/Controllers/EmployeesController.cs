using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class EmployeesController : ApiController
    {
        [HttpGet, Route("api/employees")]
        public IHttpActionResult GetAll()
        {
            return Ok(Data.Employees);
        }
        [HttpGet, Route("api/employees/{id:int}")]
        public IHttpActionResult GetById(int id)
        {
            Employee emp = Data.Employees.FirstOrDefault(e => e.Id == id);
            if (emp == null) return Content(HttpStatusCode.NotFound, new { message = "Сотрудник " + id + " не найден." });
            return Ok(emp);
        }
        [HttpPost, Route("api/employees")]
        public IHttpActionResult Create(Employee emp)
        {
            if (emp == null || !ModelState.IsValid) return BadRequest("Некорректное тело запроса.");

            IHttpActionResult error = Check(emp, 0);
            if (error != null) return error;

            emp.Id = Data.NextEmployeeId++;
            emp.FullName = emp.FullName.Trim();
            emp.Email = emp.Email.Trim();
            Data.Employees.Add(emp);
            return Created(Request.RequestUri + "/" + emp.Id, emp);
        }
        [HttpPut, Route("api/employees/{id:int}")]
        public IHttpActionResult Update(int id, Employee newData)
        {
            Employee emp = Data.Employees.FirstOrDefault(e => e.Id == id);
            if (emp == null) return Content(HttpStatusCode.NotFound, new { message = "Сотрудник " + id + " не найден." });
            if (newData == null || !ModelState.IsValid) return BadRequest("Некорректное тело запроса.");

            IHttpActionResult error = Check(newData, id);
            if (error != null) return error;

            emp.FullName = newData.FullName.Trim();
            emp.Department = newData.Department;
            emp.Email = newData.Email.Trim();
            return Ok(emp);
        }
        private IHttpActionResult Check(Employee emp, int selfId)
        {
            if (string.IsNullOrWhiteSpace(emp.FullName))
                return BadRequest("ФИО не может быть пустым.");
            if (string.IsNullOrWhiteSpace(emp.Email))
                return BadRequest("Email не может быть пустым.");

            bool exists = Data.Employees.Any(e => e.Id != selfId
                && string.Equals(e.Email, emp.Email.Trim(), StringComparison.OrdinalIgnoreCase));
            if (exists)
                return Content(HttpStatusCode.Conflict, new { message = "Сотрудник с таким Email уже существует." });

            return null;
        }
    }
}