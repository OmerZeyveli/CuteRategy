using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using UnityEngine.UI;
using TMPro;

public class Unit : MonoBehaviour
{
    bool isSelected;
    GameManager gm;
    [SerializeField] int playerNumber;

    [SerializeField] int tileSpeed;
    [SerializeField] float moveSpeed;

    [SerializeField] int attackRange = 1;
    List<Unit> enemiesInRange = new();
    [SerializeField] GameObject weaponIcon;

    bool hasMoved;
    bool hasAttacked;

    [SerializeField] int health;
    [SerializeField] int damage;
    [SerializeField] int deflect;
    [SerializeField] int armor;

    [SerializeField] DamageIcon damageIcon;
    [SerializeField] GameObject bloodEffect;

    Animator camAnim;
    AudioSource source;

    bool isKing;
    King king;

    // Getter Setters
    public void SetIsSelected(bool isSelected)
    {
        this.isSelected = isSelected;
    }

    public bool GetIsSelected()
    {
        return isSelected;
    }

    public List<Unit> GetEnemiesInRange()
    {
        return enemiesInRange;
    }

    public void SetWeaponIconActive(bool isActive)
    {
        weaponIcon.SetActive(isActive);
    }

    public void SetHasMoved(bool hasMoved)
    {
        this.hasMoved = hasMoved;
    }

    public void SetHasAttacked(bool hasAttacked)
    {
        this.hasAttacked = hasAttacked;
    }

    public bool GetHasAttacked()
    {
        return hasAttacked;
    }

    public void SetHealth(int health)
    {
        this.health = health;
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetDeflect()
    {
        return deflect;
    }

    public int GetArmor()
    {
        return armor;
    }

    public int GetDamage()
    {
        return damage;
    }

    public bool GetIsKing()
    {
        return isKing;
    }


    // Methods
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        camAnim = Camera.main.GetComponent<Animator>();
        source = GetComponent<AudioSource>();

        ResetWeaponIcons();
        CheckKing();
        UpdateKingHP(health);
    }

    private void CheckKing()
    {
        // Check if unit is king
        king = GetComponent<King>();
        if (king != null)
        {
            isKing = true;
        }
    }

    void UpdateKingHP(int HP)
    {
        if (isKing)
        {
            king.UpdateKingHealth(HP);
        }
    }

    void OnMouseOver()
    {
        // If right-clikced open stats panel.
        if (Input.GetMouseButtonDown(1))
        {
            gm.ToggleStatsPanel(this);
        }
    }

