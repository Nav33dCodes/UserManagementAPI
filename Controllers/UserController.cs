using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.DTOs;
using UserManagementAPI.Services;

namespace UserManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();

            return Ok(users);
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser(
            CreateUserDto dto)
        {
            var user = await _userService.CreateUserAsync(dto);

            return CreatedAtAction(
                nameof(GetUserById),
                new { id = user.Id },
                user
            );
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> UpdateUser(
            int id,
            UpdateUserDto dto)
        {
            var user = await _userService.UpdateUserAsync(id, dto);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleted = await _userService.DeleteUserAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}