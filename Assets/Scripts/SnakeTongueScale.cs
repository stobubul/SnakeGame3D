using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeTongueScale : MonoBehaviour
{
    public Vector3 targetScale = new Vector3(0.1593138f, 0.18f, 1.2595f);
    public float duration = 5f;

    private Vector3 initialScale;
    private float timer = 0f;
    
    private bool growing = true;

    private SnakeController movement;
    void Start()
    {
        initialScale = transform.localScale;
        movement = FindObjectOfType<SnakeController>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > duration)
        {
            timer = 0f;
            growing = !growing;
        }

        float t = timer / duration; // Geçen sürenin yüzdesi

        if (movement.IsAlive)
        {
            if (growing)
                transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            else
                transform.localScale = Vector3.Lerp(targetScale, initialScale, t);
        }
    }
}


