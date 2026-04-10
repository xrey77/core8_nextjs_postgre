using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using core8_nextjs_postgre.Entities;
using core8_nextjs_postgre.Helpers;
using System.Threading.Tasks;

namespace core8_nextjs_postgre.Services
{
    public interface IUserService {
        Task<IEnumerable<User>> GetAll();
        Task<User> GetById(int id);
        Task UpdateProfile(User user);
        Task Delete(int id);
        Task ActivateMfa(int id, bool opt, string qrcode_url);
        Task UpdatePicture(int id, string file);
        Task UpdatePassword(User user, string password = null);
        int EmailToken(int etoken);
        Task<int> SendEmailToken(string email);
        Task ActivateUser(int id);
        Task ChangePassword(User userParam);
    }

    public class UserService : IUserService
    {
        private ApplicationDbContext _context;
        private readonly AppSettings _appSettings;

         IConfiguration config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .Build();

        public UserService(ApplicationDbContext context,IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        public async Task Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            else 
            {
               throw new AppException("User not found");
            }               
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) 
            {
                throw new AppException("User does not exist....");
            }
            return user;            
        }


        public async Task UpdateProfile(User userParam)
        {
            var user = await _context.Users.FindAsync(userParam.Id);

            if (user is null) {
                throw new AppException("User not found");
            }
            
            if (!string.IsNullOrWhiteSpace(userParam.FirstName)) {
                user.FirstName = userParam.FirstName;
            }

            if (!string.IsNullOrWhiteSpace(userParam.LastName)) {
                user.LastName = userParam.LastName;
            }

            if (!string.IsNullOrWhiteSpace(userParam.Mobile)) {
                user.Mobile = userParam.Mobile;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePassword(User userParam, string password = null)
        {
            var user = await _context.Users.FindAsync(userParam.Id);
            if (user is null)
                throw new AppException("User not found");

            if (!string.IsNullOrWhiteSpace(userParam.Password))
            {
                 user.Password = BCrypt.Net.BCrypt.HashPassword(userParam.Password);

            }
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }


        public async Task ActivateMfa(int id, bool opt, string qrcode_url)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is not null)
            {
                if (opt == true ) {

                    user.Qrcodeurl = qrcode_url;
                } else {
                    user.Qrcodeurl = null;
                }
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            else {
               throw new AppException("User not found");
            }                    }

        public async Task UpdatePicture(int id, string file)
        {
            var user = await _context.Users.FindAsync(id);                        
            if (user is not null)
            {
                user.Profilepic = file;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            else {
               throw new AppException("User not found");
            }                    
        }

       public async Task ActivateUser(int id) 
       {
            var user = await _context.Users.FindAsync(id);
            if (user.Isblocked == 1) {
                throw new AppException("Account has been blocked.");
            }
            if ( user.Isactivated == 1) {
                throw new AppException("Account is alread activated.");
            }
            user.Isactivated = 1;
            if (user is null)
            {
                throw new AppException("User not found");
            }
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
       }

        public async Task<int> SendEmailToken(string email)
        {
           var user = await _context.Users.FirstOrDefaultAsync(c => c.Email == email);           
           if (user is null) {
                throw new AppException("Email Address not found...");
           }
            var etoken = EmailToken(user.Mailtoken);
            user.Mailtoken = etoken;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return etoken;
        }       

        public int EmailToken(int etoken)
        {
            int _min = etoken;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        public async Task ChangePassword(User userParam)
        {
           var xuser = await _context.Users.FirstOrDefaultAsync(c => c.Email == userParam.Email);           
           var etoken = EmailToken(xuser.Mailtoken);

            if (xuser is null) {
                throw new AppException("Email Address not found...");
            }           
            if (xuser.UserName != userParam.UserName)
            {
                throw new AppException("Username not found...");
            }
            if (xuser.Password == null)
            {
                throw new AppException("Please enter Password...");
            }
            xuser.Password = BCrypt.Net.BCrypt.HashPassword(userParam.Password);
            _context.Users.Update(xuser);
            await _context.SaveChangesAsync();
        }
    }
}