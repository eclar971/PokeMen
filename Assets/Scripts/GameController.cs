using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle}
public class GameController : MonoBehaviour
{
    [SerializeField] PlayerControl playerControl;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Grid world;

    GameState state;

    private void Start()
    {
        playerControl.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
    }

    void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        world.enabled = false;

        var playerParty = playerControl.GetComponent<PokemenParty>();
        var wildPokemen = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildPokemen(playerParty.GetHealthyPokemen());

        battleSystem.StartBattle(playerParty, wildPokemen);
    }
    void EndBattle(bool won)
    {
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        state = GameState.FreeRoam;
        world.enabled = true;
        state = GameState.FreeRoam;

    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerControl.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
    }
}
