using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroyScript : MonoBehaviour
{
    private float timeTillDestroy;

    // Start is called before the first frame update
    void Start()
    {
        timeTillDestroy = 15f;
    }

    // Update is called once per frame
    void Update()
    {
        timeTillDestroy -= Time.deltaTime;

        if (timeTillDestroy <= 3f)
        {
            StartCoroutine(TimeOut(3f, 0.15f));
            timeTillDestroy = 15f;
        }
    }

    private IEnumerator TimeOut(float durationSeconds, float deltaTime)
    {
        for (float i = 0; i < durationSeconds; i += deltaTime)
        {
            if (gameObject.GetComponent<SpriteRenderer>().forceRenderingOff == false)
            {
                gameObject.GetComponent<SpriteRenderer>().forceRenderingOff = true;
            } else
            {
                gameObject.GetComponent<SpriteRenderer>().forceRenderingOff = false;
            }

            yield return new WaitForSeconds(deltaTime);
        }

        Destroy(gameObject);
    }
}