    void OnMouseDown()
    {
        ResetWeaponIcons();

        if (isSelected) // Deselection
        {
            isSelected = false;
            gm.SetSelectedUnit(null);
            gm.ResetTiles();
        }
        else // Selection
        {
            if (playerNumber == gm.GetPlayerTurn()) // Check team turn
            {
                if (gm.GetSelectedUnit() != null) // Deselect other (if any available)
                {
                    gm.GetSelectedUnit().SetIsSelected(false);
                }

                // Select 'this' unit
                isSelected = true;
                source.Play();
                
                gm.SetSelectedUnit(this);
                gm.ResetTiles();

                GetEnemies();
                GetWalkableTiles();
            }
        }

        Collider2D col = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.15f);
        Unit enemy = col.GetComponent<Unit>();
        if (gm.GetSelectedUnit() != null)
        {
            // If clicked on an enemy in range, attack.
            if (gm.GetSelectedUnit().GetEnemiesInRange().Contains(enemy) && !gm.GetSelectedUnit().GetHasAttacked())
            {
                gm.GetSelectedUnit().Attack(enemy);
            }
        }
    }

    void Attack(Unit enemy)
    {
        camAnim.SetTrigger("shake");
        hasAttacked = true;

        // Calculate damage which will hit enemy and current unit.
        int enemyDamage = damage - enemy.GetArmor();
        int myDamage = enemy.GetDeflect() - armor;

        // Damage enemy
        if (enemyDamage >= 1)
        {
            DamageIcon instance = Instantiate(damageIcon, enemy.transform);
            instance.Setup(enemyDamage);
            enemy.SetHealth(enemy.GetHealth() - enemyDamage);
            enemy.UpdateKingHP(enemy.GetHealth());
        }

        // Archers can shoot without getting deflect damage back, only if they are 2 blocks away.
        if (transform.CompareTag("Archer") && !enemy.CompareTag("Archer"))
        {
            // Deflect damage if archer is too close to enemy.
            if (Mathf.Abs(transform.position.x - enemy.transform.position.x) + Mathf.Abs(transform.position.y - enemy.transform.position.y) <= 1)
            {
                // Get deflect damage
                if (myDamage >= 1)
                {
                    DamageIcon instance = Instantiate(damageIcon, transform);
                    instance.Setup(myDamage);
                    health -= myDamage;
                    UpdateKingHP(health);
                }
            }
        }
        else
        {
            // Deflect damage to all other.
            // Also archers can deflect other archers damage.
            if (myDamage >= 1)
            {
                DamageIcon instance = Instantiate(damageIcon, transform);
                instance.Setup(myDamage);
                health -= myDamage;
                UpdateKingHP(health);
            }
        }


        // Kill enemy.
        if (enemy.GetHealth() <= 0)
        {
            if (enemy.GetIsKing())
            {
                king.SetVictoryPanel();
                enemy.UpdateKingHP(0);
                gm.SetEndGame(true);
            }

            Instantiate(bloodEffect, enemy.transform.position, enemy.transform.rotation);
            GetWalkableTiles();
            gm.RemoveStatsPanel(enemy);
            gm.GetComponent<Barrack>().CloseMenus();
            Destroy(enemy.gameObject);
        }

        // Kill me?
        if (health <= 0)
        {
            if (isKing)
            {
                foreach (King enemyKing in FindObjectsByType<King>(FindObjectsSortMode.None))
                {
                    if(!GetComponent<King>().Equals(enemyKing))
                    {
                        enemyKing.SetVictoryPanel();
                    }
                }
                gm.SetEndGame(true);
                UpdateKingHP(0);
            }

            Instantiate(bloodEffect, transform.position, transform.rotation);
            gm.ResetTiles();
            gm.RemoveStatsPanel(this);
            gm.GetComponent<Barrack>().CloseMenus();
            Destroy(gameObject);
        }

        gm.UpdateStatsPanel();
    }

    void GetWalkableTiles()
    {
        // Every unit can move once per turn.
        if (hasMoved)
        {
            return;
        }

        // Find moveable tiles by calculating distance.
        foreach (Tile tile in FindObjectsOfType<Tile>())
        {
            if (Mathf.Abs(transform.position.x - tile.transform.position.x)
             + Math.Abs(transform.position.y - tile.transform.position.y) < tileSpeed)
            {
                // Obstacles prevent movement.
                if (tile.IsClear())
                {
                    tile.Highlight();
                }
            }
        }
    }

    void GetEnemies()
    {
        // Clear old enemies in range from list.
        enemiesInRange.Clear();

        // Check every unit by distance.
        foreach (Unit unit in FindObjectsByType<Unit>(FindObjectsSortMode.None))
        {
            if (Mathf.Abs(transform.position.x - unit.transform.position.x)
              + Mathf.Abs(transform.position.y - unit.transform.position.y) < attackRange)
            {
                // Also check if they are enemy.
                if (unit.playerNumber != gm.GetPlayerTurn() && !hasAttacked)
                {
                    enemiesInRange.Add(unit);
                    unit.weaponIcon.SetActive(true);
                }
            }
        }
    }

    public void ResetWeaponIcons()
    {
        // Disable attackable icons
        foreach (Unit unit in FindObjectsByType<Unit>(FindObjectsSortMode.None))
        {
            unit.weaponIcon.SetActive(false);
        }
    }

    public void Move(Vector2 tilePos)
    {
        gm.ResetTiles();
        // Linear movement with coroutines.
        StartCoroutine(StartMovement(tilePos));
    }

    IEnumerator StartMovement(Vector2 tilePos)
    {
        ResetWeaponIcons();

        // Movement in horizontal x.
        Vector2 tilePosX = new Vector2(tilePos.x, transform.position.y);
        while (transform.position.x != tilePos.x)
        {
            transform.position = Vector2.MoveTowards(transform.position, tilePosX, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Movement in vertical y.
        Vector2 tilePosY = new Vector2(transform.position.x, tilePos.y);
        while (transform.position.y != tilePos.y)
        {
            transform.position = Vector2.MoveTowards(transform.position, tilePosY, moveSpeed * Time.deltaTime);
            yield return null;
        }

        hasMoved = true;
        GetEnemies();
        gm.MoveStatsPanel(this);
    }
}