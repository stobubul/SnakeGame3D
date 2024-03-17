using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHead : MonoBehaviour
{
    public SnakeController movement;

    public SpawnObject spawn;
    
    public AudioSource eatAudioSource;
    public AudioClip eatSound;
    
    void Start()
    {
        eatAudioSource = gameObject.GetComponent<AudioSource>();
        eatAudioSource.clip = eatSound;
    }

    private void Update()
    {
        
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Food")
        {
            movement.AddBodyPart();
            
            Destroy(col.gameObject);
            
            spawn.SpawnFood();
            
            // Ses çal
            eatAudioSource.Play();
        }
        else
        {
            if (movement.IsAlive)
            {
                if(col.transform != movement.BodyParts[1] && Time.time - movement.TimeFromLastRetry > 5)
                    movement.Die();
            }
        }
    }
}
