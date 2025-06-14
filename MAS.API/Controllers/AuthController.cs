using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MAS.API.Data;
using MAS.API.Models.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Data;
using System.Reflection.Metadata.Ecma335;

namespace MAS.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]

    public class AuthController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly IConfiguration _config;


        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;

        }
        [HttpPost("login")]

        public async Task<IActionResult> Login(LoginDto loginDto)
        {

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null) return Unauthorized("Nieprawidłowy Email lub hasło, spróbuj ponownie");

            if (user.PasswordHash != loginDto.Password)
                return Unauthorized("Nieprawidłowy Email lub hasło, spróbuj ponownie");

            var token = GenerateJwtToken(user);


            return Ok(new { token, user.Role });
        }
        private string GenerateJwtToken(User user)
        {

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes("ToBardzoTajnyKluczKtoryPowinnienBycWConfigu123");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            }
                ),

                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)




            };

            var token = tokenHandler.CreateToken(tokenDescriptor);


            return tokenHandler.WriteToken(token);





        }
        [HttpPost("Register")]

        public async Task<IActionResult> Register(RegisterDto registerDto)

        {
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                return BadRequest("Taki email już istnieje");

            var user = new User()
            {
                Email = registerDto.Email,
                PasswordHash = registerDto.Password, // PODATNOŚĆ - brak hashowania!
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Role = "Patient",
                CreatedAt = DateTime.Now,
                IsEmailVerified = false
            };



            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Rejestracja przebiegła pomyślnie" });


        }





    }

    public class LoginDto

    {
        public string Email { get; set; }
        public string Password { get; set; }

    }

    public class RegisterDto
    {

        public string Email { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

    }
}