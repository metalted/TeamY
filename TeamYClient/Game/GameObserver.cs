using System.Collections.Generic;
using TeamYClient.Networking;
using TeamYClient.UI;
using UnityEngine;

namespace TeamYClient.Game
{
    public class GameObserver
    {
        private GameData _data;
        private GameModifier _modifier;
        private NetworkManagement _network;
        private UIManagement _ui;

        public GameObserver(GameData gameData, GameModifier modifier, NetworkManagement network, UIManagement ui)
        {
            _data = gameData;
            _modifier = modifier;
            _network = network;
            _ui = ui;
        }

        #region Patch Entries
        public void OnCTRLZSomethingChanged(Change_Collection changes, string source)
        {
            if(_data.State != GameState.OnlineEditor)
            {
                return;
            }

            foreach(Change_Single changeSingle in changes.changeList)
            {
                switch(changes.changeType)
                {
                    case Change_Collection.ChangeType.block:
                        if(changeSingle.before == null)
                        {
                            OnBlockCreated(changeSingle.after);
                        }
                        else if(changeSingle.after == null)
                        {
                            OnBlockDestroyed(changeSingle.before);
                        }
                        else
                        {
                            OnBlockUpdated(changeSingle.before, changeSingle.after);
                        }
                        break;
                    case Change_Collection.ChangeType.floor:
                        OnFloorUpdated(changeSingle.int_before, changeSingle.int_after);
                        break;
                    case Change_Collection.ChangeType.skybox:
                        OnSkyboxUpdated(changeSingle.customSkybox_before, changeSingle.customSkybox_after, changeSingle.int_before, changeSingle.int_after);
                        break;
                    case Change_Collection.ChangeType.connection:
                        //Not implemented
                        break;
                    case Change_Collection.ChangeType.selection:
                        //This case is not used.
                        break;
                }
            }
        }

        public void OnCTRLZApplyBefore(LEV_UndoRedo instance)
        {
            if (_data.State != GameState.OnlineEditor)
            {
                return;
            }

            Change_Collection changes = instance.historyList[instance.currentHistoryPosition];

            foreach(Change_Single changeSingle in changes.changeList)
            {
                switch(changes.changeType)
                {
                    case Change_Collection.ChangeType.block:
                        if(changeSingle.before == null)
                        {
                            OnBlockDestroyed(changeSingle.after);
                        }
                        else if(changeSingle.after == null)
                        {
                            OnBlockCreated(changeSingle.before);
                        }
                        else
                        {
                            OnBlockUpdated(changeSingle.after, changeSingle.before);
                        }
                        break;
                    case Change_Collection.ChangeType.floor:
                        OnFloorUpdated(changeSingle.int_after, changeSingle.int_before);
                        break;
                    case Change_Collection.ChangeType.skybox:
                        OnSkyboxUpdated(changeSingle.customSkybox_before, changeSingle.customSkybox_after, changeSingle.int_before, changeSingle.int_after);
                        break;
                    case Change_Collection.ChangeType.connection:
                        //Not implemented
                        break;
                    case Change_Collection.ChangeType.selection:
                        //This case is not used.
                        break;
                }
            }
        }

        public void OnCTRLZApplyAfter(LEV_UndoRedo instance)
        {
            if (_data.State != GameState.OnlineEditor)
            {
                return;
            }

            Change_Collection changes = instance.historyList[instance.currentHistoryPosition];

            foreach(Change_Single changeSingle in changes.changeList)
            {
                switch(changes.changeType)
                {
                    case Change_Collection.ChangeType.block:
                        if(changeSingle.before == null)
                        {
                            OnBlockCreated(changeSingle.after);
                        }
                        else if(changeSingle.after == null)
                        {
                            OnBlockDestroyed(changeSingle.before);
                        }
                        else
                        {
                            OnBlockUpdated(changeSingle.before, changeSingle.after);
                        }
                        break;
                    case Change_Collection.ChangeType.floor:
                        OnFloorUpdated(changeSingle.int_before, changeSingle.int_after);
                        break;
                    case Change_Collection.ChangeType.skybox:
                        OnSkyboxUpdated(changeSingle.customSkybox_before, changeSingle.customSkybox_after, changeSingle.int_before, changeSingle.int_after);
                        break;
                    case Change_Collection.ChangeType.connection:
                        //Not implemented
                        break;
                    case Change_Collection.ChangeType.selection:
                        //This case is not used.
                        break;
                }
            }
        }

        public bool OnValidationRecalculateBlocks(LEV_ValidationLock instance, bool setDebounce)
        {
            if (_data.State != GameState.OnlineEditor)
            {
                return true;
            }

            instance.amountOfBlocks = _data.BlockCount;
            instance.levelTip.text = "TeamX";

            if(setDebounce)
            {
                instance.recalcDebounce = 1;
            }

            return false;
        }

        public bool OnUpdateValidationText(LEV_ValidationLock instance)
        {
            if (_data.State != GameState.OnlineEditor)
            {
                return true;
            }

            instance.debugText.text = instance.amountOfBlocks.ToString() + " " + I2.Loc.LocalizationManager.GetTranslation("ABC_Blocks");

            return false;
        }

