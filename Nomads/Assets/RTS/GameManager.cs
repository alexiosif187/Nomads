using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RTS;

/**
 * Singleton that handles the management of game state. This includes
 * detecting when a game has been finished and what to do from there.
 */

public class GameManager : MonoBehaviour
{

    public Tank tankVictory;
    public TankPlayer tankPlayer;

    private static bool created = false;
    private bool initialised = false;
    private HUD hud;


    void Update()
    {
        if (victory())
            GetComponentInChildren<VictoryMenu>().enabled = true;
        if (defeat())
            GetComponentInChildren<DefeatMenu>().enabled = true;

    }

    public bool victory()
    {
        if (tankVictory.hitPoints <10)
            return true;
        return false;
    }

    public bool defeat()
    {
        if (tankPlayer.hitPoints <10)
            return true;
        return false;
    }

    public void Awake()
    {
        Time.timeScale = 0.0f;
      //  GetComponent<UserInput>().enabled = false;
       // Cursor.visible = true;
       // ResourceManager.MenuOpen = true;
    } 
}