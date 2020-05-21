using DatingApp.WASM.Models;

namespace DatingApp.WASM.Store
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