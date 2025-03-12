using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace E_Common
{
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;

        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["AccessTokenExpirationMinutes"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false // Không xác thực thời gian hết hạn của token
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        public string GetMacAddressAndConvertToHS256()
        {
            try
            {
                // Lấy danh sách tất cả các Network Interfaces
                var activeInterface = NetworkInterface.GetAllNetworkInterfaces()
                    .FirstOrDefault(ni => ni.OperationalStatus == OperationalStatus.Up &&
                                          ni.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                                          ni.NetworkInterfaceType != NetworkInterfaceType.Tunnel);

                // Nếu tìm được interface, lấy địa chỉ MAC
                if (activeInterface != null)
                {
                    var macAddress = string.Join(":", activeInterface.GetPhysicalAddress()
                        .GetAddressBytes()
                        .Select(b => b.ToString("X2"))); // Định dạng dạng MAC Address (XX:XX:XX:XX:XX:XX)

                    // Mã hóa HS256
                    var jwtSettings = _configuration.GetSection("Jwt");
                    var keyBytes = Encoding.ASCII.GetBytes(jwtSettings["Key"]);
                    using var hmac = new HMACSHA256(keyBytes);
                    var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(macAddress));

                    // Trả về chuỗi HS256
                    return string.Concat(hashBytes.Select(b => b.ToString("X2")));
                }

                return "NO.MAC"; // Không tìm thấy interface nào phù hợp
            }
            catch (Exception)
            {
                return "ERROR.MAC"; // Xử lý lỗi chung
            }
        }
    }
}