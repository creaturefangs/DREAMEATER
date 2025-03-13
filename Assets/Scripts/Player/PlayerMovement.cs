using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of the player

    public Rigidbody2D rb; // Reference to the player's Rigidbody2D component
    public Animator animator; // Reference to the player's Animator component
    public AudioSource audioSource; // Reference to the AudioSource component
    public AudioClip[] footstepSounds; // Array to hold multiple footstep sounds

    private Vector2 movement; // Store the player's movement
    private float footstepTimer = 0f;
    private float footstepInterval = 0.6f; // Adjust this for pacing of footsteps

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.loop = false; // Ensure footsteps are played as separate sounds
    }

    void Update()
    {
        // Input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Update animation parameters
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        // Handle walking sound effect
        HandleWalkingSound();
    }

    void FixedUpdate()
    {
        // Move the player
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void HandleWalkingSound()
    {
        if (movement.sqrMagnitude > 0) // If the player is moving
        {
            footstepTimer += Time.deltaTime;

            if (footstepTimer >= footstepInterval)
            {
                PlayFootstepSound();
                footstepTimer = 0f; // Reset timer
            }
        }
        else // Stop sound when not moving
        {
            footstepTimer = 0f; // Reset timer to avoid instant sound on next move
        }
    }

    void PlayFootstepSound()
    {
        if (footstepSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, footstepSounds.Length); // Pick a random footstep sound
            //audioSource.pitch = Random.Range(1.0f, 1.5f); // Random pitch variation
            audioSource.PlayOneShot(footstepSounds[randomIndex]);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Triggered with: " + other.gameObject.name);
    }
}

