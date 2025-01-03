using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PaddleController : MonoBehaviour
{
  
    public bool isLeftPaddle = true;
  
    public float activeAngle = 60f;
   
    public float rotateSpeed = 360f;
   
    public float restAngle = 0f;
    
    public float hitForce = 5f;

    private float currentAngle = 0f;
    private float targetAngle = 0f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true; 
    }

    void Update()
    {
        
        if (isLeftPaddle)
        {
          
            if (Input.GetKey(KeyCode.LeftArrow))
                targetAngle = -activeAngle;
            else
                targetAngle = restAngle;
        }
        else
        {
           
            if (Input.GetKey(KeyCode.RightArrow))
                targetAngle = activeAngle;
            else
                targetAngle = restAngle;
        }

       
        currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, rotateSpeed * Time.deltaTime);
        transform.localEulerAngles = new Vector3(0f, 0f, currentAngle);
    }

  
    private void OnCollisionEnter2D(Collision2D collision)
    {
       
        if (collision.gameObject.CompareTag("Pinball"))
        {
          
            Vector2 normal = Vector2.zero;
            foreach (var contact in collision.contacts)
            {
                normal += contact.normal;
            }
            normal /= collision.contactCount;
            normal.Normalize();

           
            Rigidbody2D ballRb = collision.rigidbody;
            if (ballRb != null)
            {
                Vector2 extraVelocity = normal * hitForce;
                ballRb.velocity += extraVelocity;
            }
        }
    }

}
