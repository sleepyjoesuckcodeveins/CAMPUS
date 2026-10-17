using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Enums;
using CAMPUS.DOMAIN;
using Microsoft.EntityFrameworkCore;

namespace CAMPUS.DAL
{
    public class UserDal
    {

        private readonly CampusDbContext _context;

        public UserDal(CampusDbContext context)
        {
            _context = context;
        }

        public User? GetUserByAuthId(string authId)
        {
            return _context.Users.FirstOrDefault(u => u.Auth0Id == authId);
        }
        public async Task<User> CreateUserAsync(string authId, string email, string name)
        {
            var user = new User
            {
                Auth0Id = authId,
                Email = email,
                Name = name,
                Role = Roles.Employee // Default role
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
 