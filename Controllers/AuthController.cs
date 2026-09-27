using Employee_Self_Service.DAL;
using Employee_Self_Service.Modals.DTOs;
using Employee_Self_Service.Modals.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Employee_Self_Service.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly AppDbContext _db;

        private readonly IConfiguration _configuration;

        public AuthController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            AppDbContext appDbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _db = appDbContext;
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!)
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [Route("api/Auth/Register")]
        public async Task<IActionResult> Register(RegisterUser model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userManager.FindByNameAsync(model.UserName);

            if (existingUser != null)
                return BadRequest("Username already exists.");

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName
            };

            var result = await _userManager.CreateAsync(user, model.PasswordHash);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _userManager.AddToRoleAsync(user, "Employee");

            return Ok(new
            {
                message = "User created successfully."
            });
        }

        [Route("api/Auth/login")]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUser model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user == null)
                return Unauthorized("Invalid username or password.");

            bool validPassword =
                await _userManager.CheckPasswordAsync(user, model.Password);

            if (!validPassword)
                return Unauthorized("Invalid username or password.");

            var token = await GenerateJwtToken(user);

            var roles = await _userManager.GetRolesAsync(user);

            var permissions = _db.roleActivities
                              .Where(x => roles.Contains(x.Role))
                              .Select(x => new
                              {
                                  x.Role,
                                  x.Screen,
                                  x.canView,
                                  x.canAdd,
                                  x.canEdit,
                                  x.canDelete
                              })
                              .ToList();

            return Ok(new
            {
                token,
                expiresIn = 3600,
                roles,
                permissions,
                user = new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.FullName
                }   
            });
        }

        [Authorize(Roles = "Admin")]
        [Route("api/Auth/AssignRole")]
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole(AssignRole dto)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);

            if (user == null)
                return NotFound("User not found.");

            if (!await _roleManager.RoleExistsAsync(dto.RoleName))
                return BadRequest("Role does not exist.");

            var userRoles = await _userManager.GetRolesAsync(user);

            if (userRoles.Contains(dto.RoleName))
                return BadRequest("User already has this role.");

            var result =
                await _userManager.AddToRoleAsync(user, dto.RoleName);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Role assigned successfully.");
        }

        ///////////////////////////// User Management /////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        // GET ALL USERS
        [Authorize(Roles = "Admin")]
        [Route("api/Auth/getAllUsers")]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers(string? userName = null, int r = 0, int p = 10)
        {
            var query = _userManager.Users.Where(x => userName == null || x.UserName == userName);

            var totalRecords = query.Count();

            var users = query
                .Skip(r * p)
                .Take(p)
                .ToList();

            var result = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.FullName,
                    user.PhoneNumber,
                    role = roles.FirstOrDefault()
                });
            }

            return Ok(new
            {
                users = result,
                totalRecords
            });
        }

        // GET USER By ID
        [Authorize(Roles = "Admin")]
        [Route("api/Auth/getUserById")]
        [HttpGet("get-users")]
        public async Task<IActionResult> getUserById(string id)
        {
            var user = _userManager.Users
                        .Where(x => x.Id == id)
                        .FirstOrDefault();

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.FullName,
                user.PhoneNumber,
                role = roles.FirstOrDefault()
            });
        }

        // CREATE USER
        [Authorize(Roles = "Admin")]
        [Route("api/Auth/CreateUser")]
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser(RegisterUser model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userManager.FindByNameAsync(model.UserName);

            if (existingUser != null)
                return BadRequest("Username already exists.");

            var existingEmail = await _userManager.FindByEmailAsync(model.Email);

            if (existingEmail != null)
                return BadRequest("Email already exists.");

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.PasswordHash);

            if (model.Role != null)
            {
                await AssignRole(new AssignRole() { UserName = model.UserName, RoleName = model.Role });
            }

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new{message = "User created successfully."});
        }


        // EDIT USER
        [Authorize(Roles = "Admin")]
        [Route("api/Auth/EditUser")]
        [HttpPut("edit-user")]
        public async Task<IActionResult> EditUser(RegisterUser model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
                return NotFound("User not found.");

            // Check if new username belongs to another user
            if (!string.IsNullOrEmpty(model.UserName) &&
                model.UserName != user.UserName)
            {
                var existingUser =
                    await _userManager.FindByNameAsync(model.UserName);

                if (existingUser != null && existingUser.Id != user.Id)
                    return BadRequest("Username already exists.");

                user.UserName = model.UserName;
            }

            // Check if new email belongs to another user
            if (!string.IsNullOrEmpty(model.Email) &&
                model.Email != user.Email)
            {
                var existingEmail =
                    await _userManager.FindByEmailAsync(model.Email);

                if (existingEmail != null && existingEmail.Id != user.Id)
                    return BadRequest("Email already exists.");

                user.Email = model.Email;
            }

            if (!string.IsNullOrEmpty(model.FullName))
                user.FullName = model.FullName;

            if (!string.IsNullOrEmpty(model.PhoneNumber))
                user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Change password if provided
            if (!string.IsNullOrEmpty(model.PasswordHash))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var passwordResult =
                    await _userManager.ResetPasswordAsync(
                        user,
                        token,
                        model.PasswordHash);

                if (!passwordResult.Succeeded)
                    return BadRequest(passwordResult.Errors);
            }

            if (model.Role != null)
            {
                await AssignRole(new AssignRole() { UserName = model.UserName, RoleName = model.Role });
            }

            return Ok(new
            {
                message = "User updated successfully."
            });
        }


        // DELETE USER
        [Authorize(Roles = "Admin")]
        [Route("api/Auth/DeleteUser")]
        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound("User not found.");

            // Optional: prevent admin from deleting himself
            if (user.Id == _userManager.GetUserId(User))
                return BadRequest("You cannot delete your own account.");

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new
            {
                message = "User deleted successfully."
            });
        }

        ///////////////////////////// Roles Management /////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        // GET ALL Roles
        [Authorize(Roles = "Admin")]
        [Route("api/Auth/getAllRoles")]
        [HttpGet("roles")]
        public async Task<IActionResult> getAllRoles(string? Name = null, int r = 0, int p = 10)
        {
            var roles = _roleManager.Roles
                        .Where(x => x.Name == Name || Name == null)
                        .Skip(r * p)
                        .Take(p)
                        .ToList();

            return Ok(new { roles, totalRecords = _roleManager.Roles.ToList().Count });
        }

        // GET ROLES By ID
        [Authorize(Roles = "Admin")]
        [Route("api/Auth/getRoleById")]
        [HttpGet("get-roles")]
        public async Task<IActionResult> getRoleById(string id)
        {
            var role = _roleManager.Roles
                        .Where(x => x.Id == id)
                        .FirstOrDefault();

            return Ok(role);
        }

        // CREATE ROLE
        [Authorize(Roles = "Admin")]
        [Route("api/Auth/CreateRole")]
        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole(RegisterRole model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingRole = await _roleManager.FindByNameAsync(model.Name);

            if (existingRole != null)
                return BadRequest("Role Name already exists.");

            var role = new IdentityRole
            {
                Name = model.Name
            };

            var result = await _roleManager.CreateAsync(role);

            //////////////////// Create Role Activities ///////////////////

            List<Screens> screens = _db.screens.ToList();

            List<RoleActivities> roleActivities = new List<RoleActivities>();

            foreach (var screen in screens)
            {
                roleActivities.Add(new RoleActivities
                {
                    Role = role.Name,
                    Screen = screen.Name,
                    canView = false,
                    canAdd = false,
                    canEdit = false,
                    canDelete = false
                });
            }


            _db.roleActivities.AddRange(roleActivities);
            _db.SaveChanges();

            //////////////////// End Create Role Activities ///////////////////

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Role created successfully." });
        }


        // EDIT Role
        [Authorize(Roles = "Admin")]
        [Route("api/Auth/EditRole")]
        [HttpPut("edit-role")]
        public async Task<IActionResult> EditRole(RegisterRole model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var role = await _roleManager.FindByIdAsync(model.Id);

            if (role == null)
                return NotFound("Role not found.");

            // Check if new username belongs to another user
            if (!string.IsNullOrEmpty(model.Name) &&
                model.Name != role.Name)
            {
                var existingRole =
                    await _roleManager.FindByNameAsync(model.Name);

                if (existingRole != null && existingRole.Id != role.Id)
                    return BadRequest("Role Name already exists.");

                role.Name = model.Name;
            }

            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new
            {
                message = "Role updated successfully."
            });
        }


        // DELETE ROLE
        [Authorize(Roles = "Admin")]
        [Route("api/Auth/DeleteRole")]
        [HttpDelete("delete-role")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
                return NotFound("Role not found.");

            var result = await _roleManager.DeleteAsync(role);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new
            {
                message = "Role deleted successfully."
            });
        }

        
    }
}