using UnityEngine;

public class ProyectilFuego : MonoBehaviour
{
    [Header("Estadísticas de Fuego")]
    public float danoPorSegundo = 1f; 
    public int duracionQuemadura = 3; 
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
            // --- ESTA ES LA LÍNEA CLAVE ---
            // Buscamos el script de Salud, no el de movimiento
            PlayerHealth saludJugador = otro.GetComponent<PlayerHealth>();
            
            if (saludJugador != null)
            {
                saludJugador.RecibirDano(5f); 
                saludJugador.StartCoroutine(saludJugador.AplicarQuemadura(1f, 3));
            }

            Destroy(gameObject);
        }
    }
}