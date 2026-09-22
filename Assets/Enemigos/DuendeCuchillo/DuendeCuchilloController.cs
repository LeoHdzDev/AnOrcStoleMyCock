using System.Collections;
using UnityEngine;

public class DuendeCuchilloController : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform objetivo;

    [Header("Movimiento")]
    public float velocidad = 2f;
    public float distanciaAtaque = 0.8f;

    [Header("Ataque")]
    public float tiempoEntreAtaques = 1.2f;
    public float duracionAtaque = 0.5f;
    public float danoAtaque = 5f; // --- NUEVO: Cantidad de daño que hace

    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 movimiento;
    private bool atacando = false;
    private float siguienteAtaque = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (objetivo == null)
        {
            GameObject jugador = GameObject.Find("Farmer_player");
            if (jugador != null)
            {
                objetivo = jugador.transform;
            }
        }
    }

    void Update()
    {
        if (objetivo == null)
        {
            movimiento = Vector2.zero;
            animator.SetFloat("Velocidad", 0);
            return;
        }

        if (atacando)
        {
            movimiento = Vector2.zero;
            animator.SetFloat("Velocidad", 0);
            return;
        }

        Vector2 diferencia = objetivo.position - transform.position;
        float distancia = diferencia.magnitude;

        if (diferencia.x > 0.05f)
        {
            animator.SetFloat("Direccion", 1);
        }
        else if (diferencia.x < -0.05f)
        {
            animator.SetFloat("Direccion", -1);
        }

        if (distancia <= distanciaAtaque)
        {
            movimiento = Vector2.zero;
            animator.SetFloat("Velocidad", 0);

            if (Time.time >= siguienteAtaque)
            {
                IniciarAtaque();
            }
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

    // --- CORREGIDO: Ahora calcula la distancia y hace daño ---
    IEnumerator RutinaAtaque()
    {
        // Esperamos a la mitad del tiempo de la animación (cuando tira la cuchillada)
        yield return new WaitForSeconds(duracionAtaque / 2f);

        // Si el granjero sigue cerca, le restamos vida
        if (objetivo != null && Vector2.Distance(transform.position, objetivo.position) <= distanciaAtaque + 0.5f)
        {
            PlayerHealth salud = objetivo.GetComponent<PlayerHealth>();
            if (salud != null)
            {
                salud.RecibirDano(danoAtaque);
            }
        }

        // Esperamos la otra mitad de la animación
        yield return new WaitForSeconds(duracionAtaque / 2f);

        atacando = false;
        animator.SetBool("Atacar", false);
    }
}