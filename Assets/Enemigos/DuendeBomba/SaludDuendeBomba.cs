using System.Collections;
using UnityEngine;

public class SaludDuendeBomba : MonoBehaviour
{
    [Header("Estadísticas")]
    public float vidaMaxima = 15f;
    private float vidaActual;

    [Header("Explosión al Morir")]
    public float tiempoAntesDeExplotar = 2f; // Segundos que le das al granjero para huir
    public float radioExplosion = 2.5f;
    public float danoExplosion = 30f;
    public GameObject prefabEfectoExplosion; // Tu sprite/animación de explosión (cuando lo tengas)

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool muerto = false;
    private EnemyAudio enemyAudio;

    void Start()
    {
        enemyAudio = GetComponent<EnemyAudio>();
        vidaActual = vidaMaxima;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void RecibirDano(float cantidad)
    {
        if (muerto) return; // Si ya está haciendo su cuenta regresiva de muerte, ignoramos más golpes

        vidaActual -= cantidad;

        if (spriteRenderer != null)
        {
            StartCoroutine(ParpadeoRojo());
        }

        if (vidaActual <= 0)
        {
            IniciarMuerteExplosiva();
        }
        IEnumerator ParpadeoRojo()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        
        // Solo lo regresa a blanco si el duende sigue vivo
        if (!muerto && spriteRenderer != null) spriteRenderer.color = Color.white;
    }
    }

    IEnumerator ParpadeoRojo()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        if (!muerto && spriteRenderer != null) spriteRenderer.color = Color.white;
    }

    void IniciarMuerteExplosiva()
    {
        muerto = true;

        // 1. Apagamos su inteligencia artificial para que deje de caminar y lanzar bombas
        DuendeBombaController controlador = GetComponent<DuendeBombaController>();
        if (controlador != null) controlador.enabled = false;

        // 2. Apagamos su caja de colisión para que el granjero pueda atravesarlo al huir
        GetComponent<Collider2D>().enabled = false;
        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero; // Lo frenamos en seco

        // Sonido de muerte
        if (enemyAudio != null) enemyAudio.PlayDeath();

        // 3. Activamos el Trigger "Die" en el Animator para que reproduzca su sprite de muerte
        if (animator != null) animator.SetTrigger("Die");

        // 4. Iniciamos la cuenta regresiva antes de explotar
        StartCoroutine(CuentaRegresivaExplosion());
    }

    IEnumerator CuentaRegresivaExplosion()
    {
        float tiempoRestante = tiempoAntesDeExplotar;
        float velocidadParpadeo = 0.3f;

        // Bucle que hace parpadear al duende cada vez más rápido
        while (tiempoRestante > 0)
        {
            if (spriteRenderer != null) spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(velocidadParpadeo);
            
            if (spriteRenderer != null) spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(velocidadParpadeo);

            tiempoRestante -= (velocidadParpadeo * 2);
            velocidadParpadeo = Mathf.Max(0.05f, velocidadParpadeo - 0.05f); // Acelera el parpadeo
        }

        Explotar();
    }

    void Explotar()
    {
        // 1. Instanciar el efecto visual
        if (prefabEfectoExplosion != null)
        {
            Instantiate(prefabEfectoExplosion, transform.position, Quaternion.identity);
        }

        // 2. Detectar si el granjero sigue dentro del área y hacerle daño
        Collider2D[] afectados = Physics2D.OverlapCircleAll(transform.position, radioExplosion);
        foreach (Collider2D afectado in afectados)
        {
            if (afectado.CompareTag("Player"))
            {
                PlayerHealth salud = afectado.GetComponent<PlayerHealth>();
                if (salud != null) salud.RecibirDano(danoExplosion);
            }
        }

        // Sonido de la explosión del duende
        if (enemyAudio != null) enemyAudio.PlayExplosion();

        // 3. Finalmente, borrar el cadáver del duende
        Destroy(gameObject);
    }

    // Para ver el área de la explosión final en rojo desde el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioExplosion);
    }
}