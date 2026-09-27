using Employee_Self_Service.DAL;
using Employee_Self_Service.Modals.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Self_Service.Controllers
{
    [Authorize]
    [ApiController]
    public class RoleActivitiesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public RoleActivitiesController(AppDbContext appDbContext)
        {
            _db = appDbContext;
        }

        ///////////////////////////// Roles Activities Management /////////////////
        ///////////////////////////////////////////////////////////////////////////

        // GET ROLE ACTIVITIES
        [Authorize(Roles = "Admin")]
        [Route("api/RoleActivities/getAllRoleActivities")]
        [HttpPost("get-role-activities")]
        public IActionResult getAllRoleActivities(string role)
        {
            try
            {
                List<RoleActivities> roleActivities = new List<RoleActivities>();

                if (role != null)
                {
                    roleActivities = _db.roleActivities.Where(x => x.Role == role).ToList();
                }

                return Ok(roleActivities);
            }
            catch (Exception)
            {

                throw;
            }
        }

        // CREATE ROLE ACTIVITIES
        [Authorize(Roles = "Admin")]
        [Route("api/RoleActivities/CreateRoleActivities")]
        [HttpPost("create-role-activities")]
        public IActionResult CreateRoleActivities(string role)
        {
            try
            {
                if (role != null)
                {
                    List<Screens> screens = _db.screens.ToList();

                    List<RoleActivities> roleActivities = new List<RoleActivities>();

                    foreach (var screen in screens)
                    {
                        roleActivities.Add(new RoleActivities
                        {
                            Role = role,
                            Screen = screen.Name,
                            canView = false,
                            canAdd = false,
                            canEdit = false,
                            canDelete = false
                        });
                    }


                    _db.roleActivities.AddRange(roleActivities);
                    _db.SaveChanges();
                }

                return Ok();
            }
            catch (Exception)
            {

                throw;
            }
        }

        // UPDATE ROLE ACTIVITIES
        [Authorize(Roles = "Admin")]
        [Route("api/RoleActivities/UpdateRoleActivities")]
        [HttpPost("update-role-activities")]
        public IActionResult UpdateRoleActivities(RoleActivities[] model)
        {
            try
            {
                if (model != null)
                {
                    _db.roleActivities.UpdateRange(model);
                    _db.SaveChanges();
                }

                return Ok();
            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}
