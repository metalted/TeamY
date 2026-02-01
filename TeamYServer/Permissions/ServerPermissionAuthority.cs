using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamYShared.Permissions;

namespace TeamYServer.Permissions
{
    public class ServerPermissionAuthority
    {
        private PermissionRegistry registry;
        private Dictionary<ulong, string> playerGroups;

        public ServerPermissionAuthority()
        {
            registry = new PermissionRegistry();
            playerGroups = new Dictionary<ulong, string>();
        }

        public bool Can(ulong steamID, string permission)
        {
            if (!playerGroups.TryGetValue(steamID, out var groupName))
                return false;

            var group = registry.Get(groupName);
            return PermissionEvaluator.Has(group, permission);
        }

        public int GetLimit(ulong steamID, string key, int defaultValue = 0)
        {
            if (!playerGroups.TryGetValue(steamID, out var groupName))
                return defaultValue;

            var group = registry.Get(groupName);
            return PermissionEvaluator.GetLimit(group, key, defaultValue);
        }
    }
}
