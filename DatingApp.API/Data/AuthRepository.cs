using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        public DataContext _Context { get; set; }
        public AuthRepository(DataContext context)
        {
            _Context = context;
        }
        public async Task<User> Login(string username, string password)
        {
            var user = await _Context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Username == username);

            if(user == null)
              return null;

            if(!VerifyPasswordHash(password,user.PasswordHash,user.PasswordSalt))
                return null;

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
               var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
               for (int i = 0; i < computeHash.Length; i++)
               {
                   if(computeHash[i] != passwordHash[i]) 
                        return false;
               }
            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordhash, passwordsalt;
            CreatePassword(password, out passwordhash, out passwordsalt);

            user.PasswordHash = passwordhash;
            user.PasswordSalt = passwordsalt;

            await _Context.Users.AddAsync(user);
            await _Context.SaveChangesAsync();
            return user;
        }

        private void CreatePassword(string password, out byte[] passwordhash, out byte[] passwordsalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordsalt = hmac.Key;
                passwordhash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username)
        {
            if(await _Context.Users.AnyAsync(x=> x.Username == username))
             return true;

             return false;
        }
    }
}