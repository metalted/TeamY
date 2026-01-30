using TeamYShared.Permissions;

namespace TeamYClient.Permissions
{
    public class ClientPermissionState
    {
        public PermissionProfile Profile { get; private set; }
        private readonly PermissionManager manager;

        public ClientPermissionState(PermissionRegistry registry)
        {
            manager = new PermissionManager(registry);
        }

        public void ApplyProfile(PermissionProfile profile)
        {
            Profile = profile;
            //manager.SetProfile(profile);
        }

        //public bool Has(string permission) => manager.Has(permission);

        //public int GetLimit(string key) => manager.GetLimit(key);
    }
}
