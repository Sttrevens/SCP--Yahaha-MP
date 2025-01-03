using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PinballUIController : MonoBehaviour
{
    [Header("UI References")]
  
    public RectTransform pinballArea;

   
    public RectTransform ballRect;

    
    public List<RectTransform> obstacleList;

    [Header("Ball Settings")]
   
    public Vector2 initialVelocity = new Vector2(200f, 300f);

  
    public float gravity = -200f;

   
    public float growthPerCollision = 4f;

    
    public float maxRadius = 200f;

    
    public float bounceDamping = 0.9f;

    [Header("Game Flow")]
    public bool isPlaying = false;


    private Vector2 velocity;
    private float ballRadius;

    void Start()
    {

    }


    public void StartMinigame()
    {
        isPlaying = true;

        if (pinballArea != null)
        {
            ballRect.anchoredPosition = Vector2.zero;
        }
        else
        {
            ballRect.anchoredPosition = new Vector2(0f, -100f);
        }


        float initialDiameter = ballRect.sizeDelta.x;

        ballRadius = initialDiameter * 0.5f;


        velocity = initialVelocity;
    }


    public void EndMinigame()
    {
        isPlaying = false;
    }

    void Update()
    {
        if (!isPlaying) return;


        velocity.y += gravity * Time.deltaTime;


        Vector2 pos = ballRect.anchoredPosition;
        pos += velocity * Time.deltaTime;


        if (pinballArea != null)
        {

            float halfW = pinballArea.rect.width * 0.5f;
            float halfH = pinballArea.rect.height * 0.5f;


            if (pos.x - ballRadius < -halfW)
            {
                pos.x = -halfW + ballRadius;
                velocity.x = -velocity.x * bounceDamping;
                GrowBall();
            }

            if (pos.x + ballRadius > halfW)
            {
                pos.x = halfW - ballRadius;
                velocity.x = -velocity.x * bounceDamping;
                GrowBall();
            }

            if (pos.y - ballRadius < -halfH)
            {
                pos.y = -halfH + ballRadius;
                velocity.y = -velocity.y * bounceDamping;
                GrowBall();
            }

            if (pos.y + ballRadius > halfH)
            {
                pos.y = halfH - ballRadius;
                velocity.y = -velocity.y * bounceDamping;
                GrowBall();
            }
        }


        ballRect.anchoredPosition = pos;

        foreach (RectTransform obstacle in obstacleList)
        {
            CheckCollisionRotatedRect(obstacle);
        }


    }


    private void GrowBall()
    {
        float newRadius = ballRadius + growthPerCollision;
        newRadius = Mathf.Min(newRadius, maxRadius);
        if (Mathf.Approximately(newRadius, ballRadius)) return;

        ballRadius = newRadius;
        float diameter = ballRadius * 2f;
        ballRect.sizeDelta = new Vector2(diameter, diameter);
    }


    private void CheckCollisionRotatedRect(RectTransform obstacle)
    {
        if (obstacle == null) return;

       
        Matrix4x4 toLocal = obstacle.worldToLocalMatrix;   
        Matrix4x4 toWorld = obstacle.localToWorldMatrix;   

       
        float halfW = obstacle.rect.width * 0.5f;
        float halfH = obstacle.rect.height * 0.5f;

      
        Vector3 ballWorldPos = ballRect.TransformPoint(Vector3.zero);
      

        
        Vector3 ballLocalPos = toLocal.MultiplyPoint3x4(ballWorldPos);

       
        float clampedX = Mathf.Clamp(ballLocalPos.x, -halfW, halfW);
        float clampedY = Mathf.Clamp(ballLocalPos.y, -halfH, halfH);

        Vector2 closestPointLocal = new Vector2(clampedX, clampedY);
        Vector2 deltaLocal = (Vector2)ballLocalPos - closestPointLocal;
        float distSq = deltaLocal.sqrMagnitude;
        float radiusSq = ballRadius * ballRadius;

      
        if (distSq < radiusSq)
        {
            float dist = Mathf.Sqrt(distSq);
            if (dist < 0.0001f)
            {
                dist = ballRadius;
                deltaLocal = Vector2.up * ballRadius;
            }

          
            float penetration = ballRadius - dist;
            Vector2 nLocal = deltaLocal.normalized;
            Vector2 moveLocal = nLocal * penetration;

           
            Vector3 moveWorld = toWorld.MultiplyVector(moveLocal);

            
            var oldBallWorldPos = ballWorldPos;
            var newBallWorldPos = oldBallWorldPos + moveWorld;
           
            ballRect.position = newBallWorldPos;

            
            Vector2 vLocal = toLocal.MultiplyVector(velocity);

            float vDot = Vector2.Dot(vLocal, nLocal);
            vLocal = vLocal - 2f * vDot * nLocal;
            vLocal *= bounceDamping;

            
            Vector2 vWorld = toWorld.MultiplyVector(vLocal);
            velocity = vWorld;

           
            GrowBall();
        }
    }
}