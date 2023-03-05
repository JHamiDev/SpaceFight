using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    //dashing variables
    public float dashDelay = 1f;
    public Slider dashDelayBar;
    [SerializeField] private float dashInvincibilityTime;
    public static bool isDashing = false;

    //player variables
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Camera cam;
    Vector2 movement;
    Vector2 mousePos;

    Vector2 lastPos;

    private bool isInvincible = false;

    public GameObject GameOverScreenUI;
    public static bool GameIsOver = false;

    public ParticleSystem dashParticles;
    public ParticleSystem flameParticles;

    private GameObject scoreObject;

    private IEnumerator invincibilityCoroutine;

    public static bool playerDeathSoundHasPlayed;

    void Start()
    {
        scoreObject = GameObject.Find("Score");
        invincibilityCoroutine = BecomeTemporarilyInvincible(3, 0.15f);
        lastPos = Vector2.zero;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "EnemyBullet" && !isInvincible)
        {
            gameObject.GetComponent<Health>().health--;
            Destroy(collision.gameObject);
            FindObjectOfType<AudioManager>().Play("DamageSound");
            //reset invincibility
            StopCoroutine(invincibilityCoroutine);
            invincibilityCoroutine = BecomeTemporarilyInvincible(3, 0.15f);
            StartCoroutine(invincibilityCoroutine);
        } else if (collision.gameObject.tag == "EnemyBullet" && isInvincible)
        {
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "Enemy" && !isInvincible)
        {
            gameObject.GetComponent<Health>().health--;
            //restart invincibility
            StopCoroutine(invincibilityCoroutine);
            invincibilityCoroutine = BecomeTemporarilyInvincible(3, 0.15f);
            StartCoroutine(invincibilityCoroutine);
        }

        if (collision.gameObject.CompareTag("HeartPickup"))
        {
            //restart invincibility
            StopCoroutine(invincibilityCoroutine);
            invincibilityCoroutine = BecomeTemporarilyInvincible(3, 0.15f);
            StartCoroutine(invincibilityCoroutine);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //dash move
        dashDelay -= Time.deltaTime;
        dashDelayBar.value = dashDelay;

        //Play particle system if player is moving
        if ((Vector2)transform.position != lastPos)
        {
            if (flameParticles.isStopped)
            {
                flameParticles.Play();
            }
            Dash();
        } else
        {
            if (flameParticles.isPlaying)
            {
                flameParticles.Stop();
            }
        }

        //Display game over screen when health is zero (or less than zero)
        if (gameObject.GetComponent<Health>().health <= 0 && !GameIsOver)
        {
            GameOverScreenUI.SetActive(true);
            Time.timeScale = 0f;
            GameIsOver = true;
            scoreObject.GetComponent<Score>().UpdateGameOverScoreText();

            if (!playerDeathSoundHasPlayed)
            {
                FindObjectOfType<AudioManager>().Play("DeathSound");
            }

            playerDeathSoundHasPlayed = true;
        }

        //input directions
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        //position of mouse on the screen
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    void FixedUpdate()
    {
        //move player
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);

        //set rotation based on mouse position
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    void LateUpdate()
    {
        lastPos = transform.position;
    }

    private void Dash()
    {
        if (Input.GetKeyDown(KeyCode.Space) && dashDelay <= 0)
        {
            float dashSpeed = 4000f;
            //get angle of player movement
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            //convert angle into direction (Vector2)
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
            //add force to player and reset delay
            rb.AddForce(dir * dashSpeed);
            StartCoroutine(DashParticles());
            if (isInvincible == false)
            {
                StartCoroutine(BecomeTemporarilyInvincible(3, 0.12f));
            }
            dashDelay = 1f;
            FindObjectOfType<AudioManager>().Play("DashSound");
        }
    }

    private IEnumerator BecomeTemporarilyInvincible(float invincibilityDurationSeconds, float invincibilityDeltaTime)
    {
        isInvincible = true;

        if (!isDashing)
        {
            for (float i = 0; i < invincibilityDurationSeconds; i += invincibilityDeltaTime)
            {
                if (gameObject.GetComponent<SpriteRenderer>().forceRenderingOff == false)
                {
                    gameObject.GetComponent<SpriteRenderer>().forceRenderingOff = true;
                } else
                {
                    gameObject.GetComponent<SpriteRenderer>().forceRenderingOff = false;
                }

                yield return new WaitForSeconds(invincibilityDeltaTime);
            }

            if (gameObject.GetComponent<SpriteRenderer>().forceRenderingOff)
            {
                gameObject.GetComponent<SpriteRenderer>().forceRenderingOff = false;
            }
        } else
        {
            yield return new WaitForSeconds(invincibilityDeltaTime);
        }

        isInvincible = false;
    }

    private IEnumerator DashParticles()
    {
        isDashing = true;
        dashParticles.Play();

        yield return new WaitForSeconds(dashInvincibilityTime);

        dashParticles.Stop();
        isDashing = false;
    }
}
