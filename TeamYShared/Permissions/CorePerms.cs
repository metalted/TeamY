using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamYShared.Permissions
{
    public static class CorePerms
    {
        public const string EDITOR_CREATE = "editor.create";
        public const string EDITOR_DESTROY = "editor.destroy";
        public const string EDITOR_UPDATE_ALL = "editor.update.all";
        public const string EDITOR_UPDATE_SELF = "editor.update.self";
        public const string EDITOR_UPDATE_SKYBOX = "editor.update.skybox";
        public const string EDITOR_UPDATE_FLOOR = "editor.update.floor";

        public const string BLOCK_BANNED_PREFIX = "block.banned.";
        public static string BLOCK_BANNED(int id)
        {
            return $"{BLOCK_BANNED_PREFIX}{id}";
        }
        public const string BLOCK_LIMIT = "block.limit";        
    }
}
