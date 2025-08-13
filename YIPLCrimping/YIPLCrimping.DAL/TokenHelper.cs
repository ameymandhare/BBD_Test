using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using YIPLCrimpingAPI.Models;

namespace YIPLCrimping.DAL
{
    public static class TokenHelper
    {
        public static string GenerateJwtToken(UserAccount user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("YourSuperSecretKeyHere"); // Use config in production

            var claims = new List<Claim>
            {
                new Claim("EmployeeId", user.Id.ToString()),
                new Claim("EmployeeCode", user.EmployeeId ?? ""),
                new Claim("PlantId", user.Plant.ToString()),
                new Claim("Email", user.Email ?? ""),
                new Claim("RoleCode", user.RoleCode.ToString() ?? ""),
                new Claim("RoleName", user.MRoleCode.RoleName ?? ""),
                new Claim(ClaimTypes.Name, $"{user.UserName}")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}