using System.Collections;
using UnityEngine;

public class SaludDuendeCuchillo : MonoBehaviour
{
    [Header("Estadísticas")]
    public float vidaMaxima = 15f;

    private float vidaActual;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool muerto = false;

    void Start()
    {
        vidaActual = vidaMaxima;

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void RecibirDano(float cantidad)
    {
        // Si ya está muerto, no recibe más daño.
        if (muerto) return;

        vidaActual -= cantidad;

        // Efecto visual de daño.
        if (spriteRenderer != null)
        {
            StartCoroutine(ParpadeoRojo());
        }

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    IEnumerator ParpadeoRojo()
    {
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(0.15f);

        // Solo regresa a blanco si sigue vivo (evita bugs visuales al morir)
        if (!muerto && spriteRenderer != null) 
        {
            spriteRenderer.color = Color.white;
        }
    }

    void Morir()
    {
        muerto = true;

        // 1. Evitamos que siga moviéndose.
        DuendeCuchilloController controlador = GetComponent<DuendeCuchilloController>();
        if (controlador != null)
        {
            controlador.enabled = false;
        }

        // 2. Apagamos su cuerpo físico para que el granjero lo pueda atravesar.
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // 3. Limpiamos su color por si murió justo en medio del parpadeo rojo.
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }

        // 4. Activamos la animación de muerte con el Gatillo exacto que creamos en el Animator.
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // 5. Esperamos antes de destruir el objeto.
        StartCoroutine(DestruirDespuesDeMorir());
    }

    IEnumerator DestruirDespuesDeMorir()
    {
        // Aumentado a 1.5f para asegurar que la animación se vea completa
        yield return new WaitForSeconds(1.5f);

        Destroy(gameObject);
    }
}