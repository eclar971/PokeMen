using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System;

public enum BattleState { Start, PlayerAction, PlayerMove, ENemyMove, Busy }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;
    public Grid world;
    public GameObject battleSystem;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;

    PokemenParty playerParty;
    Pokemen wildPokemen;

    public void StartBattle(PokemenParty playerParty, Pokemen wildPokemen)
    {
        this.playerParty = playerParty;
        this.wildPokemen = wildPokemen;
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup(playerParty.GetHealthyPokemen());
        enemyUnit.Setup(wildPokemen);
        playerHud.SetData(playerUnit.Pokemen);
        enemyUnit.Pokemen.Level = wildPokemen.Level;
        enemyHud.SetData(enemyUnit.Pokemen);

        dialogBox.SetMoveNames(playerUnit.Pokemen.Moves);

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemen.Base.Name} appeared.");

        playerAction();
    }

    void playerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("Choose an action"));
        dialogBox.EnableActionSelector(true);
    }

    void playerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }
    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;
        var move = playerUnit.Pokemen.Moves[currentMove];
        yield return dialogBox.TypeDialog($"{playerUnit.Pokemen.Base.Name} used {move.Base.Name}");

        playerUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1);

        enemyUnit.PlayHitAnimation();

        var damageDetails = enemyUnit.Pokemen.TakeDamage(move, playerUnit.Pokemen);
        yield return enemyHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemen.Base.name} Fainted");
            enemyUnit.PlayFaintAnimation();

            int expYeild = enemyUnit.Pokemen.Base.ExpYeild;
            int enemyLevel = enemyUnit.Pokemen.Level;

            int expGain = Mathf.FloorToInt(expYeild * enemyLevel / 7);
            playerUnit.Pokemen.Exp += expGain;
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemen.Base.name} gained {expGain} exp");
            yield return playerHud.SetExpSmooth();

            while (playerUnit.Pokemen.CheckForLevelUp())
            {
                playerUnit.Pokemen.HP = playerUnit.Pokemen.MaxHp;
                yield return playerHud.UpdateHP();
                playerHud.SetLvl();
                yield return dialogBox.TypeDialog($"{playerUnit.Pokemen.Base.name} grew to level {playerUnit.Pokemen.Level}");

                yield return playerHud.SetExpSmooth(true);
            }

            yield return new WaitForSeconds(2f);
            OnBattleOver(true);
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }
    IEnumerator EnemyMove()
    {
        state = BattleState.Busy;

        var move = enemyUnit.Pokemen.GetRandomMove();
        yield return dialogBox.TypeDialog($"{enemyUnit.Pokemen.Base.Name} used {move.Base.Name}");

        enemyUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1);

        playerUnit.PlayHitAnimation();

        var damageDetails = playerUnit.Pokemen.TakeDamage(move, enemyUnit.Pokemen);
        yield return playerHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemen.Base.name} Fainted");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);

            var nextPokemon = playerParty.GetHealthyPokemen();
            if (nextPokemon != null)
            {
                playerUnit.Setup(nextPokemon);
                playerHud.SetData(nextPokemon);

                dialogBox.SetMoveNames(nextPokemon.Moves);

                yield return dialogBox.TypeDialog($"Go {nextPokemon.Base.Name}");

                playerAction();
            }
            else
            {
                playerUnit.Pokemen.HP = playerUnit.Pokemen.MaxHp;
                yield return playerHud.UpdateHP();
                OnBattleOver(false);
            }
        }
        else
        {
            playerAction();
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("A critical hit!");

        if (damageDetails.TypeEffectiveness > 1f)
            yield return dialogBox.TypeDialog("It's super effective!");
        if (damageDetails.TypeEffectiveness < 1f)
            yield return dialogBox.TypeDialog("It's not very effective!");
    }
    public void HandleUpdate()
    {
        if (state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (currentAction < 1)
                ++currentAction;
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            if (currentAction > 0)
                --currentAction;
        }

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                //fight
                playerMove();
            }
            else if (currentAction == 1)
            {
                //run
                StartCoroutine(dialogBox.TypeDialog("You Ran away safely"));
                OnBattleOver(false);
            }
        }
    }
    void HandleMoveSelection()
    {
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            if (currentMove < playerUnit.Pokemen.Moves.Count - 1)
                ++currentMove;
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            if (currentMove > 0)
                --currentMove;
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (currentMove < playerUnit.Pokemen.Moves.Count - 2)
                currentMove += 2;
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            if (currentMove > 1)
                currentMove -= 2;
        }

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemen.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerMove());
        }
    }
}
