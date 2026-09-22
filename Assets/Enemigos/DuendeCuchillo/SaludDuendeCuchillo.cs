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
        if (muerto)
            return;

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

        spriteRenderer.color = Color.white;
    }

    void Morir()
    {
        muerto = true;

        // Evitamos que siga moviéndose.
        DuendeCuchilloController controlador =
            GetComponent<DuendeCuchilloController>();

        if (controlador != null)
        {
            controlador.enabled = false;
        }

        // Activamos la animación de muerte.
        if (animator != null)
        {
            animator.SetBool("Muerto", true);
        }

        // Esperamos antes de destruir el objeto.
        StartCoroutine(DestruirDespuesDeMorir());
    }

    IEnumerator DestruirDespuesDeMorir()
    {
        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }
}