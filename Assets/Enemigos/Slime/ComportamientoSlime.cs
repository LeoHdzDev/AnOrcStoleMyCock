using UnityEngine;
using System.Collections;

public class ComportamientoSlime : MonoBehaviour
{
    [Header("Visión")]
    public float rangoDeVision = 6f; 
    public LayerMask capaMuros; // <-- NUEVA: La capa para que el láser choque con la piedra

    [Header("Estadísticas del Slime")]
    public float vidaMaxima = 20f;
    private float vidaActual;
    public float velocidad = 2f;
    public float distanciaAtaque = 1.5f; 
    public float danoAtaque = 10f;
    public float tiempoEntreAtaques = 2f;

    [Header("Tiempos de Animación")]
    public float tiempoParaGolpe = 0.5f; 
    public float tiempoFinAtaque = 0.5f; 

    [Header("Efectos")]
    public GameObject prefabCharco; 

    private Transform jugador;
    private Animator animator;
    private SpriteRenderer spriteSlime; 
    private float temporizadorAtaque;
    private bool estaAtacando = false;
    private bool estaMuerto = false;
    private EnemyAudio enemyAudio;

    private bool jugadorDetectado = false; 

    void Start()
    {
        vidaActual = vidaMaxima;
        enemyAudio = GetComponent<EnemyAudio>();
        animator = GetComponent<Animator>();
        spriteSlime = GetComponent<SpriteRenderer>(); 

        GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
        if (objJugador != null) jugador = objJugador.transform;
    }

    void Update()
    {
        if (jugador == null || estaMuerto) return;
        if (estaAtacando) return; 

        Vector2 diferencia = jugador.position - transform.position;
        float distancia = diferencia.magnitude;

        // --- 1. LÓGICA DE VISIÓN CON LÁSER ---
        if (distancia <= rangoDeVision)
        {
            RaycastHit2D impacto = Physics2D.Linecast(transform.position, jugador.position, capaMuros);

            if (impacto.collider == null)
            {
                // Reproducir el sonido solo la primera vez que te detecta
                if (!jugadorDetectado && enemyAudio != null) enemyAudio.PlayDetectOnce();
                
                jugadorDetectado = true; // No hay muro, te ve
                Debug.DrawLine(transform.position, jugador.position, Color.green);
            }
            else
            {
                jugadorDetectado = false; // Hay pared
                Debug.DrawLine(transform.position, impacto.point, Color.red);
            }
        }
        else
        {
            jugadorDetectado = false; // Estás muy lejos
        }

        // Si no te detecta, se queda quieto
        if (!jugadorDetectado)
        {
            animator.SetBool("Caminando", false);
            return; 
        }

        // --- 2. EFECTO ESPEJO (flipX) ---
        if (diferencia.x > 0.05f) 
        {
            spriteSlime.flipX = false; // Mira a la derecha
        }
        else if (diferencia.x < -0.05f) 
        {
            spriteSlime.flipX = true; // Se voltea a la izquierda
        }

        // --- 3. MOVERSE Y ATACAR ---
        if (distancia > distanciaAtaque)
        {
            transform.position = Vector2.MoveTowards(transform.position, jugador.position, velocidad * Time.deltaTime);
            animator.SetBool("Caminando", true); 
        }
        else
        {
            animator.SetBool("Caminando", false);
            
            if (temporizadorAtaque <= 0)
            {
                StartCoroutine(RutinaAtaque()); 
            }
        }

        if (temporizadorAtaque > 0) temporizadorAtaque -= Time.deltaTime;
    }

    IEnumerator RutinaAtaque()
    {
        estaAtacando = true;
        animator.SetTrigger("Atacar");
        if (enemyAudio != null) enemyAudio.PlayPrepare(); 

        yield return new WaitForSeconds(tiempoParaGolpe);

        if (enemyAudio != null && !estaMuerto) enemyAudio.PlayAttack(); 

        if (jugador != null && !estaMuerto)
        {
            float distancia = Vector2.Distance(transform.position, jugador.position);
            
            if (distancia <= distanciaAtaque + 1.5f) 
            {
                PlayerHealth salud = jugador.GetComponent<PlayerHealth>();
                if (salud != null) salud.RecibirDano(danoAtaque);
            }
        }

        yield return new WaitForSeconds(tiempoFinAtaque);

        estaAtacando = false;
        temporizadorAtaque = tiempoEntreAtaques;
    }

    public void RecibirDano(float cantidad)
    {
        if (estaMuerto) return;

        vidaActual -= cantidad;
        StartCoroutine(EfectoDanar());

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    IEnumerator EfectoDanar()
    {
        if (spriteSlime != null) spriteSlime.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        if (spriteSlime != null) spriteSlime.color = Color.white;
    }

    void Morir()
    {
        estaMuerto = true;
        if (enemyAudio != null) enemyAudio.PlayDeath();
        animator.SetBool("Caminando", false);
        animator.SetTrigger("Morir"); 
        
        if (prefabCharco != null)
        {
            Instantiate(prefabCharco, transform.position, Quaternion.identity);
        }

        GetComponent<Collider2D>().enabled = false;
        StopAllCoroutines(); 
        if (spriteSlime != null) spriteSlime.color = Color.white; 

        Destroy(gameObject, 2f); 
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, rangoDeVision);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
    }
}