using LearnAngularWebAPI.DataContext;
using LearnAngularWebAPI.Model;
using LearnAngularWebAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LearnAngularWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeData _employee;
        private readonly DataContext2 _dataContext;
        public readonly IConfiguration _configuration;
        public EmployeeController(IEmployeeData employee, DataContext2 dataContext2, IConfiguration configuration)
        {

            _employee = employee;
            _dataContext = dataContext2;
            _configuration = configuration;
        }
        [HttpPost("CreateUser")]
        public IActionResult Create(UserViewModel userModel)
        {
            UserModel userModel1 = new UserModel();
            if (_dataContext.UserRegistration.Where(x=>x.Email==userModel.Email).FirstOrDefault()!=null)
            {
                return Ok("User Already Exist");
            }
            if(userModel.Email!="" && userModel.Phoneno!="" && userModel.Password != "")
            {
                var mydata = "Durgesh";
                var test = "Jay";
            userModel1.IsDelete = true;
            userModel1.IsActive = true;
            userModel1.Email = userModel.Email;
            userModel1.FirstName = userModel.FirstName;
            userModel1.LastName = userModel.LastName;
            userModel1.Phoneno = userModel.Phoneno;
            userModel1.Zipcode = userModel.Zipcode;
            userModel1.City = userModel.City;
            userModel1.Password=userModel.Password;
            _dataContext.UserRegistration.Add(userModel1);
            _dataContext.SaveChanges();
            return Ok("Success");
            }
            else
            {
                return Ok("wrong");
            }
        }
        private bool Test()
        {
            return true;
        }

        private bool TestData()
        {
            return true;
        }
        [HttpPost("Test")]
        public IActionResult test()
        {
            return Ok("success");
        }

        [HttpPost("Login")]
        public IActionResult LoginUser(LoginModel loginModel)
        {
            if (_dataContext.UserRegistration.Where(x=>x.Email==loginModel.Email && x.Password==loginModel.Password).FirstOrDefault()!=null)
            {
                var Token = GenerateJsonWebToken(loginModel);
                loginModel.Email = new JwtSecurityTokenHandler().WriteToken(Token);
                return Ok(loginModel.Email);
            }
            return Ok();
        }


        #region Generate Json Web Token By Using UserName and Password
        private JwtSecurityToken GenerateJsonWebToken(LoginModel loginModel)
        {
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, loginModel.Email),
                     new Claim(ClaimTypes.GivenName, loginModel.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
            var token = GetToken(authClaims);
            return token;
        }



        #endregion

        private JwtSecurityToken GetToken(IEnumerable<Claim> claims)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("thisismysecretkey"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            return new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddDays(15),
                signingCredentials: signinCredentials
            );

        }

       
        [HttpDelete("{id}")]
        [Authorize]
        public void Delete(int id)
        {
        }
    }
}
