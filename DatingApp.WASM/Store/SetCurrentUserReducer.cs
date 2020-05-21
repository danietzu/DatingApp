using Fluxor;

namespace DatingApp.WASM.Store
{
    public class SetCurrentUserReducer : Reducer<UserState, SetCurrentUserAction>
    {
        public override UserState Reduce(UserState state, SetCurrentUserAction action)
        {
            return new UserState(action.User);
        }
    }
}