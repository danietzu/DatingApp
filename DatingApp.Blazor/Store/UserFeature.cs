using Fluxor;

namespace DatingApp.Blazor.Store
{
    public class UserFeature : Feature<UserState>
    {
        public override string GetName() => "User";

        protected override UserState GetInitialState() => new UserState(null);
    }
}