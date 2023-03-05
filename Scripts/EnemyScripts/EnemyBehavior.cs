using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public Rigidbody2D playerRb;
    public Rigidbody2D rb;
    public GameObject sprite;
    public Transform firePoint;
    public GameObject bulletPrefab;
    private float shootDelay = 0.6f;
    public float minShootDelay, maxShootDelay;
    public float bulletForce = 5f;
    public int numBullets = 3;
    public bool tripleShotEnabled = false;
    private float spread;
    Vector2 playerPos;

    public int numScore;

    //movement variables
    public float speed;
    private float moveDelay = 1f;
    private Vector2 movementDir;

    private Vector2 lastPos;

    public ParticleSystem flameParticles;
    Color color;
    private GameObject scoreObject;

    void Start()
    {
        playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();

        color = new Color(255, 87, 0, 255);

        scoreObject = GameObject.Find("Score");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "PlayerBullet")
        {
            gameObject.GetComponent<Health>().health--;
            Destroy(collision.gameObject);
            FindObjectOfType<AudioManager>().Play("DamageSound");
        }

        if (collision.gameObject.CompareTag("EnemyBullet"))
        {
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            movementDir = -movementDir;
        }

        if (collision.gameObject.layer == 9)
        {
            Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), collision.collider);
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = playerRb.position;
        shootDelay -= Time.deltaTime;

        //Play particle system if player is moving
        if ((Vector2)transform.position != lastPos)
        {
            if (flameParticles.isStopped)
            {
                flameParticles.Play();
            }
        } else
        {
            if (flameParticles.isPlaying)
            {
                flameParticles.Stop();
            }
        }

        if (shootDelay <= 0)
        {
            firePoint.GetComponent<ParticleSystem>().Play();

            if (tripleShotEnabled)
            {
                spread = -numBullets * 2f;
                MultiShot();
                shootDelay = Random.Range(minShootDelay, maxShootDelay);
            } else
            {
                Shoot();
                shootDelay = Random.Range(minShootDelay, maxShootDelay);
            }
        }

        //destroy object when health is 0 (or less)
        if (gameObject.GetComponent<Health>().health <= 0)
        {
            Destroy(gameObject);
            FindObjectOfType<AudioManager>().Play("EnemyDeathSound");
            scoreObject.GetComponent<Score>().UpdateScore(numScore);
        }

        if (FindClosestEnemy().distance <= 3)
        {
            transform.position = Vector2.MoveTowards(transform.position, 
            FindClosestEnemy().closest.transform.position, -1 * speed * Time.deltaTime);
        }

        RandomMove();
    }

    void FixedUpdate()
    {
        //update enemy rotation to look at player
        Vector2 lookDir = playerPos - new Vector2(sprite.transform.position.x, sprite.transform.position.y);
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        sprite.transform.eulerAngles = Vector3.forward * angle;

        //Move enemy
        rb.MovePosition(rb.position + movementDir.normalized * speed * Time.deltaTime);
    }

    void LateUpdate()
    {
        lastPos = transform.position;
    }

    void Shoot()
    {
        //create bullet object
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.tag = "EnemyBullet";
        bullet.layer = 8;
        bullet.GetComponent<SpriteRenderer>().color = color;

        //get rigidbody2D component from spawned bullet
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        //Add randomness to bullet direction
        float angle = Mathf.Atan2(firePoint.up.y, firePoint.up.x) * Mathf.Rad2Deg;
        float spread = Random.Range(-5, 5);
        float bulletrotation = angle + spread;

        //calculate new direction using random angle
        Vector2 movementDir = new Vector2(
            Mathf.Cos(bulletrotation * Mathf.Deg2Rad),
            Mathf.Sin(bulletrotation * Mathf.Deg2Rad)
        ).normalized;

        //add force to bullet
        bulletRb.AddForce(movementDir * bulletForce, ForceMode2D.Impulse);
    }

    void MultiShot()
    {
        for (int i = 0; i < numBullets; i++)
        {
            //Create bullet object
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.tag = "EnemyBullet";
            bullet.layer = 8;
            bullet.GetComponent<SpriteRenderer>().color = color;

            //get rigidbody2D component from spawned bullet
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

            //Add randomness to bullet direction
            float angle = Mathf.Atan2(firePoint.up.y, firePoint.up.x) * Mathf.Rad2Deg;
            Debug.Log(spread);
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

    public (GameObject closest, float distance) FindClosestEnemy()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector2 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector2 goPosition = go.transform.position;
            Vector2 diff = goPosition - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance && go != gameObject)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return (closest, distance);
    }

    public void RandomMove()
    {
        moveDelay -= Time.deltaTime;
        if (moveDelay <= 0)
        {
            movementDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            moveDelay = 1f;
        }
    }
}