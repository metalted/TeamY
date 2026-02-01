using Imui.Controls;
using Imui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZeepSDK.UI;

namespace TeamYClient.UI
{
    public class TeamToolbarDrawer : IZeepToolbarDrawer
    {
        public string MenuTitle => "TeamY";
        private UIManagement _ui;
        public TeamToolbarDrawer(UIManagement ui)
        {
            _ui = ui;
        }

        public void DrawMenuItems(ImGui gui)
        {
            if (gui.Menu("Admin Panel"))
            {
                _ui.OpenAdminWindow();
            }

            if (gui.Menu("Chat"))
            {
                _ui.OpenChatWindow();
            }
        }
    }
}
