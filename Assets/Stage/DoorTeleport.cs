using UnityEngine;

public class DoorTeleport : MonoBehaviour
{
    [Header("Destino del Teletransporte")]
    [SerializeField] private Transform targetDestination; // Arrastra aquí el punto a donde irás

    [Header("Configuración de Tecla")]
    [SerializeField] private KeyCode interactKey = KeyCode.E; // Tecla para entrar

    private bool isPlayerInside = false;
    private Transform playerTransform;

    private void Update()
    {
        if (isPlayerInside && Input.GetKeyDown(interactKey))
            {
                TeleportPlayer();
            }
    }

    private void TeleportPlayer()
    {
        if (targetDestination == null)
        {
            Debug.LogWarning($"Falta asignar el 'Target Destination' en la puerta {gameObject.name}");
            return;
        }

        if (playerTransform != null)
        {
            // Mueve instantáneamente al jugador al punto de destino
            playerTransform.position = targetDestination.position;

            // Frena cualquier movimiento que traiga el jugador
            Rigidbody2D rb = playerTransform.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            Debug.Log($"Jugador teletransportado a {targetDestination.name}");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInside = true;
            playerTransform = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInside = false;
            playerTransform = null;
        }
    }
}