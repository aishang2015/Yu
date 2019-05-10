using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Yu.Core.Constants;
using Yu.Core.Extensions;

namespace Yu.Core.Jwt
{
    public class JwtFactory : IJwtFactory
    {
        private readonly JwtOption _jwtOption;

        public JwtFactory(IOptions<JwtOption> jwtOption)
        {
            _jwtOption = jwtOption.Value;
        }

        public ClaimsPrincipal DecodeJwtToken(string token)
        {
            return new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
            {
                ValidIssuer = _jwtOption.Issuer,
                ValidateIssuer = true,
                ValidAudience = _jwtOption.Audience,
                ValidateAudience = true,
                IssuerSigningKey = _jwtOption.SecurityKey,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false,
            }, out SecurityToken securityToken);
        }

        public string GenerateJwtToken(List<(string, string)> tuples)
        {
            var userClaims = new List<Claim>();
            foreach (var tuple in tuples)
            {
                userClaims.Add(new Claim(tuple.Item1, tuple.Item2));
            }
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtOption.Issuer,
                audience: _jwtOption.Audience,
                expires: _jwtOption.Expires,
                claims: userClaims,
                signingCredentials: _jwtOption.SigningCredentials);
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return token;
        }

        public string RefreshJwtToken(string oldToken)
        {
            // 解析旧token
            var claimsPrincipal = DecodeJwtToken(oldToken);

            // 取得旧token过期时间
            var oldExpires = claimsPrincipal.GetClaimValue(CustomClaimTypes.Expires);
            var expireDateTime = Utils.DateTimeUtil.GetDateTime(oldExpires);

            // TODO 比较时间判断是否能够刷新。

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtOption.Issuer,
                audience: _jwtOption.Audience,
                expires: _jwtOption.Expires,
                claims: claimsPrincipal.Claims,
                signingCredentials: _jwtOption.SigningCredentials);
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return token;
        }
    }
}
