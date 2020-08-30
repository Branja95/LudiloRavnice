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
        [Route("FindUser")]
        public async Task<IActionResult> FindUser(string userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(user);
            }
        }

        [HttpGet]
        [Route("IsManagerBanned")]
        public bool IsManagerBanned(string userId)
        {
            if (_unitOfWork.BanedManagers.Find(bm => bm.User.Id == userId).Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        [HttpGet]
        [Route("IsUserInRole")]
        public async Task<IActionResult> IsUserInRole(string userId, string role)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Ok(false);
            }
            else
            {
                if (_userManager.IsInRoleAsync(user, role).Result)
                {
                    return Ok(true);
                }
                else
                {
                    return Ok(false);
                }
            }
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
                    DocumentImage = user.DocumentImage,
                    DateOfBirth = user.DateOfBirth.Value.Date
                });
            }

            foreach (UserRoleBindingModel userRole in userRoles)
            {
                userRole.Role = _userManager.GetRolesAsync(new ApplicationUser { Id = userRole.Id }).Result.FirstOrDefault();
            }

            return Ok(userRoles);
        }

        [HttpGet]
        [Route("IsAccountApproved")]
        public IActionResult IsAccountApproved(string userId)
        {
            if (_unitOfWork.AccountsForApproval.Find(a => a.UserId == userId).Any())
            {
                return NotFound(false);
            }
            else
            {
                return Ok(true);
            }
        }


        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [Route("AccountForApproval")]
        public IActionResult AccountForApproval()
        {
            return Ok(_unitOfWork.AccountsForApproval.GetAll().Count());
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
                    DocumentImage = user.DocumentImage,
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
                ApplicationUser applicationUser = await _userManager.FindByIdAsync(userId);
                if (_userManager.IsInRoleAsync(applicationUser, model.Role).Result)
                {
                    return Ok();
                }
                else
                {
                    string oldRole = _userManager.GetRolesAsync(applicationUser).Result.FirstOrDefault();
                    await _userManager.AddToRoleAsync(applicationUser, model.Role);
                    await _userManager.RemoveFromRoleAsync(applicationUser, oldRole);
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
                    DocumentImage = user.DocumentImage,
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

            ImageHelper imageHelper = new ImageHelper();
            string fileName = await imageHelper.UploadImageToServer(_environment.WebRootPath, image);
            user.DocumentImage = fileName;

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

            await _hubContext.Clients.All.SendAsync("newUserAccountToApprove", _unitOfWork.AccountsForApproval.Count());
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

            await _hubContext.Clients.All.SendAsync("newUserAccountToApprove", _unitOfWork.AccountsForApproval.Count());
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
    }
}