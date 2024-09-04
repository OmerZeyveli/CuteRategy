using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BarrackItem : MonoBehaviour
{
    [SerializeField] int cost;

    public int GetCost()
    {
        return cost;
    }
}
