using UnityEngine;
using System.Collections;

public class SaludEnemigo : MonoBehaviour
{
    [Header("Estadísticas")]
    public float vidaMaxima = 15f; // Con tus 5 de daño, morirá en 3 golpes
    private float vidaActual;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        vidaActual = vidaMaxima;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void RecibirDano(float cantidad)
    {
        vidaActual -= cantidad;
        
        // Inicia el efecto visual de recibir daño
        StartCoroutine(ParpadeoRojo());

        // Si la vida llega a 0, desaparece
        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    IEnumerator ParpadeoRojo()
    {
        // Se tiñe de rojo
        spriteRenderer.color = Color.red;
        
        // Espera una fracción de segundo
        yield return new WaitForSeconds(0.15f);
        
        // Vuelve a su color original
        spriteRenderer.color = Color.white;
    }

    void Morir()
    {
        // Por ahora lo destruimos (desaparece). Más adelante puedes cambiar esto por una animación.
        Destroy(gameObject);
    }
}