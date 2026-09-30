using UnityEngine;
using System.Collections;

public class SaludEnemigo : MonoBehaviour
{
    [Header("Estadísticas")]
    public float vidaMaxima = 15f; // Con tus 5 de daño, morirá en 3 golpes
    private float vidaActual;

    private SpriteRenderer spriteRenderer;
    
    // --- NUEVO: Variables para controlar la muerte y animación ---
    private Animator animator;
    private bool muerto = false;
    private EnemyAudio enemyAudio;

    void Start()
    {
        vidaActual = vidaMaxima;
        enemyAudio = GetComponent<EnemyAudio>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); // Obtiene el Animator del espíritu
    }

    public void RecibirDano(float cantidad)
    {
        if (muerto) return; // Si ya está haciendo la animación de muerte, ignora más daño

        vidaActual -= cantidad;
        
        // Inicia el efecto visual de recibir daño
        StartCoroutine(ParpadeoRojo());

        // Si la vida llega a 0, desaparece
        if (vidaActual <= 0)
        {
            Morir();
        }
        else if (enemyAudio != null)
        {
            enemyAudio.PlayHurt(); // solo suena "herido" si sigue vivo
        }
    }

    IEnumerator ParpadeoRojo()
    {
        // Se tiñe de rojo
        spriteRenderer.color = Color.red;
        
        // Espera una fracción de segundo
        yield return new WaitForSeconds(0.15f);
        
        // Vuelve a su color original, SOLO si no ha muerto
        if (!muerto && spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }

    void Morir()
    {
        if (enemyAudio != null) enemyAudio.PlayDeath();

        if (spriteRenderer != null) spriteRenderer.color = Color.white;

        // 1. Apagamos su colisión para que el granjero lo pueda atravesar
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // 2. Apagamos su script de persecución para que deje de seguirte
        Comportamiento_espiritus ia = GetComponent<Comportamiento_espiritus>();
        if (ia != null) ia.enabled = false;

        // 3. Lo frenamos en seco (por si estaba empujado por físicas)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // 4. Activamos la animación
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // 5. Esperamos a que la animación termine antes de borrarlo
        StartCoroutine(DestruirDespuesDeMuerte());
    }

    IEnumerator DestruirDespuesDeMuerte()
    {
        // Puedes cambiar este "0.8f" por los segundos exactos que dure tu animación de muerte
        yield return new WaitForSeconds(2f); 
        Destroy(gameObject);
    }
}