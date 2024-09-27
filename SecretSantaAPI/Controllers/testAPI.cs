using Business;
using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecretSantaAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly Random _random = new Random();

        // Inject the IUserService into the controller
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/users/paginated
        [HttpGet("Paginated")]
        public async Task<ActionResult<List<User>>> GetPaginatedUsers( int limit = 5,  int offset = 0)
        {
            var users = await _userService.GetPaginatedUsersAsync(limit, offset);
            return Ok(users);
        }

        // GET: api/users/active
        [HttpGet("Active")]
        public async Task<ActionResult<List<User>>> GetAllActiveUsers( int limit = 5, int offset = 0)
        {
            var users = await _userService.GetAllActiveUsersAsync();
            return Ok(users);
        }

        [HttpGet("ById")]
        public async Task<ActionResult<User>> GetUsersById(int id)
        {
            var user = await _userService.GetUsersByIdAsync(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found."); 
            }
            return Ok(user);
        }
        // GET: api/users/getNumbers
        [HttpGet("GetNumbers")]
        public ActionResult GetNumbers()
        {
            var response = new
            {
                Message = "Current date",
                Date = DateTime.UtcNow.ToString("yyyy/MM/dd")
            };
            return Ok(response);
        }
        
        // POST: api/users/calculate
        [HttpPost("Calculate")]
        public ActionResult Post([FromBody] MathRequestDTO request)
        {
            if (request == null || request.Symbol == default)
            {
                return BadRequest("Invalid request data.");
            }

            double randNum1 = _random.Next(0, 100);
            double randNum2 = _random.Next(0, 100);
            double output;

            switch (request.Symbol)
            {
                case '+':
                    output = randNum1 + randNum2;
                    break;
                case '-':
                    output = randNum1 - randNum2;
                    break;
                case '*':
                    output = randNum1 * randNum2;
                    break;
                case '/':
                    if (randNum2 == 0)
                    {
                        return BadRequest("Division by zero is not allowed.");
                    }
                    output = randNum1 / randNum2;
                    break;
                default:
                    return BadRequest("Invalid operation symbol. Please use '+', '-', '*', or '/'");
            }

            return Ok(new
            {
                Message = "Operation completed successfully.",
                Numbers = new { Num1 = randNum1, Num2 = randNum2 },
                Result = output
            });
        }

        // PUT: api/users/update
        [HttpPut("Update")]
        public ActionResult Update([FromBody] MathRequestDTO request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request data.");
            }

            // Add your logic for updating values here
            return Ok("Values updated successfully");
        }

        // DELETE: api/users/delete
        [HttpDelete("Delete")]
        public ActionResult Delete()
        {
            // Add your logic for deleting/resetting values here
            return Ok("Values reset successfully");
        }
    }
}
