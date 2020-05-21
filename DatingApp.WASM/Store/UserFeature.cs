using Fluxor;

namespace DatingApp.WASM.Store
{
    public class UserFeature : Feature<UserState>
    {
        public override string GetName() => "User";

        protected override UserState GetInitialState() => new UserState(null);
    }
}