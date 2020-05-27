using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AccountManaging.Models.Bindings;
using AccountManaging.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AccountManaging.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<ApplicationUser> userManager,
            IConfiguration confiugration)
        {
            _userManager = userManager;
            _configuration = confiugration;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginBindingModel model)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]));
                SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                List<string> role = _userManager.GetRolesAsync(user).Result.ToList();
                Claim[] claims = new[] {
                     new Claim("role", role.FirstOrDefault()),
                     new Claim("unique_name", user.Email),
                     new Claim("nameid", user.Id)
                };

                JwtSecurityToken token = new JwtSecurityToken(
                    _configuration["Jwt:Site"],
                    _configuration["Jwt:Site"],
                    claims,
                    expires: DateTime.Now.AddMinutes(120),
                    signingCredentials: credentials
                );

                dynamic data = new ExpandoObject();
                data.access_token = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(data);
            }
            else
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }
        }

        [HttpPost]
        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }
    }
}