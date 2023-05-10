using DLL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UsersAPI.Services;
using UsersAPI.Services.EntityServices.DI;

namespace UsersAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly IUserGroupService _userGroupService;
        private readonly IUserStateService _userStateService;
        private readonly HashService _hashService;

        public UsersController(IUserService userService,
            HashService hashService,
            IUserGroupService userGroupService,
            IUserStateService userStateService)
        {
            _userService = userService;
            _hashService = hashService;
            _userGroupService = userGroupService;
            _userStateService = userStateService;
        }

        [HttpGet("get_all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetManyAsync(u => u.State.State == DLL.Entities.State.Active);
            if (users.Count() == 0)
                return new JsonResult(new { message = "no users yet.." });
            return new JsonResult(users);
        }

        [HttpGet("get_all_paged")]
        public async Task<IActionResult> GetAllUsers(int pageNumber, int pageSize)
        {
            var users = await _userService.GetManyAsync(u => u.State.State == DLL.Entities.State.Active);
            if (users.Count() == 0)
                return new JsonResult(new { message = "no users yet.." });
            var pagedList = PagedList<User>.ToPagedList(users, pageNumber, pageSize);
            var metadata = new
            {
                pagedList.TotalCount,
                pagedList.PageSize,
                pagedList.CurrentPage,
                pagedList.TotalPages,
                pagedList.HasNext,
                pagedList.HasPrevious
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            return new JsonResult(pagedList);
        }


        [HttpGet("get_by_id")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return new JsonResult(new { message = $"no such users with id: {id}" });
            return new JsonResult(user);
        }


        [HttpPost("create")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (user == null)
                return new JsonResult(new { message = "please, specify user to add" });

            if (user.Group.Role == Role.Admin)
            {
                var foundAdmin = await _userService.GetManyAsync(u => u.Group.Role == Role.Admin && u.State.State == State.Active);
                if (foundAdmin?.Count() != 0)
                {
                    return new JsonResult(new
                    {
                        message = $"cannot create more than one admin"
                    });
                }
            }
            
            user.State = user.State ?? new UserState(State.Active, "default");
            user.Group = user.Group ?? new UserGroup(Role.User, "default");

            user.Password = _hashService.Hash(user.Password);
            var createdLessFiveSecAgo = await _userService
                .GetManyAsync(u => u.Login == user.Login && DateTime.UtcNow - u.CreationDate <= TimeSpan.FromSeconds(5) && u.State.State == State.Active);

            if (createdLessFiveSecAgo.Any())
            {
                var longestToWait = createdLessFiveSecAgo.Min(u => (DateTime.UtcNow - u.CreationDate).Seconds);
                return new JsonResult(new
                {
                    message = $"cannot create user with login {user.Login}, " +
                    $"please, waite for {5 - longestToWait} seconds and try again"
                });
            }
            user.Id = 0;
            user.CreationDate = DateTime.UtcNow;
            await _userService.CreateAsync(user);
            return new JsonResult(new { message = $"user with id: {user.Id} added" });
        }


        [HttpGet("remove_by_id")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveUser(int id)
        {
            var foundUser = await _userService.GetByIdAsync(id);
            if (foundUser == null)
            {
                return new JsonResult(new
                {
                    message = $"no users with id: {id}"
                });
            }
            var fetchedState = await _userStateService.GetOrCreateAsync(State.Blocked);
            if (fetchedState == null)
            {
                return new JsonResult(new
                {
                    message = $"server error"
                });
            }
            if(foundUser.State.State == State.Blocked)
            {
                return new JsonResult(new
                {
                    message = $"this user has already been deleted"
                });
            }
            foundUser.State = fetchedState;
            await _userService.UpdateAsync(foundUser);
            return new JsonResult(new
            {
                message = $"user with id: {id} was successfully deleted"
            });
        }


    }
}
