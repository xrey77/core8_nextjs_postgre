using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using core8_nextjs_postgre.Entities;
using core8_nextjs_postgre.Helpers;

namespace core8_nextjs_postgre.Services
{
    public interface IUserService {
        IEnumerable<User> GetAll();
        User GetById(int id);
        void UpdateProfile(User user);
        void Delete(int id);
        void ActivateMfa(int id, bool opt, string qrcode_url);
        void UpdatePicture(int id, string file);
        void UpdatePassword(User user, string password = null);
        int EmailToken(int etoken);
        int SendEmailToken(string email);
        void ActivateUser(int id);
        void ChangePassword(User userParam);
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

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            else {
               throw new AppException("User not found");
            }   
        }

        public IEnumerable<User> GetAll()
        {
            var users = _context.Users.ToList();
            return users;
        }

        public User GetById(int id)
        {
            var user = _context.Users.Find(id);
            if (user is null) {
                throw new AppException("User does'not exists....");
            }
            return user;
        }

        public void UpdateProfile(User userParam)
        {
            var user = _context.Users.Find(userParam.Id);
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

            DateTime now = DateTime.Now;
            user.UpdatedAt = now;
            _context.Users.Update(user);
            _context.SaveChanges();            
        }

        public void UpdatePassword(User userParam, string password = null)
        {
            var user = _context.Users.Find(userParam.Id);
            if (user is null)
                throw new AppException("User not found");

            if (!string.IsNullOrWhiteSpace(userParam.Password))
            {
                 user.Password = BCrypt.Net.BCrypt.HashPassword(userParam.Password);

            }
            DateTime now = DateTime.Now;
            user.UpdatedAt = now;
            _context.Users.Update(user);
            _context.SaveChanges();            
        }


        public void ActivateMfa(int id, bool opt, string qrcode_url)
        {
           var user = _context.Users.Find(id);
            if (user is not null)
            {
                if (opt == true ) {

                    user.Qrcodeurl = qrcode_url;
                } else {
                    user.Qrcodeurl = null;
                }
                _context.Users.Update(user);
                _context.SaveChanges();
            }
            else {
               throw new AppException("User not found");
            }                    }

        public void UpdatePicture(int id, string file)
        {
            var user = _context.Users.Find(id);
            if (user is not null)
            {
                user.Profilepic = file;
                _context.Users.Update(user);
                _context.SaveChanges();
            }
            else {
               throw new AppException("User not found");
            }                    
        }

       public void ActivateUser(int id) 
       {
            var user = _context.Users.Find(id);
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
            _context.SaveChanges();            
       }

        public int SendEmailToken(string email)
        {
           var user =  _context.Users.AsQueryable().FirstOrDefault(c => c.Email == email);
           if (user is null) {
                throw new AppException("Email Address not found...");
           }
            var etoken = EmailToken(user.Mailtoken);
            user.Mailtoken = etoken;
            _context.Users.Update(user);
            _context.SaveChanges();
            return etoken;
        }       

        public int EmailToken(int etoken)
        {
            int _min = etoken;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        public void ChangePassword(User userParam)
        {
           var xuser =  _context.Users.AsQueryable().FirstOrDefault(c => c.Email == userParam.Email);
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
            _context.SaveChanges();
        }



    }
}