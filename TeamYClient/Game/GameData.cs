using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamYClient.Game
{
    public enum GameState
    {
        StartUp,
        MainMenu,      
        EnteringOnlineEditorFromMainMenu,
        OnlineEditor,
        OnlineGame
    }

    public class GameData
    {
        private bool _init = false;
        public ulong SteamID { get; private set; }
        public GameState State { get; private set; }
        public int BlockCount { get; private set; }
        public LEV_LevelEditorCentral LEV_Central { get; private set; }

        public void Initialize()
        {
            if(_init)
            {
                return;
            }

            SteamID = PlayerManager.Instance.steamAchiever.GetPlayerSteamID();

            _init = true;
        }

        public void SetState(GameState state)
        {
            State = state;
        }

        public void SetCurrentLEVCentral(LEV_LevelEditorCentral LEV_Central)
        {
            this.LEV_Central = LEV_Central;
        }
    }
}
