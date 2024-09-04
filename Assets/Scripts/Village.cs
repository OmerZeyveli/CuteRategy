using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Village : MonoBehaviour
{
    [SerializeField] int goldPerTurn;
    [SerializeField] int playerNumber;

    public int GetPlayerNumber()
    {
        return playerNumber;
    }

    public int GetGoldPerTurn()
    {
        return goldPerTurn;
    }
}
