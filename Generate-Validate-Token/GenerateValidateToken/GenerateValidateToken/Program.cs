// See https://aka.ms/new-console-template for more information
using System;
using System.IO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;


string token = GenerateToken();
Console.WriteLine("Generated JWT Token:");
Console.WriteLine(token);
ValidateToken(token);

//Generate Token
static string GenerateToken()
{
    string privateKey = File.ReadAllText(@"../../../jwtRS256.key");
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
    return tokenHandler.WriteToken(token);
}

// Validate Token
static async void ValidateToken(string token)
{
    var client = new HttpClient();
    var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:8003/v1/conversion/word-to-pdf");
    request.Headers.Add("Authentication", $"Bearer {token}");
    var content = new MultipartFormDataContent();
    content.Add(new StreamContent(File.OpenRead(@"../../../SalesInvoice.docx")), "file", "SalesInvoice.docx");
    content.Add(new StringContent("{\"File\": \"file\",\"Password\": null,\"PreserveFormFields\": true,\"PdfComplaince\": \"PDF/A-1B\",\"EnableAccessibility\": false}"), "settings");
    request.Content = content;
    var response = await client.SendAsync(request);
    response.EnsureSuccessStatusCode();
    Console.WriteLine(await response.Content.ReadAsStringAsync());
}