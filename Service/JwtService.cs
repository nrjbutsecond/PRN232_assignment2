using BO;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expirationMinutes;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // Đọc config từ appsettings.json
            _secretKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT SecretKey not configured");
            _issuer = _configuration["Jwt:Issuer"] ?? "FUNewsManagementAPI";
            _audience = _configuration["Jwt:Audience"] ?? "FUNewsManagementClient";
            _expirationMinutes = int.Parse(_configuration["Jwt:ExpireMinutes"] ?? "60");
        }

        /// <summary>
        /// Generate JWT token cho account
        /// Token chứa: AccountId, Email, Role
        /// </summary>
        public string GenerateToken(SystemAccount account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            // Tạo claims - thông tin lưu trong token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
                new Claim(ClaimTypes.Email, account.AccountEmail),
                new Claim(ClaimTypes.Name, account.AccountName),
                new Claim(ClaimTypes.Role, GetRoleName(account.AccountRole)),
                new Claim("AccountRole", account.AccountRole.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())


            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_expirationMinutes),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Validate JWT token
        /// </summary>
        public bool ValidateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Helper method: Convert role number sang role name
        /// </summary>
        private string GetRoleName(int role)
        {
            return role switch
            {
                0 => "Admin",
                1 => "Staff",
                2 => "Lecturer",
                _ => "Unknown"
            };
        }
    }
}