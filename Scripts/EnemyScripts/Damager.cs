using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour
{
    public GameObject player;

    public Rigidbody2D rb;

   void OnCollisionEnter2D(Collision2D collision)
   {
       player = GameObject.FindGameObjectWithTag("Player");

       //reference player's rigidbody
       Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
       
       float force = 8000f;

       if (collision.gameObject == player)
       {
           //calculate direction vector
           Vector2 dir = rb.position - playerRb.position;

           //get opposite -(Vector2) and normalize it
           dir = -dir.normalized;

           //add force to player
           playerRb.AddForce(dir*force);
       }
   }
}
