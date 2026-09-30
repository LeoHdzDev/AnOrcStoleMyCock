using System.Collections;
using UnityEngine;

public class DuendeCuchilloController : MonoBehaviour
{
    [Header("Objetivo y Visión")]
    public Transform objetivo;
    public float rangoDeVision = 6f; // <-- Radio morado de detección inicial

    [Header("Movimiento")]
    public float velocidad = 2f;
    public float distanciaAtaque = 0.8f;

    [Header("Ataque")]
    public float tiempoEntreAtaques = 1.2f;
    public float duracionAtaque = 0.5f;
    public float danoAtaque = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer; // <-- Añadido para voltear el dibujo

    private Vector2 movimiento;
    private bool atacando = false;
    private float siguienteAtaque = 0f;
    
    // --- NUEVO: Interruptor de memoria ---
    private bool jugadorDetectado = false; 
    private EnemyAudio enemyAudio;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // <-- Conectamos el SpriteRenderer
        enemyAudio = GetComponent<EnemyAudio>();

        if (objetivo == null)
        {
            GameObject jugador = GameObject.Find("Farmer_player");
            if (jugador != null) objetivo = jugador.transform;
        }
    }

    void Update()
    {
        if (objetivo == null || atacando)
        {
            movimiento = Vector2.zero;
            animator.SetFloat("Velocidad", 0);
            return;
        }

        Vector2 diferencia = objetivo.position - transform.position;
        float distancia = diferencia.magnitude;

        // --- LÓGICA: CAZADOR IMPLACABLE ---
        if (!jugadorDetectado && distancia <= rangoDeVision)
        {
            jugadorDetectado = true; 
        }

        if (!jugadorDetectado)
        {
            movimiento = Vector2.zero;
            animator.SetFloat("Velocidad", 0);
            return; 
        }

        // --- SOLUCIÓN ANIMACIÓN: Usar flipX en lugar del parámetro "Direccion" ---
        if (diferencia.x > 0.05f) spriteRenderer.flipX = false;
        else if (diferencia.x < -0.05f) spriteRenderer.flipX = true;

        if (distancia <= distanciaAtaque)
        {
            movimiento = Vector2.zero;
            animator.SetFloat("Velocidad", 0);

            if (Time.time >= siguienteAtaque) IniciarAtaque();
            return;
        }

        movimiento = diferencia.normalized;
        animator.SetFloat("Velocidad", movimiento.magnitude);
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // --- SOLUCIÓN FÍSICA: Matamos cualquier inercia externa ---
        // Esto evita que salga volando hacia atrás si el granjero lo golpea o empuja.
        rb.linearVelocity = Vector2.zero; 

        if (!atacando)
        {
            rb.MovePosition(rb.position + movimiento * velocidad * Time.fixedDeltaTime);
        }
    }

    void IniciarAtaque()
    {
        atacando = true;
        siguienteAtaque = Time.time + tiempoEntreAtaques;
        animator.SetBool("Atacar", true);
        if (enemyAudio != null) enemyAudio.PlayAttack();
        StartCoroutine(RutinaAtaque());
    }

    IEnumerator RutinaAtaque()
    {
        yield return new WaitForSeconds(duracionAtaque / 2f);

        if (objetivo != null && Vector2.Distance(transform.position, objetivo.position) <= distanciaAtaque + 0.5f)
        {
            PlayerHealth salud = objetivo.GetComponent<PlayerHealth>();
            if (salud != null) salud.RecibirDano(danoAtaque);
        }

        yield return new WaitForSeconds(duracionAtaque / 2f);
        atacando = false;
        animator.SetBool("Atacar", false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, rangoDeVision);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
    }
}