using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repo.IRepository;
using Services.IService;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    /*public class JwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(User user, long roleId)
        {
            var jwtSettings = _configuration.GetSection("Jwt");

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, roleId.ToString()),// Gán RoleId vào Token
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"]));

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }*/

    public class JwtService
    {
        /*private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public JwtService(IConfiguration configuration, IUserService agencyService)
        {
            _configuration = configuration;
            _userService = agencyService;
        }*/

        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository; // Gọi trực tiếp UserRepo thay vì UserService

        public JwtService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
        }

        public async Task<string> GenerateJwtTokenAsync(User user, long roleId)
        {
            var jwtSettings = _configuration.GetSection("Jwt");

            long? agencyId = await _userRepository.GetAgencyIdByUserId(user.UserId);

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, roleId.ToString())
        };

            if (agencyId.HasValue)
            {
                claims.Add(new Claim("AgencyId", agencyId.Value.ToString()));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"]));

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
