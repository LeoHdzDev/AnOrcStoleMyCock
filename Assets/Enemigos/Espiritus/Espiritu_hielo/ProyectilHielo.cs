using UnityEngine;

public class ProyectilHielo : MonoBehaviour
{
    [Header("Estadísticas de Hielo")]
    public float danoImpacto = 3f; 
    public float factorRalentizacion = 0.5f; 
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
        // 1. Ignorar al espíritu para que la bola de nieve no se autodestruya al nacer
        if (otro.CompareTag("Enemigos"))
        {
            return;
        }

        // 2. Si choca con el jugador, hace daño y lo ralentiza
        if (otro.CompareTag("Player"))
        {
            PlayerHealth saludJugador = otro.GetComponent<PlayerHealth>();
            if (saludJugador != null)
            {
                saludJugador.RecibirDano(danoImpacto); 
            }

            PlayerController controlJugador = otro.GetComponent<PlayerController>();
            if (controlJugador != null)
            {
                controlJugador.StartCoroutine(controlJugador.AplicarRalentizacion(factorRalentizacion, duracionRalentizacion));
            }

            Destroy(gameObject);
            return;
        }

        // 3. Si no es el espíritu ni el granjero, significa que chocó contra un muro
        Destroy(gameObject);
    }
}