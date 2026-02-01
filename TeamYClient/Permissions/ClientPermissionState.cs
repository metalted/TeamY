using TeamYShared.Permissions;

namespace TeamYClient.Permissions
{
    public class ClientPermissionState
    {
        public PermissionGroup Group { get; private set; }

        public void ApplyGroup(PermissionGroup group)
        {
            Group = group;
        }

        public bool Has(string permission) => PermissionEvaluator.Has(Group, permission);

        public int GetLimit(string key, int defaultValue = 0) => PermissionEvaluator.GetLimit(Group, key, defaultValue);
    }
}
