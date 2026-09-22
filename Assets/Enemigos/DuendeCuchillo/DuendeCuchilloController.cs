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

    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 movimiento;
    private bool atacando = false;
    private float siguienteAtaque = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Si no asignamos manualmente al Farmer,
        // intentamos encontrarlo por su nombre.
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
        // Si no encontramos al jugador, no hacemos nada.
        if (objetivo == null)
        {
            movimiento = Vector2.zero;
            animator.SetFloat("Velocidad", 0);
            return;
        }

        // Si estamos atacando, permanecemos quietos.
        if (atacando)
        {
            movimiento = Vector2.zero;
            animator.SetFloat("Velocidad", 0);
            return;
        }

        // Calculamos la distancia entre el duende y el Farmer.
        Vector2 diferencia = objetivo.position - transform.position;
        float distancia = diferencia.magnitude;

        // Determinamos hacia dónde está el Farmer.
        if (diferencia.x > 0.05f)
        {
            animator.SetFloat("Direccion", 1);
        }
        else if (diferencia.x < -0.05f)
        {
            animator.SetFloat("Direccion", -1);
        }

        // Si está suficientemente cerca, intenta atacar.
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

        // Si está lejos, perseguimos al Farmer.
        movimiento = diferencia.normalized;

        animator.SetFloat("Velocidad", movimiento.magnitude);
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        if (atacando)
            return;

        rb.MovePosition(rb.position + movimiento * velocidad * Time.fixedDeltaTime);
    }

    void IniciarAtaque()
    {
        atacando = true;

        // Guardamos el momento del siguiente ataque.
        siguienteAtaque = Time.time + tiempoEntreAtaques;

        // Avisamos al Animator.
        animator.SetBool("Atacar", true);

        StartCoroutine(TerminarAtaque());
    }

    IEnumerator TerminarAtaque()
    {
        yield return new WaitForSeconds(duracionAtaque);

        atacando = false;

        // Avisamos al Animator que terminó el ataque.
        animator.SetBool("Atacar", false);
    }
}