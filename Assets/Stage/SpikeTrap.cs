using System.Collections;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [Header("Tiempos del Ciclo (Segundos)")]
    [SerializeField] private float retractedDuration = 2f; // Tiempo oculto
    [SerializeField] private float warningDuration = 0.8f; // Tiempo asomado un poco
    [SerializeField] private float activeDuration = 1.5f;  // Tiempo fuera haciendo daño

    [Header("Daño")]
    [SerializeField] private float damageAmount = 10f;     // Cantidad de daño que resta
    [SerializeField] private float damageCooldown = 0.5f;  // Intervalo de daño si se queda encima

    [Header("Sonidos (opcionales)")]
    [SerializeField] private AudioClip[] clipsAdvertencia;   // Cuando asoman un poco
    [SerializeField] private AudioClip[] clipsActivacion;    // Cuando salen por completo
    [SerializeField, Range(0f, 1f)] private float volumen = 1f;
    [Tooltip("Distancia al jugador a partir de la cual ya no se oye. Evita que suenen todas las trampas del nivel a la vez.")]
    [SerializeField] private float distanciaMaximaSonido = 10f;

    private AudioSource audioSource;
    private Transform jugador;
    private Animator animator;
    private Collider2D trapCollider;
    private bool isActive = false;
    private float lastDamageTime;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        trapCollider = GetComponent<Collider2D>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    private void Start()
    {
        GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
        if (objJugador != null) jugador = objJugador.transform;

        StartCoroutine(TrapRoutine());
    }

    private IEnumerator TrapRoutine()
    {
        while (true)
        {
            // 1. Estado Oculto / Guardado
            isActive = false;
            if (trapCollider != null) trapCollider.enabled = false;
            if (animator != null) animator.Play("Retracted");
            yield return new WaitForSeconds(retractedDuration);

            // 2. Estado Advertencia (salen un poco)
            if (animator != null) animator.Play("Warning");
            ReproducirSonido(clipsAdvertencia);
            yield return new WaitForSeconds(warningDuration);

            // 3. Estado Activo (salen por completo y hacen daño)
            if (animator != null) animator.Play("Active");
            ReproducirSonido(clipsActivacion);
            isActive = true;
            if (trapCollider != null) trapCollider.enabled = true;
            yield return new WaitForSeconds(activeDuration);
        }
    }

    // Elige un clip al azar y baja el volumen según la distancia al jugador
    private void ReproducirSonido(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;

        float factor = 1f;
        if (jugador != null)
        {
            float distancia = Vector2.Distance(transform.position, jugador.position);
            factor = 1f - Mathf.Clamp01(distancia / distanciaMaximaSonido);
        }
        if (factor <= 0f) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];
        audioSource.PlayOneShot(clip, volumen * factor);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isActive && collision.CompareTag("Player"))
        {
            // Aplica daño en intervalos para evitar restarlo cada frame
            if (Time.time >= lastDamageTime + damageCooldown)
            {
                PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.RecibirDano(damageAmount);
                    lastDamageTime = Time.time;
                }
            }
        }
    }
}