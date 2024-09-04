using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    SpriteRenderer renderer;
    [SerializeField] Sprite[] tileGraphics;
    [SerializeField] LayerMask obstacleLayer;

    [SerializeField] Color highlightedColor;
    [SerializeField] Color creatableColor;

    bool isCreatable;
    bool isWalkable;
    GameManager gm;


    void Start()
    {
        GetRandomSprite();
        gm = FindObjectOfType<GameManager>();
    }

    void GetRandomSprite()
    {
        renderer = GetComponent<SpriteRenderer>();
        int randTile = Random.Range(0, tileGraphics.Length);
        renderer.sprite = tileGraphics[randTile];
    }

    void OnMouseDown() {

        if(isWalkable && (gm.GetSelectedUnit() != null))
        {
            StartMovement();
        }
        else if (isCreatable)
        {
            StartCreation();
        }
    }

    public bool IsClear()
    {
        Collider2D obstacle = Physics2D.OverlapCircle(transform.position, 0.2f, obstacleLayer);
        // Check any obstacles like units or villages etc.
        if(obstacle != null)
        {
            return false;
        }
        return true;
    }

    public void Highlight()
    {
        // Moveable tiles.
        renderer.color = highlightedColor;
        isWalkable = true;
    }

    public void ResetHighlight()
    {
        renderer.color = Color.white;
        isWalkable = false;
        isCreatable = false;
    }

    public void SetCreatable()
    {   
        // Tiles to create new units.
        renderer.color = creatableColor;
        isCreatable = true;
    }

    private void StartMovement()
    {
        gm.GetSelectedUnit().Move(this.transform.position);
    }

    private void StartCreation()
    {
        // Get tiles position.
        Vector2 pos = new(this.transform.position.x, transform.position.y);
        BarrackItem item = Instantiate(gm.GetPurchasedItem(), pos, Quaternion.identity);
        gm.ResetTiles();
        gm.BuyItem();

        // Check if its a unit and not a village.
        Unit unit = item.GetComponent<Unit>();
        if (unit != null) 
        {
            // Newly created units cannot move or attack.
            unit.SetHasMoved(true);
            unit.SetHasAttacked(true);
        }
    }
}
