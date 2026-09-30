using UnityEngine;

// Colócalo en cada puerta (Puerta_Laberinto1, 2, 3, 4, 5, 6...) junto al
// script que ya tienes para la funcionalidad de entrar. Solo se encarga
// de mostrar/ocultar el mensaje en pantalla; no interfiere con tu lógica
// de teletransporte existente.
//
// Requiere que la puerta tenga un Collider2D en modo "Is Trigger" (si tu
// puerta ya usa un Collider2D en Trigger para detectar al jugador y entrar,
// puedes reutilizar exactamente el mismo, no hace falta uno nuevo).
public class MensajeInteraccion : MonoBehaviour
{
    [Header("Mismo GameObject de texto en TODAS las puertas")]
    [SerializeField] private GameObject promptUI;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && promptUI != null)
        {
            promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && promptUI != null)
        {
            promptUI.SetActive(false);
        }
    }
}
