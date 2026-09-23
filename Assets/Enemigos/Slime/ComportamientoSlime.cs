using UnityEngine;
using System.Collections;

public class ComportamientoSlime : MonoBehaviour
{
    [Header("Visión")]
    public float rangoDeVision = 6f; // <-- NUEVO: Radio morado de detección

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

    // --- NUEVO: Interruptor de memoria ---
    private bool jugadorDetectado = false; 

    void Start()
    {
        vidaActual = vidaMaxima;
        animator = GetComponent<Animator>();
        spriteSlime = GetComponent<SpriteRenderer>(); 

        GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
        if (objJugador != null) jugador = objJugador.transform;
    }

    void Update()
    {
        if (jugador == null || estaMuerto) return;
        if (estaAtacando) return; 

        float distancia = Vector2.Distance(transform.position, jugador.position);

        // --- NUEVA LÓGICA: CAZADOR IMPLACABLE ---
        // Si no te había visto, pero entraste al círculo morado, te detecta para siempre
        if (!jugadorDetectado && distancia <= rangoDeVision)
        {
            jugadorDetectado = true;
        }

        // Si aún no te ha detectado, se queda dormido
        if (!jugadorDetectado)
        {
            animator.SetBool("Caminando", false);
            return; 
        }
        // ----------------------------------------

        if (distancia > distanciaAtaque)
        {
            // --- CAMINAR ---
            transform.position = Vector2.MoveTowards(transform.position, jugador.position, velocidad * Time.deltaTime);
            animator.SetBool("Caminando", true); 
        }
        else
        {
            // --- DETENERSE PARA ATACAR ---
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

        yield return new WaitForSeconds(tiempoParaGolpe);

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

    // --- NUEVO: DIBUJADO DE RANGOS VISUALES ---
    private void OnDrawGizmosSelected()
    {
        // Rango de Visión (Morado)
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, rangoDeVision);

        // Distancia de Ataque (Rojo)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
    }
}