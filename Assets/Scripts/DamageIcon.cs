using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIcon : MonoBehaviour
{
    [SerializeField] Sprite[] damageSprites;

    [SerializeField] float lifetime;

    [SerializeField] GameObject effect;
    

    void Start() {
        Invoke(nameof(Destruction), lifetime); 
    }

    public void Setup(int damage)
    {
        // Changes sprite according to damage.
        GetComponent<SpriteRenderer>().sprite = damageSprites[damage - 1];
    }

    void Destruction()
    {
        Instantiate(effect, transform.position, transform.rotation); // Particles
        Destroy(gameObject);
    }
}
