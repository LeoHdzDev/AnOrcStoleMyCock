using UnityEngine;

public class Comportamiento_espiritus : MonoBehaviour 
{
    [Header("Visión")]
    public float rangoDeVision = 8f; // <-- NUEVO: Radio de detección (morado)

    [Header("Movimiento")]
    public float velocidad = 2.5f;
    public float distanciaFrenado = 4f;

    [Header("Ataque")]
    public GameObject prefabProyectil; 
    public float tiempoEntreDisparos = 2f;
    private float proximoDisparo = 0f;

    private Transform jugador;
    private Animator animator; 
    
    // --- NUEVO: Interruptor de memoria ---
    private bool jugadorDetectado = false; 

    void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>(); 
    }

    void Update()
    {
        if (jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        // --- NUEVA LÓGICA: CAZADOR IMPLACABLE ---
        // Si no te había visto, pero entraste al círculo morado, se activa para siempre
        if (!jugadorDetectado && distancia <= rangoDeVision)
        {
            jugadorDetectado = true;
        }

        // Si aún no te ha detectado, se queda en paz y detiene la lectura del script
        if (!jugadorDetectado)
        {
            animator.SetBool("Caminando", false);
            return; 
        }
        // ----------------------------------------

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
        Instantiate(prefabProyectil, transform.position, Quaternion.identity); 
    }

    // --- NUEVO: DIBUJADO DE RANGOS VISUALES ---
    private void OnDrawGizmosSelected()
    {
        // Rango de Visión (Morado): A partir de aquí te detecta
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, rangoDeVision);

        // Distancia de Frenado (Rojo): Aquí se detiene para empezar a dispararte
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaFrenado);
    }
}