using Employee_Self_Service.DAL;
using Employee_Self_Service.Modals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Employee_Self_Service.Controllers
{
    [Authorize]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _db;

        public EmployeeController(AppDbContext appDbContext)
        {
            _db = appDbContext;
        }

        [HttpGet]
        [Route("api/Employee/getAllEmployees")]
        [Authorize(Roles = "Admin,HR,Employee")]
        public IActionResult getAllEmployees(string obj,int r, int p)
        {
            try
            {
                emp_info? search_emp_Info = JsonSerializer.Deserialize<emp_info>(obj);

                List<emp_info> employees = _db.emp_info
                                              .Where(x => x.emp_no == search_emp_Info.emp_no || search_emp_Info.emp_no == 0)
                                              .Skip(r * p)
                                              .Take(p)
                                              .ToList();

                int totalRecords = _db.emp_info.Count();

                employees.ForEach(x => x.totalRecords = totalRecords);

                return Ok(employees);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpGet]
        [Route("api/Employee/getEmployeeById")]
        [Authorize(Roles = "Admin,HR")]
        public IActionResult getEmployeeById(int Id)
        {
            try
            {
                emp_info? employee = _db.emp_info.Where(x => x.emp_no == Id).FirstOrDefault();

                if (employee != null)
                {
                    return Ok(employee);
                }
                else
                {
                    return Ok("No Employee Found");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpPost]
        [Route("api/Employee/saveEmployee")]
        [Authorize(Roles = "Admin")]
        public IActionResult saveEmployee(string obj)
        {
            try
            {
                emp_info? emp_Info = JsonSerializer.Deserialize<emp_info>(obj);

                if (emp_Info != null)
                {
                    _db.emp_info.Add(emp_Info);
                    _db.SaveChanges();
                }

                return Ok(new { message = "saved" });
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPut]
        [Route("api/Employee/updateEmployee")]
        [Authorize(Roles = "Admin")]
        public IActionResult updateEmployee(string obj)
        {
            try
            {
                emp_info? emp_Info = JsonSerializer.Deserialize<emp_info>(obj);

                if (emp_Info != null)
                {
                    _db.emp_info.Update(emp_Info);
                    _db.SaveChanges();
                }

                return Ok(new { message = "updated" });
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpDelete]
        [Route("api/Employee/deleteEmployee")]
        [Authorize(Roles = "Admin")]
        public IActionResult deleteEmployee(int Id)
        {
            try
            {
                if (Id != 0)
                {
                    emp_info? emp_Info = _db.emp_info.Where(x => x.emp_no == Id).FirstOrDefault();

                    if (emp_Info != null)
                    {
                        _db.emp_info.Remove(emp_Info);
                        _db.SaveChanges();
                        return Ok(new { message = "deleted" });
                    }    
                }

                return Ok(new { message = "not deleted" });
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
