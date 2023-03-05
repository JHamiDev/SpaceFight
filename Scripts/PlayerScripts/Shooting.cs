using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{

    public Transform firePoint;
    public GameObject bulletPrefab;

    public ParticleSystem muzzleFlash;

    public float shootDelay = 0.5f;
    public float bulletForce = 4f;

    //power-up variables
    public bool tripleShot = false;
    public bool fastShot = false;
    public bool powerShot = false;

    //Triple Shot power up
    public int numBullets = 3;
    private float spread;

    private Color color;

    void Start()
    {
        color = new Color(0.3f, 0.86f, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        shootDelay -= Time.deltaTime;

        if (!(PlayerMovement.GameIsOver) && !(PauseMenu.GameIsPaused) && !(EnemyWaves.WaveDelayIsActive) && !(EnemyWaves.levelComplete))
        {
            //check if a certain powerup is enabled
            if (Input.GetButton("Fire1") && shootDelay <= 0 && PlayerMovement.isDashing == false)
            {
                muzzleFlash.Play();

                if (tripleShot == true)
                {
                    spread = -numBullets * 2;
                    TripleShot();
                    bulletForce = 4f;
                    shootDelay = 0.7f;
                } else if (fastShot == true)
                {
                    Shoot();
                    bulletForce = 4f;
                    shootDelay = 0.3f;
                } else if (powerShot == true)
                {
                    Shoot();
                    bulletForce = 8f;
                    shootDelay = 0.5f;
                } else
                {
                    Shoot();
                    bulletForce = 4f;
                    shootDelay = 0.5f;
                }
            }
        }
    }

    //default shooting function
    void Shoot()
    {
        //Creates bullet object and adds force to it
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.tag = "PlayerBullet";
        bullet.layer = 7;
        bullet.GetComponent<SpriteRenderer>().color = color;
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
    }

    //triple-shot powerup
    void TripleShot()
    {
        for (int i = 0; i < numBullets; i++)
        {
        //Create bullet object
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.tag = "PlayerBullet";
        bullet.layer = 7;
        bullet.GetComponent<SpriteRenderer>().color = color;

        //get rigidbody2D component from spawned bullet
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        //Add randomness to bullet direction
        float angle = Mathf.Atan2(firePoint.up.y, firePoint.up.x) * Mathf.Rad2Deg;
        float bulletrotation = angle + spread;

        //calculate new direction using random angle
        Vector2 movementDir = new Vector2(
            Mathf.Cos(bulletrotation * Mathf.Deg2Rad),
            Mathf.Sin(bulletrotation * Mathf.Deg2Rad)
        ).normalized;

        //add force
        rb.AddForce(movementDir * bulletForce, ForceMode2D.Impulse);

        spread += (numBullets * 4f) / (numBullets - 1f);
        }
    }
}
