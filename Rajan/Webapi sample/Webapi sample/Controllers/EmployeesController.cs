using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using Webapi_sample.Models;

namespace Webapi_sample.Controllers
{
    
    [BasicAuthentication]

    public class EmployeesController : ApiController
    {

        private EmployeesEntities1 entities1 = new EmployeesEntities1();
        

        [HttpGet]
        public HttpResponseMessage GetEmployees()
        {  
            {
                var employeeDetails = entities1.EmployeeDetails.ToList();
                
                try
                {
                    if (employeeDetails != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, employeeDetails);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Employee Details in the database");
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                }
            }

        }
        [HttpGet]
        [Route("api/Employees/{Id}")]
        public HttpResponseMessage GetEmployee(int id)
        {
                try
                {
                    var employeeDetail = entities1.EmployeeDetails.FirstOrDefault(e => e.Id == id);

                    if (employeeDetail != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, employeeDetail);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee Id =" + id.ToString() + " is not in the database");
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);

                }

        }

        [HttpPost]
        public HttpResponseMessage CreateEmployees([FromBody] EmployeeDetail employee)
        {
            try
            {
     

                if (employee != null)
                {
                    entities1.EmployeeDetails.Add(employee);
                    entities1.SaveChanges();
                    var message = Request.CreateResponse(HttpStatusCode.OK, employee);
                    return message;
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest,"Employee not created");
                }
                
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);

            }
        }

        [HttpPut]
        [Route("api/Employees/{Id}")]
        public HttpResponseMessage UpdateEmployees( int id,[FromBody] EmployeeDetail employee)
        {
            try
            {
                    var entity = entities1.EmployeeDetails.FirstOrDefault(e => e.Id == id);

                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee with Id = " + id.ToString() + "not found to update");
                    }
                    else
                    {
                        entity.FirstName = employee.FirstName;
                        entity.LastName = employee.LastName;
                        entity.Gender = employee.Gender;
                        entity.Salary = employee.Salary;
                        entities1.SaveChanges();
                        var message = Request.CreateResponse(HttpStatusCode.OK, entity);
                        return message;
                    }
                
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpDelete]
        [Route("api/Employees/{Id}")]
        public HttpResponseMessage DeleteEmployee(int id)
        {
            try
            {
                    var entity = entities1.EmployeeDetails.FirstOrDefault(e => e.Id == id);
                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee with Id = " + id.ToString() + " Not found to delete");
                    }
                    else
                    {
                        entities1.EmployeeDetails.Remove(entity);
                        entities1.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, "Id = "+id.ToString() + " Is deleted");
                    }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
