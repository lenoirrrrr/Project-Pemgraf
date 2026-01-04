using UnityEngine;

// Memastikan script ini hanya bisa dipasang jika ada Rigidbody
[RequireComponent(typeof(Rigidbody))]
public class NPCWanderAI : MonoBehaviour
{
    [Header("AI Settings")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f;
    public float wanderRadius = 5f; // Jarak maksimal jalan dari titik awal

    [Header("Timing")]
    public float minWaitTime = 2f; // Waktu diam minimal
    public float maxWaitTime = 5f; // Waktu diam maksimal
    public float moveTime = 3f;    // Berapa lama dia berjalan

    private Rigidbody rb;
    private Vector3 startPosition;
    private Vector3 targetDirection;
    private float timer;

    // State Machine Sederhana
    private enum AIState { Idle, Wandering }
    private AIState currentState;

    // Referensi ke script interaksi agar berhenti saat ngobrol
    private NPCInteraction interactionScript;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        interactionScript = GetComponent<NPCInteraction>();
        startPosition = transform.position; // Ingat posisi awal
        currentState = AIState.Idle;
        timer = Random.Range(minWaitTime, maxWaitTime);
    }

    void FixedUpdate()
    {
        // --- CEK INTERAKSI ---
        // Jika sedang ngobrol dengan player, jangan lakukan apa-apa (diam)
        if (interactionScript != null && interactionScript.IsTalking())
        {
            rb.velocity = Vector3.zero; // Paksa berhenti
            return; 
        }

        timer -= Time.fixedDeltaTime;

        switch (currentState)
        {
            case AIState.Idle:
                rb.velocity = Vector3.zero; // Pastikan diam
                if (timer <= 0)
                {
                    PickNewDirection();
                    currentState = AIState.Wandering;
                    timer = moveTime;
                }
                break;

            case AIState.Wandering:
                MoveNPC();
                if (timer <= 0)
                {
                    currentState = AIState.Idle;
                    timer = Random.Range(minWaitTime, maxWaitTime);
                }
                break;
        }
    }

    void PickNewDirection()
    {
        // Pilih arah random di sumbu X dan Z
        Vector3 randomPoint = startPosition + Random.insideUnitSphere * wanderRadius;
        randomPoint.y = transform.position.y; // Jaga agar tetap di tanah
        
        targetDirection = (randomPoint - transform.position).normalized;
    }

    void MoveNPC()
    {
        // Gerakkan Rigidbody
        Vector3 velocity = targetDirection * moveSpeed;
        // Pertahankan gravitasi (velocity.y) yang ada sekarang
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);

        // Putar badan menghadap arah jalan
        if (targetDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    // Untuk debugging visual di Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (Application.isPlaying)
            Gizmos.DrawWireSphere(startPosition, wanderRadius);
        else
            Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }
}