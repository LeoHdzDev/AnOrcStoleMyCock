using System.Collections;
using UnityEngine;

public class BombaExplosiva : MonoBehaviour
{
    [Header("Configuración de Vuelo")]
    public float tiempoDeVuelo = 1f;
    public float alturaParabola = 2f;

    [Header("Explosión")]
    public float tiempoParaExplotar = 5f; // Los 5 segundos de espera
    public float radioExplosion = 2.5f;
    public float danoExplosion = 20f;
    public GameObject prefabEfectoExplosion; // Aquí pondrás tu sprite de explosión luego

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Esta función la llamará el duende al lanzarla
    public void InicializarLanzamiento(Vector2 origen, Vector2 destino)
    {
        StartCoroutine(RutinaVuelo(origen, destino));
    }

    IEnumerator RutinaVuelo(Vector2 origen, Vector2 destino)
    {
        float tiempo = 0f;

        while (tiempo < tiempoDeVuelo)
        {
            tiempo += Time.deltaTime;
            float porcentajeVuelo = tiempo / tiempoDeVuelo;

            // Movimiento lineal hacia el objetivo
            Vector2 posicionLineal = Vector2.Lerp(origen, destino, porcentajeVuelo);
            
            // Calculamos la altura de la parábola usando una curva matemática (Seno)
            float alturaExtra = Mathf.Sin(porcentajeVuelo * Mathf.PI) * alturaParabola;

            transform.position = new Vector3(posicionLineal.x, posicionLineal.y + alturaExtra, 0f);

            yield return null;
        }

        // Al tocar el piso, inicia la cuenta regresiva
        StartCoroutine(CuentaRegresiva());
    }

    IEnumerator CuentaRegresiva()
    {
        float tiempoRestante = tiempoParaExplotar;
        float velocidadParpadeo = 0.5f;

        while (tiempoRestante > 0)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(velocidadParpadeo);
            
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(velocidadParpadeo);

            tiempoRestante -= (velocidadParpadeo * 2);

            // Hace que parpadee más rápido conforme se acerca la explosión
            velocidadParpadeo = Mathf.Max(0.05f, velocidadParpadeo - 0.05f); 
        }

        Explotar();
    }

    void Explotar()
    {
        // 1. Instanciar el efecto visual de explosión (si lo tienes)
        if (prefabEfectoExplosion != null)
        {
            Instantiate(prefabEfectoExplosion, transform.position, Quaternion.identity);
        }

        // 2. Detectar a quién le hizo daño en área (AoE)
        Collider2D[] afectados = Physics2D.OverlapCircleAll(transform.position, radioExplosion);
        foreach (Collider2D afectado in afectados)
        {
            if (afectado.CompareTag("Player"))
            {
                PlayerHealth salud = afectado.GetComponent<PlayerHealth>();
                if (salud != null) salud.RecibirDano(danoExplosion);
            }
        }

        // 3. Destruir la bomba
        Destroy(gameObject);
    }

    // Dibuja el radio de la explosión en rojo en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioExplosion);
    }
}