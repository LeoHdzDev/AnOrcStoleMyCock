using UnityEngine;
using System.Collections;

public class CharcoDano : MonoBehaviour
{
    [Header("Estadísticas del Charco")]
    public float danoPorSegundo = 5f; 
    public float duracionDelCharco = 5f; // Controla cuánto tiempo se queda aquí

    private bool jugadorAdentro = false;
    private PlayerHealth saludJugador;

    void Start()
    {
        // Se destruye automáticamente después de 'duracionDelCharco' segundos
        Destroy(gameObject, duracionDelCharco);
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        // VERIFICA QUE TU GRANJERO TENGA EL TAG "Player" EN UNITY
        if (otro.CompareTag("Player"))
        {
            saludJugador = otro.GetComponent<PlayerHealth>();
            if (saludJugador != null)
            {
                jugadorAdentro = true;
                StartCoroutine(HacerDanoContinuo());
            }
        }
    }

    void OnTriggerExit2D(Collider2D otro)
    {
        if (otro.CompareTag("Player"))
        {
            jugadorAdentro = false;
            saludJugador = null; // El jugador logró salir del charco
        }
    }

    IEnumerator HacerDanoContinuo()
    {
        while (jugadorAdentro && saludJugador != null)
        {
            saludJugador.RecibirDano(danoPorSegundo);
            yield return new WaitForSeconds(1f); // Le resta vida cada 1 segundo
        }
    }
}