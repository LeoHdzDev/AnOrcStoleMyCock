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

    private Vector2 movimiento;
    private bool atacando = false;
    private float siguienteAtaque = 0f;
    
    // --- NUEVO: Interruptor de memoria ---
    private bool jugadorDetectado = false; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

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

        // --- NUEVA LÓGICA: CAZADOR IMPLACABLE ---
        // Si no te había visto, pero entraste al círculo morado, te detecta para siempre
        if (!jugadorDetectado && distancia <= rangoDeVision)
        {
            jugadorDetectado = true; 
        }

        // Si aún no te ha detectado, se queda quieto como estatua
        if (!jugadorDetectado)
        {
            movimiento = Vector2.zero;
            animator.SetFloat("Velocidad", 0);
            return; 
        }

        // --- A partir de aquí persigue y ataca, ignorando el rango de visión ---

        if (diferencia.x > 0.05f) animator.SetFloat("Direccion", 1);
        else if (diferencia.x < -0.05f) animator.SetFloat("Direccion", -1);

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
        if (rb == null || atacando) return;
        rb.MovePosition(rb.position + movimiento * velocidad * Time.fixedDeltaTime);
    }

    void IniciarAtaque()
    {
        atacando = true;
        siguienteAtaque = Time.time + tiempoEntreAtaques;
        animator.SetBool("Atacar", true);
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
        // Visión inicial (Morado)
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, rangoDeVision);

        // Rango de ataque (Rojo)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
    }
}