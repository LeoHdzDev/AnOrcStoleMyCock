using UnityEngine;
using System.Collections;

public class ComportamientoSlime : MonoBehaviour
{
    [Header("Estadísticas del Slime")]
    public float vidaMaxima = 20f;
    private float vidaActual;
    public float velocidad = 2f;
    public float distanciaAtaque = 1.5f; 
    public float danoAtaque = 10f;
    public float tiempoEntreAtaques = 2f;

    [Header("Tiempos de Animación (NUEVO)")]
    // Ajusta estos segundos en el Inspector para que cuadren con tu dibujo
    public float tiempoParaGolpe = 0.5f; // Tiempo que tarda en levantar las manos y tocar el piso
    public float tiempoFinAtaque = 0.5f; // Tiempo que tarda en recuperarse después del golpe

    [Header("Efectos")]
    public GameObject prefabCharco; 

    private Transform jugador;
    private Animator animator;
    private SpriteRenderer spriteSlime; 
    private float temporizadorAtaque;
    private bool estaAtacando = false;
    private bool estaMuerto = false;

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

        // Si está en medio de un ataque, ignoramos todo lo demás
        if (estaAtacando) return; 

        float distancia = Vector2.Distance(transform.position, jugador.position);

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
                StartCoroutine(RutinaAtaque()); // Llamamos a la nueva corrutina segura
            }
        }

        if (temporizadorAtaque > 0) temporizadorAtaque -= Time.deltaTime;
    }

    // --- NUEVO SISTEMA DE ATAQUE POR TIEMPO ---
    IEnumerator RutinaAtaque()
    {
        estaAtacando = true;
        animator.SetTrigger("Atacar"); 

        // 1. Esperamos exactamente el tiempo que le toma llegar al frame 12
        yield return new WaitForSeconds(tiempoParaGolpe);

        // 2. En este momento exacto, aplicamos el daño
        if (jugador != null && !estaMuerto)
        {
            float distancia = Vector2.Distance(transform.position, jugador.position);
            
            // Le damos un margen extra (+1.5f) para que el daño no falle si te moviste un pixel
            if (distancia <= distanciaAtaque + 1.5f) 
            {
                PlayerHealth salud = jugador.GetComponent<PlayerHealth>();
                if (salud != null) salud.RecibirDano(danoAtaque);
            }
        }

        // 3. Esperamos a que la animación termine de bajar los brazos
        yield return new WaitForSeconds(tiempoFinAtaque);

        // 4. Liberamos al Slime para que vuelva a caminar
        estaAtacando = false;
        temporizadorAtaque = tiempoEntreAtaques;
    }

    // --- SISTEMA DE DAÑO, DESTELLO ROJO Y MUERTE ---
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
        StopAllCoroutines(); // Evitamos que siga atacando o parpadeando si ya murió
        if (spriteSlime != null) spriteSlime.color = Color.white; 

        Destroy(gameObject, 2f); 
    }
}