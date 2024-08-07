using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    private bool ballIsActive;
    private Vector3 ballPosition;
    private Vector2 ballInitialForce;

    // GameObject
    public GameObject playerObject;

    public AudioClip hitSound;
    private AudioSource audioSource;
    private Rigidbody2D rb; // Добавлено поле для Rigidbody2D

    // Start is called before the first frame update
    void Start()
    {
        // create the force
        ballInitialForce = new Vector2(200.0f, 400.0f); // Увеличена сила

        // set to inactive
        ballIsActive = false;

        // ball position
        ballPosition = transform.position;

        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing from the ball object.");
        }

        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component is missing from the ball object.");
        }
        else
        {
            // Убедитесь, что Rigidbody2D настроен корректно
            rb.isKinematic = true;
            rb.gravityScale = 1;
            rb.velocity = Vector2.zero;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (ballIsActive && audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // check for user input
        if (Input.GetButtonDown("Jump"))
        {
            // check if is the first play
            if (!ballIsActive)
            {
                // reset the force
                rb.isKinematic = false;

                // add a force with a random initial impulse
                rb.AddForce(ballInitialForce + new Vector2(Random.Range(-50.0f, 50.0f), 0));

                // set ball active
                ballIsActive = true;
            }
        }

        if (!ballIsActive && playerObject != null)
        {
            // get and use the player position
            ballPosition.x = playerObject.transform.position.x;

            // apply player X position to the ball
            transform.position = ballPosition;
        }

        // Check if ball falls
        if (ballIsActive && transform.position.y < -6)
        {
            ballIsActive = false;
            ballPosition.x = playerObject.transform.position.x;
            ballPosition.y = -4f;
            transform.position = ballPosition;

            rb.isKinematic = true;

            // Send Message
            playerObject.SendMessage("TakeLife");
        }
    }
}
