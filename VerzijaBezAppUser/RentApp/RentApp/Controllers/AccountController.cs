using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using RentApp.Models;
using RentApp.Models.Entities;
using RentApp.Providers;
using RentApp.Results;
using RentApp.Persistance.UnitOfWork;
using RentApp.Hubs;
using System.Linq;
using System.Collections;
using RentApp.Helpers;
using RentApp.Services;

namespace RentApp.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private string validationErrorMessage = "";

        private readonly IUnitOfWork unitOfWork;
        private readonly ISMTPService SMTPService;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat,
            IUnitOfWork unitOfWork,
            ISMTPService SMTPService)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
            this.unitOfWork = unitOfWork;
            this.SMTPService = SMTPService; 
        }

        public ApplicationUserManager UserManager { get; private set; }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        // GET api/Account/GetUsers
        [HttpGet]
        [Route("GetUsers")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult GetUsers()
        {
            IEnumerable<RAIdentityUser> users = UserManager.Users;

            List<UserRoleBindingModel> usersRoles = new List<UserRoleBindingModel>();

            foreach(RAIdentityUser user in users)
            {
                usersRoles.Add(new UserRoleBindingModel() {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateOfBirth = user.DateOfBirth.Value,
                });
            }

            foreach (UserRoleBindingModel userR in usersRoles)
            {
                userR.Role = UserManager.GetRoles(userR.Id).FirstOrDefault();
            }

            return Ok(usersRoles);
        }

        // GET api/Account/GetManagers
        [HttpGet]
        [Route("GetManagers")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult GetManagers()
        {

            IEnumerable<RAIdentityUser> users = UserManager.Users;

            List<ManagerBindingModel> allUsers = new List<ManagerBindingModel>();

            List<ManagerBindingModel> managers = new List<ManagerBindingModel>();

            foreach (RAIdentityUser user in users)
            {
                allUsers.Add(new ManagerBindingModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateOfBirth = user.DateOfBirth.Value,
                });
            }

            foreach (ManagerBindingModel userM in allUsers)
            {
                if(UserManager.IsInRole(userM.Id, "Manager"))
                {
                    if(unitOfWork.BanedManagers.Find(bm => bm.User.Id == userM.Id).Count() > 0)
                    {
                        userM.IsBaned = true;
                    }
                    managers.Add(userM);
                }
            }

            return Ok(managers);
        }

        // PUT api/Account/ChangeRole
        [HttpPut]
        [Route("ChangeRole")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult ChangeRole([FromUri] string userId, RoleBindingModel model)
        {
            if(!ModelState.IsValid)
            {
                BadRequest();
            }

            if (UserManager.IsInRole(userId, model.Role))
            {
                return Ok("User already have that role.");
            }
            else
            {
                string oldRole = UserManager.GetRoles(userId).FirstOrDefault();
                UserManager.AddToRole(userId, model.Role);
                UserManager.RemoveFromRole(userId, oldRole);
            }

            return Ok("Role successfully changed.");
        }

        // PUT api/Account/ChangeManagerBan
        [HttpPut]
        [Route("ChangeManagerBan")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult ChangeManagerBan(ManagerIdBindingModel model)
        {
            if(ModelState.IsValid)
            {
                BadRequest();
            }

            BanedManager banedManager = unitOfWork.BanedManagers.Find(bm => bm.User.Id == model.ManagerId).FirstOrDefault();

            if (banedManager == null)
            {
                unitOfWork.BanedManagers.Add(new BanedManager() { User = UserManager.FindById(model.ManagerId) });
            }
            else
            {
                unitOfWork.BanedManagers.Remove(banedManager);
            }

            unitOfWork.Complete();

            return Ok("Ban successfully changed.");
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

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

        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

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

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
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
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            RAIdentityUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                   OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (UserManager.FindByEmail(model.Email) != null)
                {
                    return BadRequest("Username already exists.");
                }

                var user = new RAIdentityUser() { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, DocumentImage = "", DateOfBirth = model.DateOfBirth, IsApproved = false };
                user.PasswordHash = RAIdentityUser.HashPassword(model.Password);

                IdentityResult addUserResult = await UserManager.CreateAsync(user, model.Password);

                if (!addUserResult.Succeeded)
                {
                    return GetErrorResult(addUserResult);
                }

                IdentityResult addRoleResult = await UserManager.AddToRoleAsync(user.Id, "AppUser");

                if (!addRoleResult.Succeeded)
                {
                    return GetErrorResult(addRoleResult);
                }

            }
            catch(Exception e)
            {

            }
            

            return Ok("Account successfully created.");
        }

        // POST api/Account/FinishAccount
        [HttpPost]
        [Route("FinishAccount")]
        public async Task<IHttpActionResult> FinishAccount()
        {
            RAIdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user.IsApproved)
            {
                return BadRequest("User already approve account.");
            }

            if (unitOfWork.AccountsForApprove.Find(u => u.UserId == user.Id).Count() > 0)
            {
                return BadRequest("User already send request for approve account.");
            }

            HttpRequest httpRequest = HttpContext.Current.Request;

            if (!ImageHelper.ValidateImage(httpRequest.Files[0], out validationErrorMessage))
            {
                return BadRequest(validationErrorMessage);
            }

            user.DocumentImage = ImageHelper.SaveImageToServer(httpRequest.Files[0]);

            IdentityResult addUserDocumentImageResult = await UserManager.UpdateAsync(user);

            if (!addUserDocumentImageResult.Succeeded)
            {
                return GetErrorResult(addUserDocumentImageResult);
            }

            unitOfWork.AccountsForApprove.Add(new AccountForApprove() { UserId = user.Id });
            unitOfWork.Complete();

            NotificationHub.NewUserAccountToApprove(unitOfWork.AccountsForApprove.Count());

            return Ok("Request for aprrove account successfully created");
        }

        // GET api/Account/IsUserApproved
        [HttpGet]
        [Route("IsUserApproved")]
        public async Task<IHttpActionResult> IsUserApproved()
        {
            RAIdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user.IsApproved)
            {
                return Ok(true);
            }

            if (unitOfWork.AccountsForApprove.Find(u => u.UserId == user.Id).Count() > 0)
            {
                return Ok(true);
            }
            return Ok(false);
        }

        // GET api/Account/AccountsForApproval
        [HttpGet]
        [Route("AccountsForApproval")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult AccountsForApproval()
        {
            IEnumerable<AccountForApprove> accountsForApproves = unitOfWork.AccountsForApprove.GetAll();

            List<UsersForApprovesViewModel> usersAccountsForApproves = new List<UsersForApprovesViewModel>();

            foreach (AccountForApprove accountForApprove in accountsForApproves)
            {
                usersAccountsForApproves.Add(new UsersForApprovesViewModel() { Id = accountForApprove.Id, User = UserManager.FindById(accountForApprove.UserId) });
            }

            return Ok(usersAccountsForApproves);
        }

        // GET api/Account/UserDocumentImage?imageId=5
        [HttpGet]
        [Route("UserDocumentImage")]
        [AllowAnonymous]
        public HttpResponseMessage UserDocumentImage([FromUri] string imageId)
        {
           return ImageHelper.LoadImage(imageId);
        }

        // Post api/Account/AccountsForApproval
        [HttpPost]
        [Route("ApproveAccount")]
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> ApproveAccount([FromBody]long id)
        {

            if(unitOfWork.AccountsForApprove.Find(a => a.Id == id).Count() == 0)
            {
                return BadRequest("Bad request. Id don't exists.");
            }

            AccountForApprove accountForApprove = unitOfWork.AccountsForApprove.Find(a => a.Id == id).First();

            RAIdentityUser user = await UserManager.FindByIdAsync(accountForApprove.UserId);

            user.IsApproved = true;

            SMTPService.SendMail("Account approved", "Your account is approved, now you can rent vehicle.", user.Email);

            IdentityResult updateUserResult = await UserManager.UpdateAsync(user);

            if (!updateUserResult.Succeeded)
            {
                return GetErrorResult(updateUserResult);
            }

            unitOfWork.AccountsForApprove.Remove(accountForApprove);
            unitOfWork.Complete();

            return Ok("Account Successfully approved.");
        }

        // Post api/Account/AccountsForApproval
        [HttpPost]
        [Route("RejectAccount")]
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> RejectAccount([FromBody]long id)
        {

            if (unitOfWork.AccountsForApprove.Find(a => a.Id == id).Count() == 0)
            {
                return BadRequest("Bad request. Id don't exists.");
            }

            AccountForApprove accountForApprove = unitOfWork.AccountsForApprove.Find(a => a.Id == id).First();

            RAIdentityUser user = await UserManager.FindByIdAsync(accountForApprove.UserId);

            SMTPService.SendMail("Account rejected", "Your account is rejected, upload new document image.", user.Email);

            ImageHelper.DeleteImage(user.DocumentImage);

            user.DocumentImage = "";

            IdentityResult updateUserResult = await UserManager.UpdateAsync(user);

            if (!updateUserResult.Succeeded)
            {
                return GetErrorResult(updateUserResult);
            }

            unitOfWork.AccountsForApprove.Remove(accountForApprove);
            unitOfWork.Complete();

            return Ok("Account Successfully rejected.");
        }

        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new RAIdentityUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

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
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
