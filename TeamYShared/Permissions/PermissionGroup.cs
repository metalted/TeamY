using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamYShared.Permissions
{
    public class PermissionGroup
    {
        public string Name;
        public HashSet<string> Permissions = new HashSet<string>();
        public Dictionary<string, int> Limits = new Dictionary<string, int>();
    }
}
