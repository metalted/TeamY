using Newtonsoft.Json;
using Steamworks.Ugc;
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
            if (target.gameObject.GetComponent<LocalPlayerTransformTracker>() == null)
            {
                target.gameObject.AddComponent<LocalPlayerTransformTracker>();
            }
        }

        public void LoadEditorFromSavedState()
        {

        }

        public void LoadEditorScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("LevelEditor2");
        }     
        
        public void ClearEditor()
        {
            foreach(BlockProperties bp in _data.LEV_Central.undoRedo.allBlocksDictionary.Values)
            {
                if(bp != null)
                {
                    bool isSelected = _data.LEV_Central.selection.list.Any(block => block.UID == bp.UID);
                    if (isSelected)
                    {
                        DeselectBlock(bp.UID);
                    }

                    GameObject.Destroy(bp.gameObject);
                }
            }

            _data.LEV_Central.undoRedo.allBlocksDictionary.Clear();
            _data.LEV_Central.validation.RecalcBlocksAndDraw(false);
        }

        public void CreateBlock(BlockPropertyJSON block, bool recalculate = true)
        {
            _data.LEV_Central.undoRedo.GenerateNewBlock(block, block.u);

            if(recalculate)
            {
                _data.LEV_Central.validation.RecalcBlocksAndDraw(false);
            }
        }

        public void UpdateBlock(BlockPropertyJSON block)
        {
            BlockProperties bp = _data.LEV_Central.undoRedo.TryGetBlockFromAllBlocks(block.u);
            if(bp != null)
            {
                _data.LEV_Central.undoRedo.allBlocksDictionary.Remove(block.u);
                BlockPropertyJSON json = bp.ConvertBlockToJSON_v15();
                GameObject.Destroy(bp.gameObject);
                _data.LEV_Central.undoRedo.GenerateNewBlock(block, json.u);
            }
        }

        public void DestroyBlock(string uid)
        {
            BlockProperties bp = _data.LEV_Central.undoRedo.TryGetBlockFromAllBlocks(uid);

            if(bp != null)
            {
                bool isSelected = _data.LEV_Central.selection.list.Any(block => block.UID == bp.UID);
                if(isSelected)
                {
                    DeselectBlock(bp.UID);
                }

                GameObject.Destroy(bp.gameObject);
                _data.LEV_Central.validation.RecalcBlocksAndDraw(false);
            }
        }       

        public void UpdateFloor(int floorID)
        {
            //Fix because function is called before painter is done initializing.
            if (_data.LEV_Central.painter.MaterialManager == null)
            {
                _data.LEV_Central.painter.MaterialManager = GameObject.Find("Material Manager").GetComponent<MaterialManager>();
            }

            _data.LEV_Central.painter.SetLoadGroundMaterial(floorID);
        }

        public void UpdateSkybox(string skyboxJSON)
        {
            Environment_DataObject environment = JsonConvert.DeserializeObject<Environment_DataObject>(skyboxJSON);

            _data.LEV_Central.skybox.simulateLofi = false;
            if(environment.skyboxOverride == null)
            {
                _data.LEV_Central.skyboxTool.internalSkyboxpreset = environment.skybox;
                _data.LEV_Central.skybox.SetToSkybox(environment.skybox, true, (SkyboxCreator_DataObject)null, true);
            }
            else
            {
                _data.LEV_Central.skyboxTool.internalSkyboxpreset = 15;
                _data.LEV_Central.skybox.SetToSkybox(0, true, environment.skyboxOverride, true);
            }
        }

        public void DeselectAllBlock(bool notify = false)
        {
            Plugin.Instance.SelectionObserver.Selection.DeselectAllBlocks(true, "");
            if(!notify)
            {
                Plugin.Instance.SelectionObserver.SyncListCount();
            }
        }

        public void DeselectBlock(string uid, bool notify = false)
        {
            int index = Plugin.Instance.SelectionObserver.Selection.list.FindIndex(item => item.UID == uid);
            if (index != -1)
            {
                Plugin.Instance.SelectionObserver.Selection.RemoveBlockAt(index, true, false);

                if (!notify)
                {
                    Plugin.Instance.SelectionObserver.SyncListCount();
                    Plugin.Instance.SelectionObserver.InspectSelection(false);
                }
            }
        }

        public void SelectBlock(string uid, bool notify = false)
        {
            int index = Plugin.Instance.SelectionObserver.Selection.list.FindIndex(item => item.UID == uid);

            if (index == -1 && _data.LEV_Central.undoRedo.allBlocksDictionary.ContainsKey(uid))
            {
                Plugin.Instance.SelectionObserver.Selection.AddThisBlock(_data.LEV_Central.undoRedo.allBlocksDictionary[uid]);

                if (!notify)
                {
                    Plugin.Instance.SelectionObserver.SyncListCount();
                }
            }
        }

        public void RemoveBlockFromSelection(string uid)
        {
            LEV_Selection selection = _data.LEV_Central.selection;
            int index = selection.list.FindIndex(s => s.UID == uid);

            if (index > 0)
            {
                if (selection.list.Count == 1)
                {
                    selection.ClickNothing();
                    _data.LEV_Central.gizmos.Deactivate();
                }
                else
                {
                    selection.RemoveBlockAt(index, true, true);
                }
            }
        }
    }
}
