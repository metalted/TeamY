using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamYShared.Permissions
{
    public static class PermissionEvaluator
    {
        public static bool Has(PermissionGroup group, string permission)
        {
            if (group == null)
                return false;

            if (group.Permissions.Contains("*"))
                return true;

            if (group.Permissions.Contains(permission))
                return true;

            var parts = permission.Split('.');
            for (int i = parts.Length - 1; i > 0; i--)
            {
                var wildcard = string.Join(".", parts, 0, i) + ".*";
                if (group.Permissions.Contains(wildcard))
                    return true;
            }

            return false;
        }

        public static int GetLimit(
            PermissionGroup group,
            string key,
            int defaultValue = 0)
        {
            if (group == null)
                return defaultValue;

            return group.Limits.TryGetValue(key, out var value)
                ? value
                : defaultValue;
        }
    }
}
