using CAMPUS.DAL;
using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Enums;

namespace CAMPUS.BLL
{
    public class UserService
    {
        private readonly UserDal _userDal;
        public UserService(UserDal userDal)
        {
            _userDal = userDal;
        }
        public Task<User> GetOrCreateAsync(string authId, string email, string name)
        {
            var user = _userDal.GetUserByAuthId(authId);
            if (user != null)
            {
                return Task.FromResult(user);
            }

            return _userDal.CreateUserAsync(authId, email, name);
        }
    }
}