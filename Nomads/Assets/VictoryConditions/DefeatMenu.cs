using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class DefeatMenu : MonoBehaviour
{

    public GUISkin mySkin;
    public Texture2D header;

    private Player player;
    private string[] buttons = { "Exit Game" };

    void Start()
    {
        player = transform.root.GetComponent<Player>();
    }

    void Update()
    {
    }

    void OnGUI()
    {
        GUI.skin = mySkin;

        float groupLeft = Screen.width / 2 - ResourceManager.MenuWidth / 2;
        float groupTop = Screen.height / 2 - ResourceManager.PauseMenuHeight / 2;
        GUI.BeginGroup(new Rect(groupLeft, groupTop, ResourceManager.MenuWidth, ResourceManager.PauseMenuHeight));

        //background box
        GUI.Box(new Rect(0, 0, ResourceManager.MenuWidth, ResourceManager.PauseMenuHeight), "");
        //header image
        GUI.DrawTexture(new Rect(ResourceManager.Padding, ResourceManager.Padding, ResourceManager.HeaderWidth, ResourceManager.HeaderHeight), header);

        //menu buttons
        float leftPos = ResourceManager.MenuWidth / 2 - ResourceManager.ButtonWidth / 2;
        float topPos = 2 * ResourceManager.Padding + header.height;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i > 0) topPos += ResourceManager.ButtonHeight + ResourceManager.Padding;
            if (GUI.Button(new Rect(leftPos / 3, topPos, ResourceManager.ButtonWidth * 2 - ResourceManager.Padding * 10, ResourceManager.ButtonHeight), buttons[i]))
            {
                switch (buttons[i])
                {
                    case "Exit Game": ExitGame(); break;
                    default: break;
                }
            }
            GUI.Button(new Rect(leftPos / 3, topPos + ResourceManager.ButtonHeight + 2, ResourceManager.MenuWidth - ResourceManager.Padding * 10, ResourceManager.PauseMenuHeight - ResourceManager.Padding * 10),
                "Oh no!\n\n" +
                "You have been\n" +
                "DEFEATED!"
                );
        }

        GUI.EndGroup();
    }

    public void BacktoMain()
    {
        Time.timeScale = 0.0f;
        GetComponent<TutorialMenu>().enabled = false;
        GetComponentInChildren<PauseMenu>().enabled = true;
        GetComponent<UserInput>().enabled = false;
        ResourceManager.MenuOpen = true;
    }

    private void ExitGame()
    {
        Application.Quit();
    }

}