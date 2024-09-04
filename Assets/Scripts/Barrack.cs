using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Barrack : MonoBehaviour
{
    [SerializeField] Button player1ToggleButton;
    [SerializeField] Button player2ToggleButton;

    [SerializeField] GameObject player1Menu;
    [SerializeField] GameObject player2Menu;

    GameManager gm;


    void Start()
    {
        gm = GetComponent<GameManager>();
    }

    void Update()
    {
        ActivateButton();
    }

    private void ActivateButton()
    {
        if (gm.GetPlayerTurn() == 1)
        {
            player1ToggleButton.interactable = true;
            player2ToggleButton.interactable = false;
        }
        else
        {
            player2ToggleButton.interactable = true;
            player1ToggleButton.interactable = false;
        }
    }

    public void ToggleMenu(GameObject menu)
    {
        menu.SetActive(!menu.activeSelf);
    }

    public void CloseMenus()
    {
        player1Menu.SetActive(false);
        player2Menu.SetActive(false);
    }

    public void BuyItem(BarrackItem item)
    {
        if (gm.GetPlayerTurn() == 1 && item.GetCost() <= gm.GetPlayerGold(1))
        {
            CloseMenus();
        }
        else if (gm.GetPlayerTurn() == 2 && item.GetCost() <= gm.GetPlayerGold(2))
        {
            CloseMenus();
        }
        else
        {
            // Not enough gold :/
            return;
        }
        
        gm.SetPurchasedItem(item);

        // Deselect all units.
        if (gm.GetSelectedUnit() != null)
        {
            gm.GetSelectedUnit().SetIsSelected(false);
            gm.SetSelectedUnit(null);
        }

        GetCreatableTiles();
    }

    void GetCreatableTiles()
    {
        foreach (Tile tile in FindObjectsByType<Tile>(FindObjectsSortMode.None))
        {
            if (tile.IsClear())
            {
                if(gm.GetPlayerTurn() == 1 && tile.transform.position.x >= 0)
                {
                    tile.SetCreatable();
                }
                else if(gm.GetPlayerTurn() == 2 && tile.transform.position.x < 0)
                {
                    tile.SetCreatable();
                }
            }
        }
    }
}
