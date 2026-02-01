using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamYShared.Permissions
{
    public class PermissionRegistry
    {
        private readonly Dictionary<string, PermissionGroup> groups = new Dictionary<string, PermissionGroup>();

        public PermissionGroup Get(string name) => groups.TryGetValue(name, out var g) ? g : null;

        public void Add(PermissionGroup group) => groups[group.Name] = group;
    }
}
