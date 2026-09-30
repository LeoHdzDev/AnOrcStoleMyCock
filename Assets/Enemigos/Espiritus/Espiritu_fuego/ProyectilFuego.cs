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
        // 1. Ignorar al espíritu para que la bola no se autodestruya al nacer
        // OJO: Asegúrate de usar la misma etiqueta exacta que te funcionó en el de agua
        if (otro.CompareTag("Enemigos")) 
        {
            return;
        }

        // 2. Si choca con el jugador, quema y hace daño
        if (otro.CompareTag("Player"))
        {
            PlayerHealth saludJugador = otro.GetComponent<PlayerHealth>();
            
            if (saludJugador != null)
            {
                saludJugador.RecibirDano(5f); 
                saludJugador.StartCoroutine(saludJugador.AplicarQuemadura(1f, 3));
            }

            Destroy(gameObject);
            return; 
        }

        // 3. Si no es el espíritu ni es el jugador, significa que chocó contra un muro
        Destroy(gameObject);
    }
}