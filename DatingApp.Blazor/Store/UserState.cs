using DatingApp.Blazor.Models;

namespace DatingApp.Blazor.Store
{
    public class UserState
    {
        public User CurrentUser { get; }

        public UserState(User user)
        {
            CurrentUser = user;
        }
    }
}