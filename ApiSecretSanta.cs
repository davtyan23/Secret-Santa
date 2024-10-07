using Newtonsoft.Json;


[Route("api/[controller]")]
[ApiController]

public class UsersController : ControllerBase
{
    private char simbol { get; set; }
    Random random = new Random();

    // GET: 

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
    
    // POST: 
    [HttpPost]
    public ActionResult Post([FromBody] MathRequest request)
    {

    public double randNum1 = Random.Next(0, 100);
    public double randNum2 = Random.Next(0, 100);
    double output;

    switch (symbol)
    {
    case '+':
        output = randNum1 + randNum2;
        console.WriteLine($"SUM is {output}");
        break;

    case "-":
        output = randNum1 - randNum2;
        console.WriteLine($"DIFFERENCE is {output} ");
        break;
    case "*":
        output = randNum1* randnum2;
    console.WriteLine($"PRODUCT is {output} ");
        break;
    case "/":
        try
        {
            if (randNum2 == 0)
            {
                throw new DivisionLogicException();
                break;
            }
output = randNum1 / randNum2;
Console.WriteLine($"QUOTIENT is {output} ");

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
      a = request.Number1;
      b = request.Number2;

return Ok($"Operation is done successfully the numbers involved {a} and {b}"); 
         }

    // PUT: api/users
    [HttpPut]
public ActionResult Put([FromBody] MathRequest request)
{
    a = request.Number1;
    b = request.Number2;
    return Ok("Values updated successfully");
}

// DELETE: api/users
[HttpDelete]
public ActionResult Delete()
{
    a = 0;
    b = 0;
    return Ok("Values reset successfully");
}

public class MathRequest
{
    public int Number1 { get; set; }
    public int Number2 { get; set; }
}
}
