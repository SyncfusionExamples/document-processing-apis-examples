// See https://aka.ms/new-console-template for more information

using System;
using System.IO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

string privateKey = File.ReadAllText("jwtRS256.key");
RSA rsa = RSA.Create();
rsa.ImportFromPem(privateKey.ToCharArray());

var tokenHandler = new JwtSecurityTokenHandler();
var tokenDescriptor = new SecurityTokenDescriptor
{
    Issuer = "sample",
    Audience = "audience",
    Expires = DateTime.UtcNow.AddMinutes(30),
    SigningCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha512)
};

var token = tokenHandler.CreateToken(tokenDescriptor);
string tokenString = tokenHandler.WriteToken(token);

Console.WriteLine("Generated JWT Token:");
Console.WriteLine(tokenString);