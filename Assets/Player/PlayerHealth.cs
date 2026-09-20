using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Salud del Granjero")]
    public float saludMaxima = 100f;
    private float saludActual;
    private bool estaMuerto = false;

    [Header("Interfaz (UI)")]
    public Image barraRelleno;

    private Animator animator;
    private PlayerController playerController;
    private Rigidbody2D rb;

    void Start()
    {
        saludActual = saludMaxima;
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        ActualizarBarra();
    }

    void Update()
    {
        // Si ya es calavera, ignoramos las teclas
        if (estaMuerto) return; 

        // Tecla de prueba para recibir daño (aumenté a 20 para probar más rápido)
        if (Input.GetKeyDown(KeyCode.E))
        {
            RecibirDano(5f); 
        }
    }

    public void RecibirDano(float cantidad)
    {
        if (estaMuerto) return;

        saludActual -= cantidad;
        
        if (saludActual <= 0)
        {
            saludActual = 0;
            Morir();
        }
        
        ActualizarBarra();
    }

    void ActualizarBarra()
    {
        float porcentaje = saludActual / saludMaxima;
        barraRelleno.fillAmount = porcentaje;

        if (porcentaje > 0.75f) barraRelleno.color = Color.green;
        else if (porcentaje > 0.50f) barraRelleno.color = Color.yellow;
        else if (porcentaje > 0.25f) barraRelleno.color = new Color(1f, 0.5f, 0f);
        else barraRelleno.color = Color.red;
    }

    void Morir()
    {
        estaMuerto = true;
        
        // 1. Apagar el script de movimiento para que no pueda caminar ni atacar
        playerController.enabled = false;
        
        // 2. Frenar cualquier deslizamiento físico residual
        rb.linearVelocity = Vector2.zero;

        // 3. Disparar la animación de muerte
        animator.SetTrigger("Die");
    }
}