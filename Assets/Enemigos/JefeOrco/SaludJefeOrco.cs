using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SaludJefeOrco : MonoBehaviour
{
    [Header("Estadísticas del Jefe")]
    public float vidaMaxima = 100f;
    private float vidaActual;
    private bool muerto = false;
    private EnemyAudio enemyAudio;

    [Header("Interfaz de Usuario")]
    public GameObject panelBarraJefe; 
    public Image barraRelleno;        

    private Animator animator;
    private JefeOrcoController controlador;
    
    // --- NUEVO: Referencia al dibujo del orco ---
    private SpriteRenderer spriteRenderer; 

    void Start()
    {
        vidaActual = vidaMaxima;
        enemyAudio = GetComponent<EnemyAudio>();
        animator = GetComponent<Animator>();
        controlador = GetComponent<JefeOrcoController>();
        
        // --- NUEVO: Obtener el SpriteRenderer ---
        spriteRenderer = GetComponent<SpriteRenderer>(); 

        // if (panelBarraJefe != null) panelBarraJefe.SetActive(false);
    }

    public void ActivarJefe()
    {
        if (panelBarraJefe != null) panelBarraJefe.SetActive(true);
        if (controlador != null) controlador.DespertarJefe();
        ActualizarBarra();
    }

    public void RecibirDano(float cantidad)
    {
        if (muerto) return;
        
        vidaActual -= cantidad;
        ActualizarBarra();

        // --- NUEVO: Activar el parpadeo al recibir daño ---
        if (spriteRenderer != null)
        {
            StartCoroutine(ParpadeoRojo());
        }

        if (vidaActual <= 0) Morir();
    }

    // --- NUEVA CORRUTINA: El efecto visual ---
    IEnumerator ParpadeoRojo()
    {
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(0.15f);

        // Solo regresa a su color normal si sigue vivo
        if (!muerto && spriteRenderer != null) 
        {
            spriteRenderer.color = Color.white;
        }
    }

    void ActualizarBarra()
    {
        if (barraRelleno != null) barraRelleno.fillAmount = vidaActual / vidaMaxima;
    }

    void Morir()
    {
        muerto = true;
        if (enemyAudio != null) enemyAudio.PlayDeath();

        if (MusicaEscena.Instance != null) MusicaEscena.Instance.Detener(1.5f);
        
        if (controlador != null) controlador.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        
        // Limpiamos el color por si el último golpe lo dejó rojo
        if (spriteRenderer != null) spriteRenderer.color = Color.white;
        
        if (animator != null) animator.SetTrigger("Die");
        if (panelBarraJefe != null) panelBarraJefe.SetActive(false);
        
        StartCoroutine(DestruirJefe());
    }

    IEnumerator DestruirJefe()
    {
        yield return new WaitForSeconds(2.5f);
        Destroy(gameObject);
    }
}