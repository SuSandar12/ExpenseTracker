using ExpenseTracker.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ExpenseTracker.API.Services
{
    public class AuthService
    {
        private readonly IConfiguration _config;
        private static List<User> _users = new();

        public AuthService(IConfiguration config)
        {
            _config = config;
        }

        public string Register(string username, string password)
        {
            var user = new User
            {
                Id = _users.Count + 1,
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };
            _users.Add(user);
            return GenerateToken(user);
        }

        public string Login(string username, string password)
        {
            var user = _users.FirstOrDefault(u => u.Username == username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                throw new Exception("Invalid credentials");
            return GenerateToken(user);
        }

        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
