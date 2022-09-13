using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class TutorialMenu : MonoBehaviour
{

    public GUISkin mySkin;
    public Texture2D header;

    private Player player;
    private string[] buttons = { "Back to Main Menu"};

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

        float groupLeft = Screen.width/10 - ResourceManager.MenuWidth/10;
        float groupTop = Screen.height/10 - ResourceManager.PauseMenuHeight/10;
        GUI.BeginGroup(new Rect(groupLeft, groupTop, ResourceManager.MenuWidth*3, ResourceManager.PauseMenuHeight*3));

        //background box
        GUI.Box(new Rect(0, 0, ResourceManager.MenuWidth*3, ResourceManager.PauseMenuHeight*3), "");
        //header image
        GUI.DrawTexture(new Rect(ResourceManager.Padding, ResourceManager.Padding, ResourceManager.HeaderWidth, ResourceManager.HeaderHeight), header);

        //menu buttons
        float leftPos = ResourceManager.MenuWidth / 2 - ResourceManager.ButtonWidth / 2;
        float topPos = 2 * ResourceManager.Padding + header.height;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i > 0) topPos += ResourceManager.ButtonHeight + ResourceManager.Padding;
            if (GUI.Button(new Rect(leftPos/3, topPos, ResourceManager.ButtonWidth*2-10, ResourceManager.ButtonHeight), buttons[i]))
            {
                switch (buttons[i])
                {
                    case "Back to Main Menu": BacktoMain(); break;
                    default: break;
                }
            }
            GUI.Button(new Rect(leftPos/3, topPos+ResourceManager.ButtonHeight+2, ResourceManager.MenuWidth*3-ResourceManager.Padding*3, ResourceManager.PauseMenuHeight*3-ResourceManager.Padding*10),
                "Welcome and thank you for playing my game! \n\n" +
                "Here are some basic controls:\n" +
                "Move cursor to edges: moves camera left,right,forward,backwards.\n" +
                "Scroll mouse: moves camera up and down\n" +
                "Alt + left click: allows camera rotation (up,down,left,right)\n" +
                "Right click: deselects object\n" +
                "Left click (if none selected): selects object\n" +
                "Left click (if blue tank selected): moves to indicated position on purple or blue tiles \n\n" +
                "Left click (if blue tank or blue turrets selected): attacks enemy tank or turret\n" +
                "Shift + right click: teleports the tank on blue tiles\n" +
                "Escape: pauses and resumes game\n" +
                "And some basic info to help you out:\n\n" +
                "You can attack the enemy tanks and turrets, but this is not how you win.\n" +
                "Move FAST and only destroy the 2 tanks in areas with a light shinning on them from above.\n" +
                "Turrets can't move, but are very POWERFUL, watch out for them.\n" +
                "You have your own turrets and should use them (e.g. lure tanks to them or attack enemy turrets).\n" +
                "Your turrets won't always attack on their own, but you can direct them to attack what you want.\n"+
                "The enemy tanks and turrets will only attack the player tank.You LOSE if your tank gets destroyed.\n" +
                "You win the game by destroying the 2 tanks in brightly light-up areas.\n\n " +
                "GOOD LUCK!"
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

}