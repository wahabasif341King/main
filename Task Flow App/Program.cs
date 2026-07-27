using Microsoft.AspNetCore.Authentication.JwtBearer; // YE Jwt Authentication ki properties le kr ata hai.
// IS ke baghair ap "JwtBearerDefaults.AuthenticationScheme" use nai kr saktey.

using Microsoft.IdentityModel.Tokens; // Ye Security wali library hai. Is mein "TokenValidationParameters" and "SymmetricSecurityKey" hain.

using System.Text;

var builder = WebApplication.CreateBuilder(args); // Builder bana raha jis ne Project manage krna sara

builder.Services.AddControllers(); // MVC Views nahi chahiye, sirf API Controllers
builder.Services.AddSingleton<MongoDBContext>(); // "AddSingleton" matlab Application Start hotey hi ek Object bane ga. 1000 Requests bhi a jayein Object ek hi rahe ga.
builder.Services.AddScoped<TokenService>(); // "AddScoped" ka matlab har Request ke liye new Object.

// JWT Authentication setup
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // DefaultAuthentication Jwt Token se hogi only.
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Agr Token nai miley ga to Jwt ka Error de do.
})
.AddJwtBearer(options => // Jwt Token ko kaisy Verify krna hai? Ye bataya gya hai is mein.
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

var app = builder.Build(); // Application build ho gayi hai saari.

app.UseHttpsRedirection(); // Agr use http:// se aye to use ko Redirect kr do https:// pr Security ke
app.UseRequestLogging();   // middleware add ho gya
app.UseAuthentication(); // Authorization se pehle hona zaroori hai. Is mein har request pehle Token check karey gi.
app.UseAuthorization(); // IS se Role Check hota.

app.MapControllers(); // Ye Controllers ko Route se connect krta hai.

app.Run("http://localhost:8005"); // App Start krta hai.