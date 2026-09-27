using Employee_Self_Service.DAL;
using Employee_Self_Service.Modals.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Employee_Self_Service.Controllers
{
    [Authorize]
    [ApiController]
    public class ScreenController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ScreenController(AppDbContext appDbContext)
        {
            _db = appDbContext;
        }

        [HttpGet]
        [Route("api/Screens/getAllScreensBasic")]
        [Authorize(Roles = "Admin")]
        public IActionResult getAllScreensBasic()
        {
            try
            {
                List<Screens> screens = _db.screens.ToList();

                return Ok(screens);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpGet]
        [Route("api/Screens/getAllScreens")]
        [Authorize(Roles = "Admin")]
        public IActionResult getAllScreens(string? obj,int r, int p)
        {
            try
            {   
                List<Screens> screens = _db.screens
                                              .Where(x => string.IsNullOrEmpty(obj) || x.Name.ToLower().Contains(obj.ToLower()))
                                              .Skip(r * p)
                                              .Take(p)
                                              .ToList();

                int totalRecords = _db.screens.Count();

                screens.ForEach(x => x.totalRecords = totalRecords);

                return Ok(screens);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpGet]
        [Route("api/Screens/getScreenById")]
        [Authorize(Roles = "Admin")]
        public IActionResult getScreenById(int Id)
        {
            try
            {
                Screens? screen = _db.screens.Where(x => x.Id == Id).FirstOrDefault();

                if (screen != null)
                {
                    return Ok(screen);
                }
                else
                {
                    return Ok("No Screen Found");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpPost]
        [Route("api/Screens/saveScreen")]
        [Authorize(Roles = "Admin")]
        public IActionResult saveScreen(Screens screen)
        {
            try
            {
                if (screen != null)
                {
                    _db.screens.Add(screen);
                    _db.SaveChanges();
                }

                return Ok(new { message = "saved" });
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [Route("api/Screens/updateScreen")]
        [Authorize(Roles = "Admin")]
        public IActionResult updateScreen(Screens screen)
        {
            try
            {
                if (screen != null)
                {
                    _db.screens.Update(screen);
                    _db.SaveChanges();
                }

                return Ok(new { message = "updated" });
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        [Route("api/Screens/deleteScreen")]
        [Authorize(Roles = "Admin")]
        public IActionResult deleteScreen(int Id)
        {
            try
            {
                if (Id != 0)
                {
                    Screens? screens = _db.screens.Where(x => x.Id == Id).FirstOrDefault();

                    if (screens != null)
                    {
                        _db.screens.Remove(screens);
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
