using Imui.Controls;
using Imui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamYShared.Permissions;
using ZeepSDK.UI;

namespace TeamYClient.UI
{
    public class TeamGUIDrawer : IZeepGUIDrawer
    {
        private UIManagement _ui;
        private bool mouseInsideAdminWindow;
        private bool mouseInsideChatWindow;

        public TeamGUIDrawer(UIManagement ui)
        {
            _ui = ui;
        }

        public void OnZeepGUI(ImGui gui)
        {
            bool adminWindowWasOpen = _ui.AdminWindowIsOpen;
            bool chatWindowWasOpen = _ui.ChatWindowIsOpen;

            if (_ui.AdminWindowIsOpen)
                DrawAdminWindow(gui);

            if (_ui.ChatWindowIsOpen)
                DrawChatWindow(gui);

            if (adminWindowWasOpen && !_ui.AdminWindowIsOpen)
                _ui.ReleaseMouseBlock();

            if (chatWindowWasOpen && !_ui.ChatWindowIsOpen)
                _ui.ReleaseMouseBlock();
        }
        private void WindowOpened() { }
        private void WindowClosed() { }

        private void DrawAdminWindow(ImGui gui)
        {
            if (gui.BeginWindow("TeamY - Admin Window", ref _ui.AdminWindowIsOpen, ref mouseInsideAdminWindow, WindowOpened, WindowClosed, _ui.MouseEnteredWindowRect, _ui.MouseExitedWindowRect, (800, 600)))
            {
                var rect = gui.AddLayoutRect(gui.GetLayoutWidth(), gui.GetLayoutHeight());
                gui.BeginTabsPane(rect);

                if (gui.BeginTab("Development"))
                {
                    DrawDevelopmentTab(gui);
                    gui.EndTab();
                }
                
                gui.EndTabsPane();

                gui.EndWindow();
            }
        }
        private void DrawChatWindow(ImGui gui)
        {
            if (!gui.BeginWindow(
                "TeamX - Chat",
                ref _ui.ChatWindowIsOpen,
                ref mouseInsideChatWindow,
                WindowOpened,
                WindowClosed,
                _ui.MouseEnteredWindowRect,
                _ui.MouseExitedWindowRect,
                (800, 600)))
                return;

            gui.Text("Chat");

            gui.EndWindow();
        }


        #region Developer Tab
        private string limitTextInput;
        private int limitValueInput;
        private string permissionTextInput;

        private void DrawPermissionCheckbox(ImGui gui, string perm)
        {
            var group = Plugin.Instance.Permissions.Group;
            bool hasPerm = group.Permissions.Contains(perm);

            bool checkboxValue = hasPerm;
            if (gui.Checkbox(ref checkboxValue, perm))
            {
                if (checkboxValue && !hasPerm)
                {
                    group.Permissions.Add(perm);
                }
                else if (!checkboxValue && hasPerm)
                {
                    group.Permissions.Remove(perm);
                }
            }
        }

        private void DrawDevelopmentTab(ImGui gui)
        {
            var gameData = Plugin.Instance.GameData;
            var perms = Plugin.Instance.Permissions;
            var group = perms.Group;

            gui.BeginHorizontal();
            gui.Text("SteamID:");
            gui.Text(gameData.Local.SteamID.ToString());
            gui.EndHorizontal();

            gui.BeginHorizontal();
            gui.Text("Position:");
            gui.Text(gameData.Local.Position.ToString());
            gui.EndHorizontal();

            gui.BeginHorizontal();
            gui.Text("Euler:");
            gui.Text(gameData.Local.EulerRotation.ToString());
            gui.EndHorizontal();

            gui.BeginHorizontal();
            gui.Text("Mode:");
            gui.Text(gameData.Local.Mode.ToString());
            gui.EndHorizontal();

            gui.BeginHorizontal();
            gui.Text("GameState:");
            gui.Text(gameData.State.ToString());
            gui.EndHorizontal();

            gui.Separator();
            gui.Text("Editor Permissions");

            DrawPermissionCheckbox(gui, CorePerms.EDITOR_CREATE);
            DrawPermissionCheckbox(gui, CorePerms.EDITOR_DESTROY);
            DrawPermissionCheckbox(gui, CorePerms.EDITOR_UPDATE_ALL);
            DrawPermissionCheckbox(gui, CorePerms.EDITOR_UPDATE_SELF);
            DrawPermissionCheckbox(gui, CorePerms.EDITOR_UPDATE_SKYBOX);
            DrawPermissionCheckbox(gui, CorePerms.EDITOR_UPDATE_FLOOR);

            gui.Separator();
            gui.Text("Raw Permissions");

            gui.BeginHorizontal();
            gui.Text("Permission:");
            gui.TextEdit(ref permissionTextInput, new ImSize(200, gui.GetRowHeight()));

            if (gui.Button("+"))
            {
                if (!string.IsNullOrWhiteSpace(permissionTextInput))
                {
                    Plugin.Instance.Permissions.Group.Permissions.Add(permissionTextInput);
                    permissionTextInput = string.Empty;
                }
            }

            gui.EndHorizontal();

            var permissionList = Plugin.Instance.Permissions.Group.Permissions;

            foreach (var perm in permissionList.ToList())
            {
                gui.BeginHorizontal();

                gui.Text(perm);

                if (gui.Button("X"))
                {
                    permissionList.Remove(perm);
                }

                gui.EndHorizontal();
            }

            gui.Separator();
            gui.Text("Limits");

            gui.BeginHorizontal();

            gui.Text("Name:");
            gui.TextEdit(ref limitTextInput, new ImSize(200, gui.GetRowHeight()));

            gui.Text("Value:");
            gui.NumericEdit(ref limitValueInput, new ImSize(100, gui.GetRowHeight()));

            if (gui.Button("+"))
            {
                if (!string.IsNullOrWhiteSpace(limitTextInput))
                {
                    Plugin.Instance.Permissions.Group.Limits[limitTextInput] = limitValueInput;
                }
            }

            gui.EndHorizontal();


            // iterate over copy to allow removal
            foreach (var key in group.Limits.Keys.ToList())
            {
                gui.BeginHorizontal();
                gui.Text($"{key}: {group.Limits[key]}");

                if (gui.Button("X"))
                {
                    group.Limits.Remove(key);
                }

                gui.EndHorizontal();
            }
        }
        #endregion        
    }
}
