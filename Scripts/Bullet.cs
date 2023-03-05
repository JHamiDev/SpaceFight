using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject hitEffect;
    private GameObject player;

    ParticleSystem.MainModule settings;
    ParticleSystem.MainModule hitSettings;
    Color colorWhite;
    Color colorYellow;

    void Awake()
    {
        FindObjectOfType<AudioManager>().Play("ShootSound");
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        settings = gameObject.GetComponent<ParticleSystem>().main;
        hitSettings = hitEffect.GetComponent<ParticleSystem>().main;
        colorWhite = new Color(0, 175, 255, 255);
        colorYellow = new Color(255, 87, 0, 255);

        if (gameObject.CompareTag("PlayerBullet"))
        {
            settings.startColor = colorWhite;
        } else if (gameObject.CompareTag("EnemyBullet"))
        {
            settings.startColor = colorYellow;
        }

        //Ignore playerbullet collision with player
        if (gameObject.CompareTag("PlayerBullet"))
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player.GetComponent<Collider2D>());
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Destroy bullet on collision
        if ((bulletPrefab.tag == "PlayerBullet" && collision.collider.tag != "Player") || 
        (bulletPrefab.tag == "EnemyBullet" && collision.collider.tag != "Enemy"))
        {
            Destroy(gameObject);
        }

        //Play sound if hit something other than player or enemies
        if (!(collision.collider.CompareTag("Enemy")) && !(collision.collider.CompareTag("Player")))
        {
            FindObjectOfType<AudioManager>().Play("HitSound");
        }

        //Instantiate particle effects on collision
        if (gameObject.tag == "PlayerBullet" && collision.collider.tag != "Player")
        {
            hitSettings.startColor = colorWhite;
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(effect, 1f);
        } else if (gameObject.tag == "EnemyBullet" && collision.collider.tag != "Enemy")
        {
            hitSettings.startColor = colorYellow;
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(effect, 1f);
        }
    }
}
