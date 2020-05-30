using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(x => x.LikerId == userId
                                                                 && x.LikeeId == recipientId);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos
                .Where(p => p.UserId == userId)
                .FirstOrDefaultAsync(p => p.IsMain == true);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);

            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Photos)
                .FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            // default sorting
            var users = _context.Users.Include(u => u.Photos)
                                      .OrderByDescending(u => u.LastActive)
                                      .AsQueryable();

            users = users.Where(u => u.Id != userParams.UserId);
            users = users.Where(u => u.Gender == userParams.Gender);

            if (userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikees.Contains(u.Id));
            }

            if (userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikers.Contains(u.Id));
            }

            if (userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDateOfBirth = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDateOfBirth = DateTime.Today.AddYears(-userParams.MinAge);

                users = users.Where(u => u.DateOfBirth >= minDateOfBirth &&
                                         u.DateOfBirth <= maxDateOfBirth);
            }

            // user sorting
            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                users = userParams.OrderBy switch
                {
                    "created" => users.OrderByDescending(u => u.Created),
                    _ => users.OrderByDescending(u => u.LastActive),
                };
            }

            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var user = await _context.Users.Include(x => x.Likers)
                                           .Include(x => x.Likees)
                                           .FirstOrDefaultAsync(x => x.Id == id);

            if (likers)
            {
                return user.Likers.Where(x => x.LikeeId == id).Select(x => x.LikerId);
            }
            else
            {
                return user.Likees.Where(x => x.LikerId == id).Select(x => x.LikeeId);
            }
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages
                                   .Include(m => m.Sender).ThenInclude(u => u.Photos)
                                   .Include(m => m.Recipient).ThenInclude(u => u.Photos)
                                   .AsQueryable();

            messages = messageParams.MessageContainer switch
            {
                "Inbox" => messages.Where(m => m.RecipientId == messageParams.UserId),
                "Outbox" => messages.Where(m => m.SenderId == messageParams.UserId),
                // "Unread" container
                _ => messages.Where(m => m.RecipientId == messageParams.UserId && m.IsRead == false),
            };

            messages = messages.OrderBy(m => m.MessageSent);

            return await PagedList<Message>.CreateAsync(messages,
                                                        messageParams.PageNumber,
                                                        messageParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            var messages = await _context.Messages
                                   .Include(m => m.Sender).ThenInclude(u => u.Photos)
                                   .Include(m => m.Recipient).ThenInclude(u => u.Photos)
                                   .Where(m => m.SenderId == userId && m.RecipientId == recipientId
                                            || m.SenderId == recipientId && m.RecipientId == userId)
                                   .OrderBy(m => m.MessageSent)
                                   .ToListAsync();

            return messages;
        }
    }
}