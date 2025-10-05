using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using core8_nextjs_postgre.Entities;
using core8_nextjs_postgre.Helpers;
using core8_nextjs_postgre.Models;
using core8_nextjs_postgre.Models.dto;

namespace core8_nextjs_postgre.Services
{    
    public interface IAuthService {
        User SignupUser(User userdata, string passwd);
        User SignUser(string usrname, string pwd);
    }

    public class AuthService : IAuthService
    {
        private ApplicationDbContext _context;
        private readonly AppSettings _appSettings;

         IConfiguration config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .Build();

        public AuthService(ApplicationDbContext context,IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        public User SignupUser(User userdata, string passwd)
        {
            User xusermail = _context.Users.Where(c => c.Email == userdata.Email).FirstOrDefault();
            if (xusermail is not null) {
                throw new AppException("Email Address was already taken...");
            }

            User xusername = _context.Users.Where(c => c.UserName == userdata.UserName).FirstOrDefault();
            if (xusername is not null) {
                throw new AppException("Username was already taken...");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var xkey = config["Jwt:Key"];
            var key = Encoding.ASCII.GetBytes(xkey);

            // CREATE SECRET KEY FOR USER TOKEN===============
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userdata.Email)
                }),
                // Expires = DateTime.UtcNow.AddDays(7),
                Expires = DateTime.UtcNow.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var secret = tokenHandler.CreateToken(tokenDescriptor);
            var secretkey = tokenHandler.WriteToken(secret);

            userdata.Secretkey = secretkey.ToUpper();             
            userdata.Password = BCrypt.Net.BCrypt.HashPassword(passwd);
            userdata.Profilepic = "https://localhost:7292/users/pix.png";
            userdata.Roles="USER";

            _context.Users.Add(userdata);                
            _context.SaveChanges();
            return userdata;
        }

        public User SignUser(string usrname, string pwd)
        {
           try {
                    var xuser =  _context.Users.Where(c => c.UserName == usrname).FirstOrDefault();
                    if (xuser is not null) {
                        if (!BCrypt.Net.BCrypt.Verify(pwd, xuser.Password)) {
                            throw new AppException("Incorrect Password...");
                        }
                        if (xuser.Isactivated == 0) {
                            throw new AppException("Please activate your account, check your email client inbox and click or tap the Activate button.");
                        }
                        return xuser;
                    } else {
                        throw new AppException("Username not found, please register first...");
                    }
            } catch(AppException ex) {
                    throw new AppException(ex.Message);
            }            
        }
    }
}