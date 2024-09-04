using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Unit selectedUnit;
    [SerializeField] GameObject selectedUnitSquare;

    int playerTurn = 1; // 1 for blues and 2 for reds.
    bool endGame;

    [SerializeField] Image playerIndicator;
    [SerializeField] Sprite player1Indicator;
    [SerializeField] Sprite player2Indicator;

    [SerializeField] int player1Gold = 100;
    [SerializeField] int player2Gold = 100;

    [SerializeField] TMP_Text player1GoldText;
    [SerializeField] TMP_Text player2GoldText;

    BarrackItem purchasedItem;

    [SerializeField] GameObject statsPanel;
    [SerializeField] Vector2 statsPanelShift;
    Unit viewedUnit;

    [SerializeField] TMP_Text healthText;
    [SerializeField] TMP_Text armorText;
    [SerializeField] TMP_Text damageText;
    [SerializeField] TMP_Text deflectText;


    // Setter Getters.
    public void SetSelectedUnit(Unit selectedUnit)
    {
        this.selectedUnit = selectedUnit;
    }

    public void SetEndGame(bool endGame)
    {
        this.endGame = endGame;
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public int GetPlayerTurn()
    {
        return playerTurn;
    }

    public int GetPlayerGold(int player)
    {
        if (player == 1)
        {
            return player1Gold;
        }
        else
        {
            return player2Gold;
        }
    }

    public void AddToPlayerGold(int player, int gold)
    {
        if (player == 1)
        {
            player1Gold += gold;
        }
        else
        {
            player2Gold += gold;
        }
    }

    public BarrackItem GetPurchasedItem()
    {
        return purchasedItem;
    }

    public void SetPurchasedItem(BarrackItem item)
    {
        this.purchasedItem = item;
    }


    // Methods
    void Start()
    {
        GetGoldIncome(1);
        GetComponent<Barrack>().CloseMenus();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !endGame)
        {
            EndTurn();
        }
        MoveSelectionSquare();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void MoveSelectionSquare()
    {
        if (selectedUnit != null)
        {
            selectedUnitSquare.SetActive(true);
            selectedUnitSquare.transform.position = selectedUnit.transform.position;
        }
        else
        {
            selectedUnitSquare.SetActive(false);
        }
    }

    // Stats Panel section ----------------------------------------------
    public void ToggleStatsPanel(Unit unit)
    {
        if (!unit.Equals(viewedUnit))
        {
            // Open panel.
            statsPanel.SetActive(true);
            viewedUnit = unit;
            MoveStatsPanel(unit);
            UpdateStatsPanel();
        }
        else
        {
            // Close panel if clicked twice.
            statsPanel.SetActive(false);
            viewedUnit = null;
        }
    }

    public void UpdateStatsPanel()
    {
        if (viewedUnit != null)
        {
            // Update data in panel
            healthText.SetText(viewedUnit.GetHealth().ToString());
            armorText.SetText(viewedUnit.GetArmor().ToString());
            damageText.SetText(viewedUnit.GetDamage().ToString());
            deflectText.SetText(viewedUnit.GetDeflect().ToString());
        }
    }

    public void MoveStatsPanel(Unit unit)
    {
        if (unit.Equals(viewedUnit))
        {
            statsPanel.transform.position = (Vector2)unit.transform.position + statsPanelShift;
        }
    }

    public void RemoveStatsPanel(Unit unit)
    {
        // Dont remove if its not the one viewed
        if (unit.Equals(viewedUnit))
        {
            // Remove panel incase character dies.
            statsPanel.SetActive(false);
            viewedUnit = null;
        }
    }
    // End of Stats Panel section ----------------------------------------------

    public void BuyItem()
    {
        AddToPlayerGold(playerTurn, purchasedItem.GetCost() * -1); // to substract ve add a negative number
        UpdateGoldText();
    }

    void GetGoldIncome(int playerTurn)
    {
        // Collect gold from villages each turn.
        foreach (Village vlg in FindObjectsByType<Village>(FindObjectsSortMode.None))
        {
            if (vlg.GetPlayerNumber() == playerTurn)
            {
                if (playerTurn == 1)
                {
                    player1Gold += vlg.GetGoldPerTurn();
                }
                else
                {
                    player2Gold += vlg.GetGoldPerTurn();
                }
            }
        }

        UpdateGoldText();
    }

    public void UpdateGoldText()
    {
        player1GoldText.SetText(player1Gold.ToString());
        player2GoldText.SetText(player2Gold.ToString());
    }

    public void ResetTiles()
    {
        foreach (Tile tile in FindObjectsOfType<Tile>())
        {
            tile.ResetHighlight();
        }
    }

    void EndTurn()
    {
        if (playerTurn == 1)
        {
            playerTurn = 2;
            playerIndicator.sprite = player2Indicator;
        }
        else
        {
            playerTurn = 1;
            playerIndicator.sprite = player1Indicator;
        }

        GetGoldIncome(playerTurn);

        // Deselect units.
        if (selectedUnit != null)
        {
            selectedUnit.SetIsSelected(false);
            selectedUnit = null;
        }

        ResetTiles();

        // Reset units movement and attack restrains.
        foreach (Unit unit in FindObjectsByType<Unit>(FindObjectsSortMode.None))
        {
            unit.SetHasMoved(false);
            unit.SetWeaponIconActive(false);
            unit.SetHasAttacked(false);
        }

        GetComponent<Barrack>().CloseMenus();
    }

    public void RestartGame()
    {
        var currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
    }
}
