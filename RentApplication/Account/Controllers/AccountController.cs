using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AccountManaging.Models.Bindings;
using AccountManaging.Models.Entities;
using AccountManaging.Persistance.UnitOfWork;
using AccountManaging.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using AccountManaging.User;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using AccountManaging.Helpers;
using Microsoft.AspNetCore.SignalR;
using AccountManaging.Hubs;

namespace AccountManaging.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private static object _lockObjectForAccounts = new object();
        private const string LocalLoginProvider = "Local";

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IEmailService _emailService;

        private readonly IHostingEnvironment _environment;
        private readonly IConfiguration _configuration;


        public AccountController(UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IHubContext<NotificationHub> hubContext,
            IHostingEnvironment environment,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _hubContext = hubContext;
            _environment = environment;
            _configuration = configuration;
        }


        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [Route("GetUsers")]
        public IActionResult GetUsers()
        {
            List<UserRoleBindingModel> userRoles = new List<UserRoleBindingModel>();

            IEnumerable<ApplicationUser> users = _userManager.Users;
            foreach (ApplicationUser user in users)
            {
                userRoles.Add(new UserRoleBindingModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateOfBirth = user.DateOfBirth.Value.Date,
                });
            }

            foreach (UserRoleBindingModel userRole in userRoles)
            {
                userRole.Role = _userManager.GetRolesAsync(new ApplicationUser { Id = userRole.Id }).Result.FirstOrDefault();
            }

            return Ok(userRoles);
        }


        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [Route("AccountForApproval")]
        public IActionResult AccountForApproval()
        {
            return Ok(_unitOfWork.AccountsForApproval.GetAll().Count());
        }


        [HttpGet]
        [Route("ServiceForApproval")]
        [Authorize(Roles = "Administrator")]
        public IActionResult ServiceForApproval()
        {
            return Ok(_unitOfWork.ServicesForApproval.GetAll().Count());
        }


        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [Route("GetManagers")]
        public IActionResult GetManagers()
        {
            List<ManagerBindingModel> managers = new List<ManagerBindingModel>();
            List<ManagerBindingModel> managerUsers = new List<ManagerBindingModel>();

            IEnumerable<ApplicationUser> users = _userManager.Users;
            foreach (ApplicationUser user in users)
            {
                managerUsers.Add(new ManagerBindingModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateOfBirth = user.DateOfBirth.Value,
                });
            }

            foreach (ManagerBindingModel manager in managerUsers)
            {
                if (_userManager.IsInRoleAsync(new ApplicationUser { Id = manager.Id }, "Manager").Result)
                {
                    if (_unitOfWork.BanedManagers.Find(bm => bm.User.Id == manager.Id).Any())
                    {
                        manager.IsBaned = true;
                    }
                    managers.Add(manager);
                }
            }

            return Ok(managers);
        }


        [HttpPut]
        [Authorize(Roles = "Administrator")]
        [Route("ChangeRole")]
        public async Task<IActionResult> ChangeRole([FromQuery] string userId, RoleBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            else
            {
                ApplicationUser applicationUser =  _userManager.FindByIdAsync(userId).Result;
                if (_userManager.IsInRoleAsync(applicationUser, model.Role).Result)
                {
                    return Ok();
                }
                else
                {
                    lock(_lockObjectForAccounts)
                    {
                        string oldRole = _userManager.GetRolesAsync(applicationUser).Result.FirstOrDefault();
                        _userManager.AddToRoleAsync(applicationUser, model.Role);
                        _userManager.RemoveFromRoleAsync(applicationUser, oldRole);
                    }
                }

                return Ok();
            }
        }


        [HttpPut]
        [Authorize(Roles = "Administrator")]
        [Route("ChangeManagerBan")]
        public IActionResult ChangeManagerBan([FromForm] ManagerIdBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                BadRequest();
            }

            lock (_unitOfWork)
            {
                BanedManager banedManager = _unitOfWork.BanedManagers.Find(bm => bm.User.Id == model.ManagerId).FirstOrDefault();
                if (banedManager == null)
                {
                    _unitOfWork.BanedManagers.Add(new BanedManager() { User = _userManager.FindByIdAsync(model.ManagerId).Result });
                }
                else
                {
                    _unitOfWork.BanedManagers.Remove(banedManager);
                }

                _unitOfWork.Complete();
            }

            List<ManagerBindingModel> managers = new List<ManagerBindingModel>();
            List<ManagerBindingModel> managerUsers = new List<ManagerBindingModel>();

            IEnumerable<ApplicationUser> users = _userManager.Users;
            foreach (ApplicationUser user in users)
            {
                managerUsers.Add(new ManagerBindingModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateOfBirth = user.DateOfBirth.Value,
                });
            }

            foreach (ManagerBindingModel manager in managerUsers)
            {
                if (_userManager.IsInRoleAsync(new ApplicationUser { Id = manager.Id }, "Manager").Result)
                {
                    if (_unitOfWork.BanedManagers.Find(bm => bm.User.Id == manager.Id).Any())
                    {
                        manager.IsBaned = true;
                    }
                    managers.Add(manager);
                }
            }

            return Ok(managers);
        }

     
        [HttpPost]
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (await _userManager.FindByEmailAsync(model.Email) != null)
                {
                    return BadRequest("Username already exists.");
                }

                ApplicationUser user = new ApplicationUser()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PasswordHash = ApplicationUser.HashPassword(model.Password),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DocumentImage = "",
                    DateOfBirth = model.DateOfBirth,
                    IsApproved = false
                };

                IdentityResult addUserResult = await _userManager.CreateAsync(user, model.Password);
                if (!addUserResult.Succeeded)
                {
                    return GetErrorResult(addUserResult);
                }

                IdentityResult addRoleResult = await _userManager.AddToRoleAsync(user, "Client");
                if (!addRoleResult.Succeeded)
                {
                    return GetErrorResult(addRoleResult);
                }

            }
            catch (DBConcurrencyException)
            {
                return BadRequest();
            }

            await _hubContext.Clients.All.SendAsync("newUserAccountToApprove", _unitOfWork.AccountsForApproval.Count());

            return Ok();
        }


        [HttpPost]
        [Route("FinishAccount")]
        public async Task<IActionResult> FinishAccount(IFormFile image)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user.IsApproved)
            {
                return BadRequest();
            }

            if (_unitOfWork.AccountsForApproval.Find(u => u.UserId == user.Id).Any())
            {
                return BadRequest();
            }

            if (image == null || image.Length == 0)
            {
                return BadRequest();
            }

            ImageHelper.UploadImageToServer(_environment.WebRootPath, image);
            user.DocumentImage = image.FileName;

            IdentityResult addUserDocumentImageResult = await _userManager.UpdateAsync(user);
            if (!addUserDocumentImageResult.Succeeded)
            {
                return GetErrorResult(addUserDocumentImageResult);
            }

            _unitOfWork.AccountsForApproval.Add(new AccountForApproval() { UserId = user.Id });
            _unitOfWork.Complete();

            await _hubContext.Clients.All.SendAsync("newUserAccountToApprove", _unitOfWork.AccountsForApproval.Count());

            return Ok();
        }


        [HttpGet]
        [Route("IsUserApproved")]
        public async Task<IActionResult> IsUserApproved()
        {
            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user.IsApproved || _unitOfWork.AccountsForApproval.Find(u => u.UserId == user.Id).Any())
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }


        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [Route("AccountsForApproval")]
        public IActionResult AccountsForApproval()
        {
            List<UsersForApprovesViewModel> usersAccountsForApproves = new List<UsersForApprovesViewModel>();

            IEnumerable<AccountForApproval> accountsForApproves = _unitOfWork.AccountsForApproval.GetAll();
            foreach (AccountForApproval accountForApprove in accountsForApproves)
            {
                usersAccountsForApproves.Add(new UsersForApprovesViewModel() { Id = accountForApprove.Id, User = _userManager.FindByIdAsync(accountForApprove.UserId).Result });
            }

            return Ok(usersAccountsForApproves);
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("UserDocumentImage")]
        public IActionResult UserDocumentImage([FromQuery] string imageId)
        {
            Stream readStream = ImageHelper.ReadImageFromServer(_environment.WebRootPath, imageId);

            return File(readStream, "image/jpeg");
        }


        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("ApproveAccount")]
        public async Task<IActionResult> ApproveAccount([FromBody] long id)
        {
            if (!_unitOfWork.AccountsForApproval.Find(a => a.Id == id).Any())
            {
                return BadRequest("Bad request. Id don't exists.");
            }

            AccountForApproval accountForApprove = _unitOfWork.AccountsForApproval.Find(a => a.Id == id).First();
            ApplicationUser user = await _userManager.FindByIdAsync(accountForApprove.UserId);
            user.IsApproved = true;
            _emailService.SendMail("Account approved", "Your account is approved, now you can rent vehicle.", user.Email);

            IdentityResult updateUserResult = await _userManager.UpdateAsync(user);
            if (!updateUserResult.Succeeded)
            {
                return GetErrorResult(updateUserResult);
            }

            try
            {
                lock (_lockObjectForAccounts)
                {
                    _unitOfWork.AccountsForApproval.Remove(accountForApprove);
                    _unitOfWork.Complete();
                }
            }
            catch (DBConcurrencyException)
            {
                return NotFound();
            }

            return Ok();
        }


        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("RejectAccount")]
        public async Task<IActionResult> RejectAccount([FromBody] long id)
        {
            if (!_unitOfWork.AccountsForApproval.Find(a => a.Id == id).Any())
            {
                return BadRequest("Bad request. Id don't exists.");
            }

            AccountForApproval accountForApprove = _unitOfWork.AccountsForApproval.Find(a => a.Id == id).First();
            ApplicationUser user = await _userManager.FindByIdAsync(accountForApprove.UserId);
            _emailService.SendMail("Account rejected", "Your account is rejected, upload new document image.", user.Email);
            ImageHelper.DeleteImage(_environment.WebRootPath, user.DocumentImage);
            user.DocumentImage = "";

            IdentityResult updateUserResult = await _userManager.UpdateAsync(user);
            if (!updateUserResult.Succeeded)
            {
                return GetErrorResult(updateUserResult);
            }

            try
            {
                lock (_lockObjectForAccounts)
                {
                    _unitOfWork.AccountsForApproval.Remove(accountForApprove);
                    _unitOfWork.Complete();
                }
            }
            catch (DBConcurrencyException)
            {
                return NotFound();
            }

            return Ok();
        }

        private IActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }

                if (ModelState.IsValid)
                {
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        #region Helpers
        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirst(ClaimTypes.Name).Value
                };
            }
        }
        #endregion

        #region To-Check

        [Authorize]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);
            return new UserInfoViewModel
            {
                Email = User.Identity.Name,
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            IdentityResult result = await _userManager.ChangePasswordAsync(new ApplicationUser { Id = User.FindFirstValue(ClaimTypes.NameIdentifier) }, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        [Route("SetPassword")]
        public async Task<IActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                IdentityResult result = await _userManager.AddPasswordAsync(new ApplicationUser { Id = User.FindFirstValue(ClaimTypes.NameIdentifier) }, model.NewPassword);
                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }

                return Ok();
            }
        }

        [Route("RemoveLogin")]
        public async Task<IActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await _userManager.RemovePasswordAsync(new ApplicationUser { Id = User.FindFirstValue(ClaimTypes.NameIdentifier) });
            }
            else
            {
                result = await _userManager.RemoveLoginAsync(new ApplicationUser { Id = User.FindFirstValue(ClaimTypes.NameIdentifier) }, model.LoginProvider, model.ProviderKey);
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }


        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();
            /*
            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }
            */
            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }
        #endregion

        #region ExternaLogin

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            /*
            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);
            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);
            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await _userManager.AddLoginAsync(new RAIdentityUser { Id = User.FindFirstValue(ClaimTypes.NameIdentifier)}, new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey, externalData.UserName));
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            */
            return Ok();
        }


        /*
        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]()
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            if (externalLogin.LoginProvider != provider)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return new ChallengeResult(provider, this);
            }

            RAIdentityUser user = await _userManager.FindByLoginAsync(externalLogin.LoginProvider, externalLogin.ProviderKey);

            bool hasRegistered = user != null;
            if (hasRegistered)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsPrincipal identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, identity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsPrincipal identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, identity);
            }

            return Ok();
        }
        */


        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = HttpContext.Authentication.GetAuthenticationSchemes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                //state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.DisplayName,
                    Url = Url.RouteUrl("ExternalLogin", new
                    {
                        provider = description.AuthenticationScheme,
                        response_type = "token",
                        //client_id = Startup.PublicClientId,
                        //redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        //state = state
                    }),
                    State = "state"
                };
                logins.Add(login);
            }

            return logins;
        }


        // POST api/Account/RegisterExternal
        //[OverrideAuthentication]
        //[HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            /*
            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            */
            ApplicationUser user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            //result = await _userManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }
        #endregion
    }
}