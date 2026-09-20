using UnityEngine;

public class ChorroAgua : MonoBehaviour
{
    public float velocidad = 7f;
    public float dano = 10f;
    public float fuerzaEmpuje = 15f;
    public float tiempoDeVida = 3f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Busca al jugador por su etiqueta para saber hacia dónde volar
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            Vector2 direccion = (jugador.transform.position - transform.position).normalized;
            rb.linearVelocity = direccion * velocidad;
        }

        // Destruye el chorro si no golpea nada después de unos segundos
        Destroy(gameObject, tiempoDeVida);
    }

    void OnTriggerEnter2D(Collider2D colision)
    {
        if (colision.CompareTag("Player"))
        {
            // 1. Quitar vida
            PlayerHealth salud = colision.GetComponent<PlayerHealth>();
            if (salud != null) salud.RecibirDano(dano);

            // 2. Aplicar empuje
            PlayerController movimiento = colision.GetComponent<PlayerController>();
            if (movimiento != null)
            {
                Vector2 direccionEmpuje = (colision.transform.position - transform.position).normalized;
                movimiento.StartCoroutine(movimiento.RecibirEmpuje(direccionEmpuje, fuerzaEmpuje, 0.2f));
            }

            // 3. Destruir el chorro de agua
            Destroy(gameObject);
        }
    }
}