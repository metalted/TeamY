using Newtonsoft.Json;
using Steamworks.Ugc;
using System.Collections.Generic;
using TeamYClient.Networking;
using TeamYClient.Permissions;
using TeamYClient.UI;
using TeamYShared.Permissions;
using UnityEngine;

namespace TeamYClient.Game
{
    public class GameObserver
    {
        private GameData _data;
        private GameModifier _modifier;
        private NetworkManagement _network;
        private UIManagement _ui;
        private LocalPlayerTracker _tracker;
        private ClientPermissionState _perms;

        public GameObserver(GameData gameData, GameModifier modifier, NetworkManagement network, UIManagement ui, LocalPlayerTracker tracker, ClientPermissionState permissions)
        {
            _data = gameData;
            _modifier = modifier;
            _network = network;
            _ui = ui;
            _tracker = tracker;
            _perms = permissions;
        }

        #region Update Patches
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
        
        public void OnPossibleSelectionChange()
        {
            if (_data.State != GameState.OnlineEditor)
            {
                return;
            }

            Plugin.Instance.SelectionObserver?.InspectSelection();
        }
        #endregion

        #region UI Patches
        public bool OnValidationRecalculateBlocks(LEV_ValidationLock instance, bool setDebounce)
        {
            if (_data.State != GameState.OnlineEditor)
            {
                return true;
            }

            instance.amountOfBlocks = _data.Editor.BlockCount;
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
        #endregion

        #region GamePlay Patches
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
            /*if (!_network.IsConnected)
            {
                return;
            }*/

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

            _tracker.SetMode(Shpleeble.CharacterMode.Build);

            _data.LEV_Central = instance;
            if(instance.gameObject.GetComponent<SelectionObserver>() == null)
            {
                Plugin.Instance.SelectionObserver = instance.gameObject.AddComponent<SelectionObserver>();
                Plugin.Instance.SelectionObserver.Initialize(instance.selection);
            }

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

            _tracker.SetMode(Shpleeble.CharacterMode.Race);
        }
        
        public bool OnEnteredTestMap(LEV_TestMap instance)
        {
            if (_data.State == GameState.OnlineEditor)
            {
                //Halts execution of loading when online editor is used.
                return false;
            }

            return true;
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

            //This needs updating to decide which is which.
            //_tracker.SetMode(Shpleeble.CharacterMode.Build);
        }
        #endregion

        #region Editor Updates
        private void OnBlockCreated(string after)
        {
            BlockPropertyJSON afterBlock = LEV_UndoRedo.GetJSONblock(after);

            //Does this player have the create permission?
            bool canCreate = _perms.Has(CorePerms.EDITOR_CREATE);
            //Is this block banned?
            bool isBlockBanned = _perms.Has(CorePerms.BLOCK_BANNED(afterBlock.i));
            //Is there a block limit and if so, are we within it?
            bool withinBlockLimit = true;
            int blockLimit = _perms.GetLimit(CorePerms.BLOCK_LIMIT);
            if(blockLimit >= 0)
            {
                //There is a block limit.
                int currentBlockCount = _data.Editor.GetBlockCountForSteamID(_data.Local.SteamID);
                if(currentBlockCount >= blockLimit)
                {
                    withinBlockLimit = false;
                }
            }
            
            //Is a valid create?
            if(canCreate && withinBlockLimit && !isBlockBanned)
            {
                //Store the block under its UID
                _data.Editor.UpsertBlock(afterBlock.u, afterBlock);
                //Assign this block to the user
                _data.Editor.AddBlockUIDForSteamID(_data.Local.SteamID, afterBlock.u);
                //Send the update to the server.
                _network.SendBlockCreate(_data.Local.SteamID, after);
            }
            else
            {
                //Not allowed, revert.
                _modifier.DestroyBlock(afterBlock.u);
            }
        }

        private void OnBlockUpdated(string before, string after)
        {
            BlockPropertyJSON beforeBlock = LEV_UndoRedo.GetJSONblock(before);
            BlockPropertyJSON afterBlock = LEV_UndoRedo.GetJSONblock(after);

            //Check if block == null (TeamX)
            BlockPropertyJSON stored = _data.Editor.GetBlock(afterBlock.u);
            if(stored == null)
            {
                //As the block is not found in the online editor, recreate it so the games are synced again. (TeamX)

                //Store the block under its UID
                _data.Editor.UpsertBlock(afterBlock.u, afterBlock);

                //Assign this block to the user
                _data.Editor.AddBlockUIDForSteamID(_data.Local.SteamID, afterBlock.u);

                //Send the update to the server.
                _network.SendBlockCreate(_data.Local.SteamID, after);

                return;
            }

            bool canEdit = _perms.Has(CorePerms.EDITOR_UPDATE_ALL) || (_perms.Has(CorePerms.EDITOR_UPDATE_SELF) && _data.Editor.HasBlockUIDForSteamID(_data.Local.SteamID, beforeBlock.u));

            if (canEdit)
            {
                _data.Editor.UpsertBlock(afterBlock.u, afterBlock);
                _network.SendBlockUpdate(_data.Local.SteamID, after);
            }
            else
            {
                //Not allowed
                //Remove the block from the selection if its in there.
                _modifier.RemoveBlockFromSelection(afterBlock.u);
                _modifier.UpdateBlock(beforeBlock);
            }
        }

