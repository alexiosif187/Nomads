using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class PauseMenu : MonoBehaviour
{

    public GUISkin mySkin;
    public Texture2D header;

    private Player player;
    private string[] buttons = { "Resume", "Exit Game", "Instructions" };

    public AudioClip clickSound;
    public float clickVolume = 1.0f;

    private AudioElement audioElement;
    void Start()
    {
        player = transform.root.GetComponent<Player>();
        if (clickVolume < 0.0f) clickVolume = 0.0f;
        if (clickVolume > 1.0f) clickVolume = 1.0f;
        List<AudioClip> sounds = new List<AudioClip>();
        List<float> volumes = new List<float>();
        sounds.Add(clickSound);
        volumes.Add(clickVolume);
        audioElement = new AudioElement(sounds, volumes, "PauseMenu", null);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Resume();
    }

    private void Resume()
    {
        Time.timeScale = 1.0f;
        GetComponent<PauseMenu>().enabled = false;
        if (player) player.GetComponent<UserInput>().enabled = true;
        Cursor.visible = false;
        ResourceManager.MenuOpen = false;
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
        float leftPos = ResourceManager.MenuWidth / 3 - ResourceManager.ButtonWidth / 3;
        float topPos = 3 * ResourceManager.Padding + header.height;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i > 0) topPos += ResourceManager.ButtonHeight + ResourceManager.Padding;
            if (GUI.Button(new Rect(leftPos, topPos, ResourceManager.ButtonWidth, ResourceManager.ButtonHeight), buttons[i]))
            {
                switch (buttons[i])
                {
                    case "Resume": Resume(); break;
                    case "Exit Game": ExitGame(); break; 
                    case "Instructions": OpenTutorial(); break ;
                    default: break;
                }
            }
        }

        GUI.EndGroup();
    }

    private void ExitGame()
    {
        Application.Quit();
    }

    public void OpenTutorial()
    {
        Time.timeScale = 0.0f;
        GetComponent<PauseMenu>().enabled = false;
        GetComponent<TutorialMenu>().enabled = true;
        Cursor.visible = false;
    }

    private void PlayClick()
    {
        if (audioElement != null) audioElement.Play(clickSound);
    }
}