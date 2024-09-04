using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class King : MonoBehaviour
{
    [SerializeField] TMP_Text kingHealth;
    [SerializeField] GameObject victoryPanel;

    [SerializeField] Button button;

    public void SetVictoryPanel()
    {
        victoryPanel.SetActive(true);
        // Disable all colliders to prevent any interaction with game.
        foreach (Collider2D col in FindObjectsByType<Collider2D>(FindObjectsSortMode.None))
        {
            col.enabled = false;
        }
        // Disable barrack buttons when game ends
        button.GetComponent<Button>().enabled = false;
    }

    public void UpdateKingHealth(int health)
    {
        kingHealth.SetText(health.ToString());
    }
}
