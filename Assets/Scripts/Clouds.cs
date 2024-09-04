using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clouds : MonoBehaviour
{
    [SerializeField] float minSpeed;
    [SerializeField] float maxSpeed;
    float speed;

    // Edges of screen.
    [SerializeField] float maxX;
    [SerializeField] float minX;

    private void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);
    }

    private void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector2.right);
        if (transform.position.x > maxX) {
            transform.position = new Vector2(minX, transform.position.y);
        } 
    }

}
