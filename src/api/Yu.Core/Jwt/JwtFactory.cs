using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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

        // 解析jwttoken
        public ClaimsPrincipal CanRefresh(string token)
        {
            ClaimsPrincipal claimsPrincipal;

            // 解析旧token
            try
            {
                claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
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
            catch
            {
                // token无法解析
                return null;
            }

            // 取得旧token过期时间
            var oldExpires = claimsPrincipal.GetClaimValue(CustomClaimTypes.Expires);
            var expireDateTime = Utils.DateTimeUtil.GetDateTime(oldExpires);

            //// 当超过刷新间或者token仍在有效期内的情况下不刷新token
            //if (expireDateTime.AddMinutes(_jwtOption.RefreshEffectiveTime) < DateTime.Now || expireDateTime > DateTime.Now)
            // 期限超过则不能刷新
            if (expireDateTime.AddMinutes(_jwtOption.RefreshEffectiveTime) < DateTime.Now)
            {
                return null;
            }

            return claimsPrincipal;
        }

        // 生成jwttoken
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

        // 生成jwttoken
        public string GenerateJwtToken(ClaimsPrincipal claimsPrincipal)
        {
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