using UnityEngine;

public class ProyectilHielo : MonoBehaviour
{
    [Header("Estadísticas de Hielo")]
    public float danoImpacto = 3f; // Daño que hace al chocar
    public float factorRalentizacion = 0.5f; // 0.5f significa que le reduce la velocidad a la mitad
    public int duracionRalentizacion = 3; 
    public float velocidadProyectil = 5f;

    private Vector2 direccion;

    void Start()
    {
        Transform jugador = GameObject.FindGameObjectWithTag("Player").transform;
        if (jugador != null)
        {
            direccion = (jugador.position - transform.position).normalized;
        }
        else
        {
            direccion = Vector2.right; 
        }
    }

    void Update()
    {
        transform.Translate(direccion * velocidadProyectil * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("Player"))
        {
            // 1. Aplicar Daño
            PlayerHealth saludJugador = otro.GetComponent<PlayerHealth>();
            if (saludJugador != null)
            {
                saludJugador.RecibirDano(danoImpacto); 
            }

            // 2. Aplicar Ralentización
            PlayerController controlJugador = otro.GetComponent<PlayerController>();
            if (controlJugador != null)
            {
                controlJugador.StartCoroutine(controlJugador.AplicarRalentizacion(factorRalentizacion, duracionRalentizacion));
            }

            // 3. Destruir la bola de nieve
            Destroy(gameObject);
        }
    }
}