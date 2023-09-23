﻿using Contracts.Identity;
using Microsoft.IdentityModel.Tokens;
using Shared.Configurations;
using Shared.Dtos.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _settings;

        public TokenService(JwtSettings settings)
        {
            this._settings = settings;
        }

        public TokenResponse GetToken(TokenRequest request)
        {
            var token = GenerateJwt();
            var result = new TokenResponse(token);
            return result;
        }

        private string GenerateJwt()
        {
            return GenerateEncryptedToken(GetSigningCredentials());
        }

        private string GenerateEncryptedToken(SigningCredentials signingCredentials)
        {
            var claims = new[]
            {
                new Claim("Role","Admin")
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: signingCredentials
                );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        private SigningCredentials GetSigningCredentials()
        {
            byte[] secret = Encoding.UTF8.GetBytes(_settings.Key);
            return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
        }
    }
}
