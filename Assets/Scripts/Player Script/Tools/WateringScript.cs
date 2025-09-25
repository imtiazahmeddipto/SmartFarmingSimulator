using UnityEngine;

public class WateringScript : MonoBehaviour
{
    public ParticleSystem waterParticles;
    public float waterHoldTime = 0.5f;

    private float waterTimer = 0f;
    private PlayerMovement movement;
    private Animator animator;

    void Start()
    {
        movement = FindObjectOfType<PlayerMovement>();
        animator = movement.GetComponent<Animator>();
    }

    void Update()
    {
        if (!enabled) return;

        // Stop watering if running
        if (movement.IsRunning)
        {
            ResetWater();
            if (waterParticles != null && waterParticles.isPlaying)
                waterParticles.Stop();
            return;
        }

        bool wateringNow = Input.GetMouseButton(1); // Right click held?

        if (wateringNow)
        {
            animator.SetBool("Watering", true);

            // Optional: rotate player to face mouse
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float direction = mouseWorld.x - transform.position.x;
            movement.FaceDirection(direction);

            waterTimer += Time.deltaTime;
            if (waterTimer >= waterHoldTime)
            {
                Debug.Log("Watering...");
                waterTimer = 0f;
            }
        }

        // --- Particle control ---
        if (waterParticles != null)
        {
            if (wateringNow && !waterParticles.isPlaying) waterParticles.Play();
            if (!wateringNow && waterParticles.isPlaying) waterParticles.Stop();
        }

        if (!wateringNow) ResetWater();
    }

    void ResetWater()
    {
        animator.SetBool("Watering", false);
        waterTimer = 0f;
    }
}
