using Business;
using Business.DTOs;
using DataAccess;
using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
        private readonly IRepository _repository;
        private readonly ILoggerAPI _logger;
        private readonly Random _random = new Random();

        // Inject the IUserService into the controller
        public UsersController(IUserService userService, IRepository repository,ILoggerAPI logger)
        {
            _userService = userService;
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<DataAccess.Models.User>>> GetAllUsers()
        {
            try
            {
                var users = await _repository.GetAllUsersAsync();
                return Ok(users); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }


        // GET: api/users/paginated
        [HttpGet("Paginated")]
        public async Task<ActionResult<List<DataAccess.Models.User>>> GetPaginatedUsers( int limit = 5,  int offset = 0)
        {
            var users = await _userService.GetPaginatedUsersAsync(limit, offset);
            return Ok(users);
           
        }

        // GET: api/users/active
        [HttpGet("Active")]
        public ActionResult<List<DataAccess.Models.User>> GetAllActiveUsers()
        {
            var users = _userService.GetAllActiveUsersAsync();
            return Ok(users);
        }

        [HttpGet("ById")]
        public async Task<ActionResult<DataAccess.Models.User>> GetUsersById(int id)
        {
            var user = await _userService.GetUsersByIdAsync(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found."); 
            }
            return Ok(user);
        }
        // POST: api/users/calculate

        [HttpPost("Deactivate")]
        public async Task DeactivateUserAsync(int id)
        {
            var user = await _userService.GetUsersByIdAsync(id);
            if (user != null)
            {
                user.IsActive = false;
                await _userService.UpdateUsersAsync(user);
            }

        }

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

       
    }
}
