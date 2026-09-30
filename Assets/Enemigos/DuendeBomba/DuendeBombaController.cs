using System.Collections;
using UnityEngine;

public class DuendeBombaController : MonoBehaviour
{
    [Header("Visión")]
    public float rangoDeVision = 8f; // Aro morado

    [Header("Movimiento")]
    public float velocidad = 2f;
    public float distanciaParaLanzar = 5f; // Aro rojo (se detiene para disparar)

    [Header("Combate y Tiempos")]
    public GameObject prefabBomba;
    public float tiempoAnimacionLanzamiento = 0.4f; // Tiempo que tarda la animación de atacar antes de soltar la bomba
    
    // Tiempos configurables que pediste:
    public float tiempoSinBomba = 3f;     // Cuánto tiempo se queda con el sprite de 'Sin Bomba'
    public float cooldownLanzamiento = 2f; // Cuánto tarda en tirar otra bomba después de haber recargado

    private Transform jugador;
    private Animator animator;
    private bool jugadorDetectado = false;
    
    private bool tieneBomba = true;
    private bool estaAtacando = false;
    private float temporizadorCooldown = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        GameObject objJugador = GameObject.Find("Farmer_player");
        if (objJugador != null) jugador = objJugador.transform;
    }

    void Update()
    {
        if (jugador == null || estaAtacando) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        if (!jugadorDetectado && distancia <= rangoDeVision)
        {
            jugadorDetectado = true;
        }

        if (!jugadorDetectado)
        {
            animator.SetBool("Caminando", false);
            return; 
        }

        // Girar hacia el jugador
        if (jugador.position.x > transform.position.x + 0.1f) animator.SetFloat("Direccion", 1);
        else if (jugador.position.x < transform.position.x - 0.1f) animator.SetFloat("Direccion", -1);

        // Lógica de Movimiento o Ataque
        if (distancia > distanciaParaLanzar)
        {
            // Solo avanza si TIENE una bomba. Si no tiene, se queda quieto recargando
            if (tieneBomba)
            {
                transform.position = Vector2.MoveTowards(transform.position, jugador.position, velocidad * Time.deltaTime);
                animator.SetBool("Caminando", true);
            }
            else
            {
                animator.SetBool("Caminando", false);
            }
        }
        else
        {
            animator.SetBool("Caminando", false);

            if (tieneBomba && temporizadorCooldown <= 0)
            {
                StartCoroutine(RutinaLanzarBomba());
            }
        }

        if (temporizadorCooldown > 0) temporizadorCooldown -= Time.deltaTime;
    }

    IEnumerator RutinaLanzarBomba()
    {
        estaAtacando = true;
        animator.SetTrigger("Atacar");

        // 1. Espera a que el duende estire el brazo
        yield return new WaitForSeconds(tiempoAnimacionLanzamiento);

        // 2. Instancia la bomba y le pasa las coordenadas para la parábola
        if (jugador != null)
        {
            GameObject bombaObj = Instantiate(prefabBomba, transform.position, Quaternion.identity);
            BombaExplosiva scriptBomba = bombaObj.GetComponent<BombaExplosiva>();
            if (scriptBomba != null)
            {
                scriptBomba.InicializarLanzamiento(transform.position, jugador.position);
            }
        }

        // 3. Pierde la bomba (Cambia al sprite Sin Bomba)
        tieneBomba = false;
        animator.SetBool("TieneBomba", false); // <-- Parámetro clave en tu Animator
        estaAtacando = false;

        // 4. Inicia su proceso de recarga
        StartCoroutine(RutinaRecargar());
    }

    IEnumerator RutinaRecargar()
    {
        // Se queda quieto y sin bomba durante este tiempo
        yield return new WaitForSeconds(tiempoSinBomba);

        // Recupera la bomba (Vuelve al sprite Idle normal)
        tieneBomba = true;
        animator.SetBool("TieneBomba", true);
        
        // Comienza a contar el cooldown extra antes del próximo tiro
        temporizadorCooldown = cooldownLanzamiento;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, rangoDeVision);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaParaLanzar);
    }
}