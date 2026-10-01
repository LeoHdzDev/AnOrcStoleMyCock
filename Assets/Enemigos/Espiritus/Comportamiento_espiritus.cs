using UnityEngine;

public class Comportamiento_espiritus : MonoBehaviour 
{
    [Header("Visión")]
    public float rangoDeVision = 8f; 
    public LayerMask capaMuros; // <-- NUEVA: La capa para que el láser choque con la piedra

    [Header("Movimiento")]
    public float velocidad = 2.5f;
    public float distanciaFrenado = 4f;

    [Header("Ataque")]
    public GameObject prefabProyectil; 
    public float tiempoEntreDisparos = 2f;
    private float proximoDisparo = 0f;

    private Transform jugador;
    private EnemyAudio enemyAudio;
    private Animator animator; 
    private SpriteRenderer spriteRenderer; // <-- NUEVO: Para voltear el dibujo como espejo
    
    private bool jugadorDetectado = false; 

    void Start()
    {
        enemyAudio = GetComponent<EnemyAudio>();
        animator = GetComponent<Animator>(); 
        spriteRenderer = GetComponent<SpriteRenderer>(); // Conectamos el componente

        GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
        if (objJugador != null) jugador = objJugador.transform;
    }

    void Update()
    {
        if (jugador == null) return;

        Vector2 diferencia = jugador.position - transform.position;
        float distancia = diferencia.magnitude;

        // --- 1. LÓGICA DE VISIÓN CON LÁSER ---
        if (distancia <= rangoDeVision)
        {
            RaycastHit2D impacto = Physics2D.Linecast(transform.position, jugador.position, capaMuros);

            if (impacto.collider == null)
            {
                jugadorDetectado = true; // No hay muro, te está viendo
                Debug.DrawLine(transform.position, jugador.position, Color.green);
            }
            else
            {
                jugadorDetectado = false; // Hay una pared tapándole la vista
                Debug.DrawLine(transform.position, impacto.point, Color.red);
            }
        }
        else
        {
            jugadorDetectado = false; // Estás muy lejos
        }

        // Si no te ha detectado (estás escondido o lejos), se queda quieto
        if (!jugadorDetectado)
        {
            animator.SetBool("Caminando", false);
            return; 
        }

        // --- 2. EFECTO ESPEJO (Para que voltee a verte) ---
        if (diferencia.x > 0.05f) 
        {
            spriteRenderer.flipX = false; // Mira a la derecha
        }
        else if (diferencia.x < -0.05f) 
        {
            spriteRenderer.flipX = true; // Se voltea a la izquierda
        }

        // --- 3. MOVERSE Y DISPARAR ---
        // Moverse hacia el jugador si está lejos
        if (distancia > distanciaFrenado)
        {
            transform.position = Vector2.MoveTowards(transform.position, jugador.position, velocidad * Time.deltaTime);
            animator.SetBool("Caminando", true); 
        }
        else
        {
            animator.SetBool("Caminando", false); 
        }

        // Disparar
        if (Time.time >= proximoDisparo)
        {
            Atacar();
            proximoDisparo = Time.time + tiempoEntreDisparos;
        }
    }

    void Atacar()
    {
        animator.SetTrigger("Atacar");
        if (enemyAudio != null) enemyAudio.PlayAttack(); 
        Instantiate(prefabProyectil, transform.position, Quaternion.identity); 
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, rangoDeVision);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaFrenado);
    }
}