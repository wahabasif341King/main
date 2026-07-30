using Microsoft.IdentityModel.Tokens; // Symmetric Security Key and SigningCredentials is mein hotey hain
                                      // Ye dono Token ko Secure Bananey ke liye use hotey hian.

using System.IdentityModel.Tokens.Jwt; // JwtSecurityToken and JwtSecurityHandler is mein hotey hain.
                                       // Ye Token Banatey hain or Read krtey Hain.

using System.Security.Claims; // Claim and ClaimTypes Provide krtey hain.
                              // Claim User ke baarey mein information. Jwt ke andar ye information save hoti hai.

using System.Text; // Ye Encoding ke liye hai. Ye string ko Bytes mein convert krta hai kiun ke Secret key Bytes mein cahhiye hoti hai.

public class TokenService
{
    private readonly IConfiguration _config;

    public TokenService (IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(User user)
    {
        var Claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id!.ToString()), // int ko string mein convert kr diya gya hai.(Imp)
            new Claim(ClaimTypes.Surname, user.Username!),
            new Claim(ClaimTypes.Role, user.Role!)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)); // Ye appsettings.json file se Jwt ke andar Key ko Get krta hai.
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var Token = new JwtSecurityToken(
          issuer: _config["Jwt:Issuer"], // Issuer matlab Token kis ne banaya.
          audience: _config["Jwt:Audience"], // Audience matlab ye token kis ke liy banaya hai.
          claims: Claims, // Us user ki information
          expires: DateTime.Now.AddMinutes(double.Parse(_config["Jwt:ExpiryMinutes"]!)), // Token kitni deer baad expire hoga.
          signingCredentials: creds  // Ye batata hai ke kis key se sign krna hai.
        );

        return new JwtSecurityTokenHandler().WriteToken(Token); // Yahan Jwt Object ko string ki form mein convert kr diya jata hai
    }
}