using LearnAngularWebAPI.DataContext;
using LearnAngularWebAPI.Services;
using LearnAngularWebAPI.Services.Interface;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//this is for authenication certificate error:-   zone.js:2707          POST https://localhost:7288/api/Employee/CreateUser net::ERR_CERT_AUTHORITY_INVALID
builder.Services.AddAuthentication(
        CertificateAuthenticationDefaults.AuthenticationScheme)
        .AddCertificate();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LearnAngular", Version = "v1" });
    //  c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                     }
                 }

    );
});


#region JWT Token

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = "http://localhost:7288",
        ValidIssuer = "http://localhost:7288",
        ClockSkew = TimeSpan.Zero,// it forces tokens to expire exactly at token expiration time instead of 5 minutes later
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("thisismysecretkey"))
    };
    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            // Call this to skip the default logic and avoid using the default response
            context.HandleResponse();

            // Write to the response in any way you wish
            context.Response.StatusCode = 401;
            context.Response.Headers.Append("Message", "You are not authorized!");
            context.Response.Headers.Append("StatusCode", "401");
            // context.Response.Headers.remo .Remove("date");
            //context.Response.Headers.Remove("server");
            await context.Response.WriteAsync("You are not authorized!");

        }
    };
});

#endregion

var connectionString = builder.Configuration.GetConnectionString("ParkShareIdentityContextConnection");
builder.Services.AddDbContext<DataContext2>(x => x.UseSqlServer(connectionString));
builder.Services.AddScoped<IEmployeeData, EmployeeService>();
builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();

app.Run();



public class SwaggerFileOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var fileUploadMime = "multipart/form-data";
        if (operation.RequestBody == null || !operation.RequestBody.Content.Any(x => x.Key.Equals(fileUploadMime, StringComparison.InvariantCultureIgnoreCase)))
            return;

        var fileParams = context.MethodInfo.GetParameters().Where(p => p.ParameterType == typeof(IFormFile));
        operation.RequestBody.Content[fileUploadMime].Schema.Properties =
            fileParams.ToDictionary(k => k.Name, v => new OpenApiSchema()
            {
                Type = "string",
                Format = "binary"
            });
    }
}