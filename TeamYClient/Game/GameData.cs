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
        public GameState State { get; private set; }
        
        public LEV_LevelEditorCentral LEV_Central { get; set; }
        public EditorData Editor { get; private set; }
        public LocalData Local { get; private set; }

        public void Initialize()
        {
            if(_init)
            {
                return;
            }

            Editor = new EditorData();
            Local = new LocalData();
            Local.SteamID = PlayerManager.Instance.steamAchiever.GetPlayerSteamID();

            Plugin.Instance.LocalPlayerTracker.OnSnapshot += (snapshot) =>
            {
                Local.Position = snapshot.Position;
                Local.EulerRotation = snapshot.EulerRotation;
                Local.Mode = snapshot.Mode;
            };

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
