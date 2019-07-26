using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Yu.Core.Jwt
{
    /// <summary>
    /// jwt抽象工厂
    /// </summary>
    public interface IJwtFactory
    {
        // 生成jwttoken
        string GenerateJwtToken(List<ValueTuple<string, string>> tuples);

        // 生成jwttoken
        string GenerateJwtToken(ClaimsPrincipal claimsPrincipal);

        // 解析jwttoken
        ClaimsPrincipal CanRefresh(string token);

        // 保存token到缓存
        void StoreToken(string key, string token);

        // 删除用户token
        void RemoveToken(string key);
    }
}
