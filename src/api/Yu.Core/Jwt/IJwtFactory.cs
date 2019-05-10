using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Yu.Core.Jwt
{
    /// <summary>
    /// jwt抽象工厂
    /// </summary>
    public interface IJwtFactory
    {
        // 生成jwttoken
        string GenerateJwtToken(List<ValueTuple<string,string>> tuples);

        // 解析jwttoken
        ClaimsPrincipal DecodeJwtToken(string token);

        // 刷新jwttoken
        string RefreshJwtToken(string oldToken);
    }
}
