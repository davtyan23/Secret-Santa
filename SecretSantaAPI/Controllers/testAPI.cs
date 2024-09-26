using Business;
using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;

namespace SecretSantaAPI.Controllers
{
    [ApiController]
    public class UsersController : ControllerBase
    {

        private char symbol { get; set; }
        Random random = new Random();
        SecretSanta secretSanta = new SecretSanta();


        // GET: api/users
   
        [Route("GetNumbers")]
        [HttpGet]
        public ActionResult GetNumbers()
        {
            var response = new
            {
                Message = "Current date",
                Date = DateTime.UtcNow.ToString("yyyy/MM/dd")
            };
            return Ok(response);
        }

        // POST

        [Route("Calculate")]
        [HttpPost]
        public ActionResult Post([FromBody] MathRequestDTO request)
        {
            double randNum1 = random.Next(0, 100);
            double randNum2 = random.Next(0, 100);
            double output;

            symbol = request.Symbol;

            switch (symbol)
            {
                case '+':
                    output = randNum1 + randNum2;
                    Console.WriteLine($"SUM is {output}");
                    break;
                case '-':
                    output = randNum1 - randNum2;
                    Console.WriteLine($"DIFFERENCE is {output}");
                    break;
                case '*':
                    output = randNum1 * randNum2;
                    Console.WriteLine($"PRODUCT is {output}");
                    break;
                case '/':
                    try
                    {
                        if (randNum2 == 0)
                        {
                            return BadRequest("Division by zero is not allowed.");
                        }
                        output = randNum1 / randNum2;
                        Console.WriteLine($"QUOTIENT is {output}");
                    }
                    catch (DivideByZeroException ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                        return BadRequest("Division by zero is not allowed.");
                    }
                    break;
                default:
                    return BadRequest("Invalid operation symbol.");
            }

            int a = request.Number1;
            int b = request.Number2;

            return Ok($"Operation is done successfully. The numbers involved: {a} and {b}");
        }

        // PUT

        [Route("Update")]
        [HttpPut]
        public ActionResult Put([FromBody] MathRequestDTO request)
        {
            int a = request.Number1;
            int b = request.Number2;
            return Ok("Values updated successfully");
        }

        // DELETE
        [Route("Delete")]
        [HttpDelete]
        public ActionResult Delete()
        {
            return Ok("Values reset successfully");
        }

       
    }
}
