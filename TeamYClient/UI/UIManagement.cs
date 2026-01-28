using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamYClient.UI
{
    public class UIManagement
    {
        public static Color lightestGreen = new Color(0.336f, 1f, 0.766f);
        public static Color lightGreen = new Color(0f, 1f, 0.656f, 1f);
        public static Color green = new Color(0, 0.82f, 0.547f, 1f);
        public static Color darkGreen = new Color(0, 0.547f, 0.371f, 1f);
        public static Color darkestGreen = new Color(0, 0.348f, 0.238f, 1f);
        public static Color grey = new Color(0.3f, 0.3f, 0.3f, 1f);
        public static Color darkgrey = new Color(0.2f, 0.2f, 0.2f, 1f);

        public void CreateMainMenuUI()
        {
            //Get the two current buttons.
            OpenUIOnStart mainmenu_canvas = GameObject.FindObjectOfType<OpenUIOnStart>();
            Transform levelEditorGUI = mainmenu_canvas.transform.Find("LevelEditorGUI").transform;
            RectTransform workshopButton = levelEditorGUI.Find("Workshop Button").GetComponent<RectTransform>();
            RectTransform levelEditorButton = levelEditorGUI.Find("Start Level Editor Button").GetComponent<RectTransform>();

            //Calculate the current spacing between the two buttons.
            float buttonSpacing = Mathf.Abs(workshopButton.anchorMax.x - levelEditorButton.anchorMin.x);
            float buttonHeight = Mathf.Abs(levelEditorButton.anchorMax.y - levelEditorButton.anchorMin.y);

            //Create a copy of the level editor button.
            RectTransform editorOnlineButton = GameObject.Instantiate(levelEditorButton.transform, levelEditorButton.transform.parent).GetComponent<RectTransform>();

            //Make the level editor button half size with half spacing.
            levelEditorButton.anchorMin = new Vector2(levelEditorButton.anchorMin.x, levelEditorButton.anchorMin.y + (buttonHeight / 2) + (buttonSpacing / 2));

            //Set the new button on the other half
            editorOnlineButton.anchorMax = new Vector2(editorOnlineButton.anchorMax.x, editorOnlineButton.anchorMax.y - (buttonHeight / 2) - (buttonSpacing / 2));

            //Remove the listener of the new button.
            GenericButton editorOnlineGenericButton = editorOnlineButton.GetComponent<GenericButton>();
            editorOnlineGenericButton.normalColor = green;
            editorOnlineGenericButton.hoverColor = lightGreen;
            editorOnlineGenericButton.clickColor = lightestGreen;

            editorOnlineGenericButton.buttonImage.color = editorOnlineGenericButton.normalColor;

            editorOnlineGenericButton.onClick.RemoveAllListeners();
            for (int i = editorOnlineGenericButton.onClick.GetPersistentEventCount() - 1; i >= 0; i--)
            {
                editorOnlineGenericButton.onClick.SetPersistentListenerState(i, UnityEngine.Events.UnityEventCallState.Off);
            }

            //Add a new listener
            editorOnlineGenericButton.onClick.AddListener(OnEditorOnlineButton);

            //Get the text component, remove the localizer and change the text
            TextMeshProUGUI buttonText = editorOnlineGenericButton.GetComponentInChildren<TextMeshProUGUI>();
            GameObject.Destroy(buttonText.GetComponent<I2.Loc.Localize>());
            buttonText.text = "TeamX Editor";

            // Find the splitscreen image and apply it to the teamx button.
            Image targetImage = FindImageByName("Players Icon 3");

            if (targetImage != null)
            {
                // Get the second child named "Image" from the editorOnlineGenericButton
                Transform secondChild = editorOnlineGenericButton.transform.GetChild(1); // 1 for the second child (0-based index)
                if (secondChild != null && secondChild.name == "Image")
                {
                    Image teamxButtonImage = secondChild.GetComponent<Image>();

                    if (teamxButtonImage != null)
                    {
                        teamxButtonImage.sprite = targetImage.sprite;
                    }
                }
            }
        }

        public static Image FindImageByName(string imageName)
        {
            // Find all Image components in the scene
            Image[] allImages = GameObject.FindObjectsOfType<Image>(true);

            // Find the one with the specific name
            return allImages.FirstOrDefault(image => image.name == imageName);
        }

        private void OnEditorOnlineButton()
        {
            try
            {
                //Plugin.Instance.client.AttemptToConnectToServer();
                //PlayerManager.Instance.weLoadedLevelEditorFromMainMenu = true;
            }
            catch (Exception e)
            {
                /*Plugin.Instance.Log(e.Message, LogType.Error);

                if (Plugin.Instance.client != null)
                {
                    Plugin.Instance.client.AttemptDisconnect();
                }*/
            }
        }
    }
}
