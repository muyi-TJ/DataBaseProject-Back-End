using System;
using System.Collections.Generic;
using System.Security.Claims;
using Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Back_End.Controllers {
    public static class Token {
        private const string secretKey = "xybxlxytql"; //私钥

        public static string GetToken(TokenInfo M) {
            var jwtCreated =
                Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds + 5);
            var jwtCreatedOver =
                Math.Round((DateTime.UtcNow.AddHours(24) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds + 5);
            var payload = new Dictionary<string, dynamic>
               {
                    {"iat", jwtCreated},//非必须。issued at。 token创建时间，unix时间戳格式
                    {"exp", jwtCreatedOver},//非必须。expire 指定token的生命周期。unix时间戳格式
                    {"jti", M.jti},//非必须。JWT ID。针对当前token的唯一标识
                    {"id", M.id},//自定义字段，用户id
                    {"phont", M.phone},//自定义字段 用手机号
                    {"email", M.email},//自定义字段 邮箱
                    {"password", M.password},//自定义字段 密码
                };
            return Jwt.JsonWebToken.Encode(payload, secretKey, Jwt.JwtHashAlgorithm.HS256);
        }

        public static Dictionary<string, dynamic> VerifyToken(string token) {
            try {
                var data = JsonWebToken.DecodeToObject<Dictionary<string, object>>(token, Token.secretKey);
                return data;
            }
            catch (SignatureVerificationException) {
                // Given token is either expired or hashed with an unsupported algorithm.
                return null;
            }
        }
    }

    public class TokenInfo {
        public TokenInfo() {
            jti = DateTime.Now.ToString("yyyyMMddhhmmss");
            phone = "";
            preNumber = "";
            email = "";
            password = "";
        }
        public string jti { get; set; }
        public string id { get; set; }
        public string phone { get; set; }
        public string preNumber { get; set; }
        public string email { get; set; }
        public string password { get; set; }
    }
}
