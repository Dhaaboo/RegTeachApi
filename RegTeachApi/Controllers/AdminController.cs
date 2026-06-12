using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegTeachApi.Data;

namespace RegTeachApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : ControllerBase
    {
        private readonly APPDBC _db;

        public AdminController(APPDBC db)
        {
            _db = db;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _db.RegTeachUsers
                .Select(x => new
                {
                    x.Id,
                    x.Username,
                    x.Email,
                    x.FirstName,
                    x.LastName,
                    x.Role,
                    x.IsEmailVerified,
                    x.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult>
            GetUser(int id)
        {
            var user = await _db.RegTeachUsers
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.Username,
                    x.Email,
                    x.FirstName,
                    x.LastName,
                    x.Role,
                    x.IsEmailVerified,
                    x.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPut("users/{id}/role")]
        public async Task<IActionResult>
            ChangeRole(
            int id,
            string role)
        {
            var user =
                await _db.RegTeachUsers
                .FindAsync(id);

            if (user == null)
                return NotFound();

            user.Role = role;

            await _db.SaveChangesAsync();

            return Ok(
                $"Role changed to {role}");
        }

        [HttpPut("users/{id}/lock")]
        public async Task<IActionResult>
            LockUser(int id)
        {
            var user =
                await _db.RegTeachUsers
                .FindAsync(id);

            if (user == null)
                return NotFound();

            user.LockoutEnd =
                DateTime.UtcNow.AddYears(100);

            await _db.SaveChangesAsync();

            return Ok("User locked.");
        }

        [HttpPut("users/{id}/unlock")]
        public async Task<IActionResult>
            UnlockUser(int id)
        {
            var user =
                await _db.RegTeachUsers
                .FindAsync(id);

            if (user == null)
                return NotFound();

            user.LockoutEnd = null;
            user.FailedLoginAttempts = 0;

            await _db.SaveChangesAsync();

            return Ok("User unlocked.");
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult>
            DeleteUser(int id)
        {
            var user =
                await _db.RegTeachUsers
                .FindAsync(id);

            if (user == null)
                return NotFound();

            _db.RegTeachUsers.Remove(user);

            await _db.SaveChangesAsync();

            return Ok("User deleted.");
        }
    }
}
