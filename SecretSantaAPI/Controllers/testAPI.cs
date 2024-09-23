using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SecretSantaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private char symbol { get; set; }  
        Random random = new Random();

        // GET: api/users
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
        [HttpPost]
        public ActionResult Post([FromBody] MathRequest request)
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
        [HttpPut]
        public ActionResult Put([FromBody] MathRequest request)
        {
            int a = request.Number1;
            int b = request.Number2;
            return Ok("Values updated successfully");
        }

        // DELETE
        [HttpDelete]
        public ActionResult Delete()
        {
            return Ok("Values reset successfully");
        }

        public class MathRequest
        {
            public int Number1 { get; set; }
            public int Number2 { get; set; }
            public char Symbol { get; set; }  
        }
    }
}
