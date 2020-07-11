using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helper;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;

        public DatingRepository(DataContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);        
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(x=> x.Photos).FirstOrDefaultAsync(x=> x.Id == id);
            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _context.Users.Include(x=> x.Photos).OrderBy(q=>q.LastActive).AsQueryable();
            
            users = users.Where(x => x.Id != userParams.UserId && x.Gender == userParams.Gender);
            
            if (userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserId,userParams.Likers);
                users = users.Where(u => userLikers.Contains(u.Id));
            }

            if( userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserId,userParams.Likees);
                users = users.Where(u => userLikees.Contains(u.Id));
            }
            
            if(userParams.MinAge > 18 || userParams.MaxAge < 99)
            {
                var minDob = DateTime.Now.AddYears(-userParams.MaxAge - 1);
                var maxDob = DateTime.Now.AddYears(userParams.MaxAge);

                users = users.Where(u => u.DateOfBirth >=minDob && u.DateOfBirth <= maxDob);
            }

            if ( !string.IsNullOrEmpty(userParams.orderBy)) {
                switch(userParams.orderBy) {
                    case "Created": 
                        users = users.OrderByDescending(x => x.CreatedOn);
                        break;
                     default :
                        users = users.OrderByDescending(x => x.LastActive);
                        break;
                }
            }
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var user = await _context.Users.Include(x=> x.Liker)
                .Include(x=>x.Likee).FirstOrDefaultAsync(x=>x.Id == id);

            if(likers)
            {
                return user.Liker.Where(x=>x.LikeeId == id).Select(x=>x.LikerId);
            }
            else
            {
                return user.Likee.Where(x=>x.LikerId == id).Select(x=>x.LikeeId);
            }
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(x => x.Id == id);
            
            return photo;
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            var mainPhoto = await _context.Photos.Where(x=> x.UserId == userId).FirstOrDefaultAsync(u => u.IsMain);

            return mainPhoto;
        }

        public async Task<Like> getLike(int userId, int recipientId)
        {
            return await _context.Like
                    .FirstOrDefaultAsync(u => u.LikerId == userId && u.LikeeId == recipientId);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messege.FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<PagedList<Message>> GetMessageForUser()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Message>> GetMessageThread(int userid, int recipientId)
        {
            throw new NotImplementedException();
        }
    }
}