        public void OnEnteredMainMenu()
        {
            _data.Initialize();
            _network.Initialize();

            _ui.CreateMainMenuUI();
            _network.OnEnteredMainMenu();

            _data.SetState(GameState.MainMenu);
        }

        public void OnEnteredLevelEditor(LEV_LevelEditorCentral instance)
        {
            if (!_network.IsConnected)
            {
                return;
            }

            //Determine if we came from main menu or test map.
            if(_data.State == GameState.EnteringOnlineEditorFromMainMenu)
            {
                PlayerManager.Instance.weLoadedLevelEditorFromMainMenu = true;
                _data.SetState(GameState.OnlineEditor);
            }
            else if(_data.State == GameState.OnlineGame)
            {
                PlayerManager.Instance.weLoadedLevelEditorFromMainMenu = false;
                _data.SetState(GameState.OnlineEditor);
            }

            //Should be OnlineEditor right here.
            if(_data.State != GameState.OnlineEditor)
            {
                Debug.LogError("Game is connected, but we are not in OnlineEditor state, this is a strange state that shouldnt happen! Current state: " + _data.State.ToString());
                return;
            }

            //multiplayer.LocalPlayerMode = CharacterMode.Build; Shpleeble stuff

            _data.SetCurrentLEVCentral(instance);
            _modifier.SetupLocalPlayerTracking(instance.cam.cameraTransform);
            _modifier.LoadEditorFromSavedState();

            if(!instance.testMap.GlobalLevel.IsTestLevel)
            {
                return;
            }

            instance.testMap.GlobalLevel.IsTestLevel = false;
            PlayerManager.Instance.unsavedContent = false;

            /* Put back player stuff.
            //Put the player back at their original location.
            if (multiplayer.lastKnownEditorLocation.SteamID != 0)
            {
                //Data has been assigned previously as the SteamID has a value.
                central.cam.transform.position = multiplayer.lastKnownEditorLocation.Position;
            }
            */

            if(PlayerManager.Instance.weLoadedLevelEditorFromMainMenu)
            {
                return;
            }

            //Assign ctrl-z history list back to the game.
            instance.undoRedo.ResetUndoList(true);
        }

        public void OnEnteredGame(SetupGame instance)
        {
            if(_data.State != GameState.OnlineEditor)
            {
                return;
            }

            _data.SetState(GameState.OnlineGame);

            //multiplayer.LocalPlayerMode = CharacterMode.Race; Shpleeble stuff
        }

        public void OnLocalPlayersSpawned(GameMaster instance)
        {
            if(_data.State != GameState.OnlineGame || !_network.IsConnected)
            {
                return;
            }

            _modifier.SetupLocalPlayerTracking(instance.PlayersReady[0].transform);
        }

        public void OnLocalPlayerStateChanged(byte newState)
        {
            if(_data.State != GameState.OnlineGame || !_network.IsConnected)
            {
                return;
            }

            //multiplayer.LocalPlayerMode = state == 3 ? (CharacterMode)2 : (CharacterMode)1; Shpleeble stuff.
        }

        public bool OnEnteredTestMap(LEV_TestMap instance)
        {
            if(_data.State == GameState.OnlineEditor)
            {
                //Halts execution of loading when online editor is used.
                return false;
            }

            return true;
        }

        public void OnPossibleSelectionChange()
        {
            if (_data.State != GameState.OnlineEditor)
            {
                return;
            }

            Plugin.Instance.SelectionObserver?.InspectSelection();
        }
        #endregion

        #region Updates
        private void OnBlockCreated(string after)
        {
            BlockPropertyJSON afterBlock = LEV_UndoRedo.GetJSONblock(after);
            //Plugin.Instance.editor.Observer...
        }

        private void OnBlockDestroyed(string before)
        {
            BlockPropertyJSON beforeBlock = LEV_UndoRedo.GetJSONblock(before);
            //Plugin.Instance.editor.Observer...
        }

        private void OnBlockUpdated(string before, string after)
        {
            BlockPropertyJSON beforeBlock = LEV_UndoRedo.GetJSONblock(before);
            BlockPropertyJSON afterBlock = LEV_UndoRedo.GetJSONblock(after);
            //Plugin.Instance.editor.Observer...
        }

        private void OnFloorUpdated(int before, int after)
        {
            //Plugin.Instance.editor.Observer...
        }

        private void OnSkyboxUpdated(string customBefore, string customAfter, int before, int after)
        {
            //Plugin.Instance.editor.Observer...
        }

        public void OnBlocksAddedToSelection(List<string> blockUIDs)
        {
            //Plugin.Instance.editor.OnBlocksAddedToSelection
        }

        public void OnBlocksRemovedFromSelection(List<string> blockUIDs)
        {
            //Plugin.Instance.editor.OnBlocksRemovedFromSelection
        }
        #endregion
    }
}
