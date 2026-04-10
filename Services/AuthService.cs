using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks; // Added for Task
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore; // CRITICAL: Added for async EF Core extensions
using core8_nextjs_postgre.Entities;
using core8_nextjs_postgre.Helpers;
using core8_nextjs_postgre.Models;
using core8_nextjs_postgre.Models.dto;

namespace core8_nextjs_postgre.Services
{    
    public interface IAuthService {
        Task<User> SignupUser(User userdata, string passwd);
        Task<User> SignUser(string usrname, string pwd);
        Task<Role> getRolename(int id);
    }

    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly AppSettings _appSettings;
        private readonly IConfiguration _config; // Inject this instead of building manually

        public AuthService(
            ApplicationDbContext context,
            IOptions<AppSettings> appSettings,
            IConfiguration config) // Added IConfiguration via DI
        {
            _context = context;
            _appSettings = appSettings.Value;
            _config = config; 
        }

        public async Task<User> SignupUser(User userdata, string passwd)
        {
            // Changed .FirstOrDefault() to .FirstOrDefaultAsync()
            User xusermail = await _context.Users.FirstOrDefaultAsync(c => c.Email == userdata.Email);
            if (xusermail is not null) {
                throw new AppException("Email Address was already taken...");
            }

            // Changed .FirstOrDefault() to .FirstOrDefaultAsync()
            User xusername = await _context.Users.FirstOrDefaultAsync(c => c.UserName == userdata.UserName);
            if (xusername is not null) {
                throw new AppException("Username was already taken...");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var xkey = _config["Jwt:Key"];
            var key = Encoding.ASCII.GetBytes(xkey);

            // CREATE SECRET KEY FOR USER TOKEN===============
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userdata.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var secret = tokenHandler.CreateToken(tokenDescriptor);
            var secretkey = tokenHandler.WriteToken(secret);

            userdata.Secretkey = secretkey.ToUpper();             
            userdata.Password = BCrypt.Net.BCrypt.HashPassword(passwd);
            userdata.Profilepic = "pix.png";
            userdata.RolesId = 2;
            
            // Changed to AddAsync and SaveChangesAsync
            await _context.Users.AddAsync(userdata);                
            await _context.SaveChangesAsync();
            
            return userdata;
        }

        public async Task<Role> getRolename(int id) {
            // Changed .Find() to .FindAsync()
            var role = await _context.Roles.FindAsync(id);
            return role;
        }

        public async Task<User> SignUser(string usrname, string pwd)
        {
            try {
                // Changed .FirstOrDefault() to .FirstOrDefaultAsync()
                var xuser = await _context.Users.FirstOrDefaultAsync(c => c.UserName == usrname);
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
