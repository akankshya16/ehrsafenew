using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using UserData;

[Route("api/[controller]")]
[ApiController]
public class TokenController : ControllerBase
{
    private readonly MasterDB dbm;
    private readonly IAuthService authService;

    public TokenController(MasterDB _dbm, IAuthService _authService)
    {
        dbm = _dbm;
        authService = _authService;
    }
[HttpPost("signup")]
public async Task<IActionResult> SignUp([FromBody] User model)
{
   
    
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                                   .SelectMany(v => v.Errors)
                                   .Select(e => e.ErrorMessage)
                                   .ToList();
            return BadRequest(new { errors });
        }

       
        if (await dbm.Users.AnyAsync(u => u.Email == model.Email))
            return BadRequest(new { message = "Email already exists." });

        // Hashing the password before saving
        model.Password = authService.HashPassword(model.Password);

       
        dbm.Users.Add(model);
        await dbm.SaveChangesAsync();

        return Ok(new { message = "User registered successfully!" });
    
    
}


    [HttpGet("login")]
    public async Task<IActionResult> Login( string email, string Password)
    {
        var user = await dbm.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null || !authService.VerifyPassword(Password, user.Password))
            return Unauthorized("Invalid credentials");

        string token = authService.GenerateJwtToken(user);
        return Ok(new { message = "User logged in successfully", Token = token });
       
    }
}
