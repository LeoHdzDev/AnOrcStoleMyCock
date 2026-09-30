using UnityEngine;
using System.Collections;

public class JefeOrcoController : MonoBehaviour
{
    [Header("Combate")]
    public float velocidad = 3f;
    public float distanciaAtaque = 2.5f;
    public float danoAtaque = 15f;
    public float tiempoEntreAtaques = 3f;

    [Header("Tiempos del Giro")]
    public float tiempoPreparacion = 0.5f; // Segundos antes de que el giro haga daño
    public float duracionGiro = 1.2f;      // Cuánto tiempo se queda dando vueltas

    [Header("Sonido de confundido")]
    [Tooltip("Segundos desde que empieza el ataque hasta que el orco queda mareado y mueve los ojos.")]
    public float tiempoConfundido = 0.7f;

    private Transform objetivo;
    private Animator animator;
    private Rigidbody2D rb;

    private bool jefeActivo = true;
    private EnemyAudio enemyAudio;
    private bool atacando = false;
    private float temporizadorAtaque = 0f;

    void Start()
    {
        enemyAudio = GetComponent<EnemyAudio>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        GameObject jugador = GameObject.Find("Farmer_player");
        if (jugador != null) objetivo = jugador.transform;
    }

    public void DespertarJefe()
    {
        jefeActivo = true; // Esta variable lo mantiene quieto hasta que entres a la sala
    }

    void Update()
    {
        if (!jefeActivo || objetivo == null || atacando) return;

        float distancia = Vector2.Distance(transform.position, objetivo.position);

        if (distancia > distanciaAtaque)
        {
            // Reproduce JefeOrco_Caminando
            transform.position = Vector2.MoveTowards(transform.position, objetivo.position, velocidad * Time.deltaTime);
            animator.SetBool("Caminando", true);

            //if (objetivo.position.x > transform.position.x) animator.SetFloat("Direccion", 1);
            //else if (objetivo.position.x < transform.position.x) animator.SetFloat("Direccion", -1);
        }
        else
        {
            animator.SetBool("Caminando", false); // Reproduce JefeOrco_Idle
            
            if (temporizadorAtaque <= 0)
            {
                StartCoroutine(AtaqueGiratorio());
            }
        }

        if (temporizadorAtaque > 0) temporizadorAtaque -= Time.deltaTime;
    }

    IEnumerator AtaqueGiratorio()
    {
        atacando = true;
        animator.SetTrigger("Atacar"); // Reproduce JefeOrco_ataque
        if (enemyAudio != null) enemyAudio.PlayAttack();
        StartCoroutine(SonidoConfundido());

        // 1. Espera a que el jefe levante el arma
        yield return new WaitForSeconds(tiempoPreparacion);

        // 2. Comienza a girar. El bucle comprueba si el jugador entra al área cada 0.1s
        float tiempoGirando = 0f;
        bool yaHizoDano = false; 

        while (tiempoGirando < duracionGiro)
        {
            if (!yaHizoDano && objetivo != null && Vector2.Distance(transform.position, objetivo.position) <= distanciaAtaque + 0.5f)
            {
                PlayerHealth salud = objetivo.GetComponent<PlayerHealth>();
                if (salud != null) 
                {
                    salud.RecibirDano(danoAtaque);
                    yaHizoDano = true; // Solo le hace daño una vez por giro para no matarlo al instante
                }
            }
            
            tiempoGirando += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        // 3. Termina de dar vueltas
        atacando = false;
        temporizadorAtaque = tiempoEntreAtaques;
    }

    IEnumerator SonidoConfundido()
    {
        // El orco termina de girar y queda mareado moviendo los ojos de lado a lado
        yield return new WaitForSeconds(tiempoConfundido);
        if (enemyAudio != null && enabled) enemyAudio.PlayConfused();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
    }
}