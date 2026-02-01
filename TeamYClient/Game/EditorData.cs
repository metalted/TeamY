using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamYClient.Game
{
    public class EditorData
    {
        public int BlockCount => AllBlocks.Count;
        private Dictionary<string, BlockPropertyJSON> AllBlocks = new Dictionary<string, BlockPropertyJSON>();
        private Dictionary<ulong, List<string>> SteamIDBlockUIDs = new Dictionary<ulong, List<string>>();
        public int FloorID { get; private set; }
        public string SkyboxJSON { get; private set; }

        public BlockPropertyJSON GetBlock(string uid)
        {
            AllBlocks.TryGetValue(uid, out var block);
            return block;
        }

        public void UpsertBlock(string uid, BlockPropertyJSON block)
        {
            AllBlocks[uid] = block;
        }

        public void AddBlockUIDForSteamID(ulong steamID, string uid)
        {
            if (!SteamIDBlockUIDs.TryGetValue(steamID, out var list))
            {
                list = new List<string>();
                SteamIDBlockUIDs[steamID] = list;
            }

            list.Add(uid);
        }

        public int GetBlockCountForSteamID(ulong steamID)
        {
            if (!SteamIDBlockUIDs.TryGetValue(steamID, out var list))
                return 0;

            return list.Count;
        }

        public bool HasBlockUIDForSteamID(ulong steamID, string uid)
        {
            if (!SteamIDBlockUIDs.TryGetValue(steamID, out var list))
                return false;

            return list.Contains(uid);
        }

        public bool RemoveBlock(string uid)
        {
            return AllBlocks.Remove(uid);
        }

        public bool RemoveBlockUIDForSteamID(ulong steamID, string uid)
        {
            if (!SteamIDBlockUIDs.TryGetValue(steamID, out var list))
                return false;

            bool removed = list.Remove(uid);

            if (removed && list.Count == 0)
                SteamIDBlockUIDs.Remove(steamID);

            return removed;
        }

        public void SetFloor(int id)
        {
            FloorID = id;
        }
        public void SetSkybox(string json)
        {
            SkyboxJSON = json;
        }
    }
}