        private void OnBlockDestroyed(string before)
        {
            BlockPropertyJSON beforeBlock = LEV_UndoRedo.GetJSONblock(before);

            //Make sure to look at the CTRLZ case in TeamX.
            BlockPropertyJSON stored = _data.Editor.GetBlock(beforeBlock.u);
            if(stored == null)
            {
                //This can happen with blocked creations in combination with control z.
                return;
            }           

            bool canDestroy = _perms.Has(CorePerms.EDITOR_DESTROY);
            bool canEdit = _perms.Has(CorePerms.EDITOR_UPDATE_ALL) || (_perms.Has(CorePerms.EDITOR_UPDATE_SELF) && _data.Editor.HasBlockUIDForSteamID(_data.Local.SteamID, beforeBlock.u));
        
            if(canDestroy && canEdit)
            {
                _data.Editor.RemoveBlock(beforeBlock.u);
                _data.Editor.RemoveBlockUIDForSteamID(_data.Local.SteamID, beforeBlock.u);
                _network.SendBlockDestroy(_data.Local.SteamID, before);
            }
            else
            {
                //Not allowed
                _modifier.CreateBlock(beforeBlock);
            }
        }
        
        private void OnFloorUpdated(int before, int after)
        {
            bool canUpdate = _perms.Has(CorePerms.EDITOR_UPDATE_FLOOR);

            if(canUpdate)
            {
                _data.Editor.SetFloor(after);
                _network.SendFloorUpdate(_data.Local.SteamID, after);
            }
            else
            {
                _modifier.UpdateFloor(before);
            }
        }
        
        private void OnSkyboxUpdated(string customBefore, string customAfter, int before, int after)
        {
            bool canUpdate = _perms.Has(CorePerms.EDITOR_UPDATE_SKYBOX);

            if(canUpdate)
            {
                Environment_DataObject env = new Environment_DataObject();
                env.skyboxOverride = string.IsNullOrEmpty(customAfter) ? null : JsonConvert.DeserializeObject<SkyboxCreator_DataObject>(customAfter);
                env.skybox = after;
                env.groundMat = _data.Editor.FloorID;
                env.overrideFog_b = _data.LEV_Central.skybox.overrideFogBool;
                env.overrideFog_f = _data.LEV_Central.skybox.overrideFogFloat;
                string json = JsonConvert.SerializeObject(env);

                _data.Editor.SetSkybox(json);
                _network.SendSkyboxUpdate(_data.Local.SteamID, json);
            }
            else
            {
                Environment_DataObject env = new Environment_DataObject();
                env.skyboxOverride = string.IsNullOrEmpty(customBefore) ? null : JsonConvert.DeserializeObject<SkyboxCreator_DataObject>(customBefore);
                env.skybox = before;
                env.groundMat = _data.Editor.FloorID;
                env.overrideFog_b = _data.LEV_Central.skybox.overrideFogBool;
                env.overrideFog_f = _data.LEV_Central.skybox.overrideFogFloat;
                string json = JsonConvert.SerializeObject(env);
                _modifier.UpdateSkybox(json);
            }
        }
        
        public void OnBlocksAddedToSelection(List<string> blockUIDs)
        {
            bool canSelectAny = _perms.Has(CorePerms.EDITOR_UPDATE_ALL);
            bool canSelectSelf = _perms.Has(CorePerms.EDITOR_UPDATE_SELF);

            foreach (string uid in blockUIDs)
            {
                BlockPropertyJSON block = _data.Editor.GetBlock(uid);
                if(block == null)
                {
                    continue;
                }

                if(canSelectAny || (canSelectSelf && _data.Editor.HasBlockUIDForSteamID(_data.Local.SteamID, uid)))
                {
                    _network.SendSelection(_data.Local.SteamID, uid);
                }
                else
                {
                    _modifier.DeselectBlock(uid);
                }
            }
        }

        public void OnBlocksRemovedFromSelection(List<string> blockUIDs)
        {
            foreach (string uid in blockUIDs)
            {
                BlockPropertyJSON block = _data.Editor.GetBlock(uid);
                if (block == null)
                {
                    continue;
                }

                _network.SendDeselection(_data.Local.SteamID, uid);
            }
        }
        #endregion
    }
}
