using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class PinballBall : MonoBehaviour
{
    [Header("Growth Settings")]

    public float growthPerCollision = 0.1f;

  
    public float maxScale = 3f;

    [Header("Launch Settings")]
   
    public Vector2 initialImpulse = new Vector2(0f, 300f);

    private bool launched = false;
    private Rigidbody2D rb2d;
    private Vector3 initialLocalScale;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    
        initialLocalScale = transform.localScale;
    }

    private void Start()
    {

        LaunchBall();
    }

   
    public void LaunchBall()
    {
        if (!launched)
        {
            launched = true;
            rb2d.AddForce(initialImpulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       
        Vector3 newScale = transform.localScale + Vector3.one * growthPerCollision;
        float maxS = initialLocalScale.x * maxScale;
        if (newScale.x > maxS) newScale = new Vector3(maxS, maxS, maxS);

        transform.localScale = newScale;
    }
}
