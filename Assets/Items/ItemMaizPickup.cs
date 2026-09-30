using UnityEngine;

// Igual que ItemPickup.cs en cómo flota hacia el jugador, pero al
// recogerse NO cambia de elemento ni dispara ninguna animación:
// solo restaura vida.
public class ItemMaizPickup : MonoBehaviour
{
    [Header("Curación")]
    [SerializeField] private float vidaQueRestaura = 20f;

    [Header("Atracción")]
    [SerializeField] private float velocidadAtraccion = 5f;
    [SerializeField] private float distanciaMinima = 0.1f;

    private Transform farmer;
    private PlayerHealth farmerSalud;
    private bool atrayendo = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            farmer = other.transform;
            farmerSalud = farmer.GetComponent<PlayerHealth>();
            atrayendo = true;
        }
    }

    private void Update()
    {
        if (!atrayendo || farmer == null)
            return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            farmer.position,
            velocidadAtraccion * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, farmer.position) <= distanciaMinima)
        {
            Recoger();
        }
    }

    private void Recoger()
    {
        atrayendo = false;

        if (farmerSalud != null)
        {
            farmerSalud.Curar(vidaQueRestaura);
        }

                if (farmer != null)
        {
            PlayerAudio audioJugador = farmer.GetComponent<PlayerAudio>();
            if (audioJugador != null) audioJugador.PlayPickup();
        }

        Destroy(gameObject);
    }
}
