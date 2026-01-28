using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamYClient.Game
{
    /// <summary>
    /// Harmony patch for detecting changes in the editor and invoking appropriate handlers.
    /// </summary>
    [HarmonyPatch(typeof(LEV_UndoRedo), "SomethingChanged")]
    public class LEV_UndoRedoSomethingChangedPatch
    {
        public static void Postfix(ref Change_Collection whatChanged, ref string source)
        {
            Plugin.Instance.GameObserver.OnCTRLZSomethingChanged(whatChanged, source);
        }
    }

    /// <summary>
    /// Harmony patch for detecting changes in the editor and invoking appropriate handlers.
    /// </summary>
    [HarmonyPatch(typeof(LEV_UndoRedo), "ApplyBeforeState")]
    public class LEV_UndoRedoApplyBeforeStatePatch
    {
        public static void Postfix(LEV_UndoRedo __instance)
        {
            Plugin.Instance.GameObserver.OnCTRLZApplyBefore(__instance);
        }
    }
    
    /// <summary>
    /// Harmony patch for detecting changes in the editor and invoking appropriate handlers.
    /// </summary>
    [HarmonyPatch(typeof(LEV_UndoRedo), "ApplyAfterState")]
    public class LEV_UndoRedoApplyAfterStatePatch
    {
        public static void Postfix(LEV_UndoRedo __instance)
        {
            Plugin.Instance.GameObserver.OnCTRLZApplyAfter(__instance);
        }
    }

    /// <summary>
    /// Harmony patch for making the Recalculate blocks function more performant in the TeamX editor.
    /// </summary>
    [HarmonyPatch(typeof(LEV_ValidationLock), "RecalculateBlocks")]
    public class LEVValidationLockRecalculateBlocks
    {
        public static bool Prefix(ref bool setDebounce, LEV_ValidationLock __instance)
        {
            return Plugin.Instance.GameObserver.OnValidationRecalculateBlocks(__instance, setDebounce);
        }
    }

    /// <summary>
    /// Harmony patch for making the Update Validation Text function more performant in the TeamX editor. Validation is kind of impossible and we dont need to iterate over all blocks as we already have the block count.
    /// </summary>
    [HarmonyPatch(typeof(LEV_ValidationLock), "UpdateValidationText")]
    public class LEVValidationUpdateValidationText
    {
        public static bool Prefix(LEV_ValidationLock __instance)
        {
            return Plugin.Instance.GameObserver.OnUpdateValidationText(__instance);
        }
    }

    /// <summary>
    /// Harmony patch that's called when we enter the main menu and updates the GameManager.
    /// </summary>
    [HarmonyPatch(typeof(MainMenuUI), "Awake")]
    public class TKMainMenuUIAwakePatch
    {
        public static void Prefix()
        {
            Plugin.Instance.GameObserver.OnEnteredMainMenu();
        }
    }

    /// <summary>
    /// Harmony patch that's called when we enter the level editor and updates the GameManager.
    /// </summary>
    [HarmonyPatch(typeof(LEV_LevelEditorCentral), "Awake")]
    public class LevelEditorCentralAwakePatch
    {
        public static void Postfix(LEV_LevelEditorCentral __instance)
        {
            Plugin.Instance.GameObserver.OnEnteredLevelEditor(__instance);
        }
    }

    /// <summary>
    /// Harmony patch that's called when we enter a game mode and updates the GameManager.
    /// </summary>
    [HarmonyPatch(typeof(SetupGame), "Awake")]
    public class SetupGameAwakePatch
    {
        public static void Postfix(SetupGame __instance)
        {
            Plugin.Instance.GameObserver.OnEnteredGame(__instance);
        }
    }

    /// <summary>
    /// Harmony patch that's called when the local players gets spawned, so we can update the state and track that player.
    /// </summary>
    [HarmonyPatch(typeof(GameMaster), "SpawnPlayers")]
    public class GameMasterSpawnPlayersPatch
    {
        public static void Postfix(GameMaster __instance)
        {
            Plugin.Instance.GameObserver.OnLocalPlayersSpawned(__instance);
        }
    }

    /// <summary>
    /// Harmony patch that's called when the local players state changes, by a gate or restart.
    /// </summary>
    [HarmonyPatch(typeof(New_ControlCar), "SetZeepkistState")]
    public class NewControlCarSetZeepkistStatePatch
    {
        public static void Prefix(ref byte newState, ref string source, ref bool playSound)
        {
            Plugin.Instance.GameObserver.OnLocalPlayerStateChanged(newState);
        }
    }

    /// <summary>
    /// Harmony patch that will make sure Zeepkist doesnt load its own file when returning to the level editor from testing.
    /// The level should always be loaded from the networked editor data.
    /// </summary>
    [HarmonyPatch(typeof(LEV_TestMap), "Start")]
    public class TKTestMapStartPatch
    {
        public static bool Prefix(LEV_TestMap __instance)
        {
            return Plugin.Instance.GameObserver.OnEnteredTestMap(__instance);
        }
    }

    /// <summary>
    /// Harmony patch called when blocks are duplicated in the level editor.
    /// </summary>
    [HarmonyPatch(typeof(LEV_GizmoHandler), "DuplicateSelectedObjects")]
    public class LEVGizmoHandlerDuplicateSelectedObjectsPatch
    {
        public static void Postfix()
        {
            Plugin.Instance.GameObserver.OnPossibleSelectionChange();
        }
    }

    /// <summary>
    /// Harmony patch called when all blocks are deselected in the level editor.
    /// </summary>
    [HarmonyPatch(typeof(LEV_Selection), "DeselectAllBlocks")]
    public class LEVSelectionDeselectAllBlocksPatch
    {
        public static void Postfix()
        {
            Plugin.Instance.GameObserver.OnPossibleSelectionChange();
        }
    }

    /// <summary>
    /// Harmony patch called when a block is selected. This is required as the observer is count based. If going from one selected to the other, it doesnt trigger.
    /// </summary>
    [HarmonyPatch(typeof(LEV_Selection), "ClickBuilding")]
    public class LEVSelectionClickBuilding
    {
        public static void Postfix()
        {
            Plugin.Instance.GameObserver.OnPossibleSelectionChange();
        }
    }
}
