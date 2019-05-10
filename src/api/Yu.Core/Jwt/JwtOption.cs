using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace Yu.Core.Jwt
{
    public class JwtOption
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string SecretKey { get; set; }

        public double EffectiveTime { get; set; } // token有效期

        public double RefreshEffectiveTime { get; set; } // 利用token刷新token的有效期

        public DateTime Expires { get => DateTime.UtcNow.AddMinutes(EffectiveTime); } // token过期时间

        public SecurityKey SecurityKey { get => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey)); } // 根据key生成一个对称安全密钥

        public SigningCredentials SigningCredentials { get => new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256); } // 根据密钥生成签名

    }
}
