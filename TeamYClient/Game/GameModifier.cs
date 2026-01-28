using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TeamYClient.Game
{
    public class GameModifier
    {
        private GameData _data;

        public GameModifier(GameData gameData)
        {
            _data = gameData;
        }

        public void SetupLocalPlayerTracking(Transform target)
        {
            if (target.gameObject.GetComponent<LocalPlayerTracker>() == null)
            {
                target.gameObject.AddComponent<LocalPlayerTracker>();
            }
        }

        public void LoadEditorFromSavedState()
        {

        }

        public void LoadEditorScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("LevelEditor2");
        }
    }
}
