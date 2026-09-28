using System.Collections.Generic;
using UnityEngine;

// Habilidad del elemento Agua: un círculo translúcido alrededor del
// jugador que empuja y daña a los enemigos que entren en él.
//
// USO:
// 1) Crea un GameObject VACÍO como HIJO de Farmer_player, llamado
//    por ejemplo "BurbujaAgua", en la posición local (0,0,0).
// 2) Agrégale un componente CircleCollider2D (el script lo configura
//    solo, no necesitas tocarlo).
// 3) Agrégale este script (BurbujaAgua).
// 4) Deja el GameObject "BurbujaAgua" desactivado en la escena
//    (el propio PlayerController lo va a activar/desactivar).
[RequireComponent(typeof(CircleCollider2D))]
public class BurbujaAgua : MonoBehaviour
{
    [Header("Tamaño de la burbuja")]
    public float radio = 2.5f;

    [Header("Daño")]
    [Tooltip("Daño total por segundo mientras el enemigo esté dentro.")]
    public float danoPorSegundo = 8f;
    [Tooltip("Cada cuánto se aplica daño/empuje a un mismo enemigo.")]
    public float tiempoEntreDanos = 0.5f;

    [Header("Empuje")]
    public float fuerzaEmpuje = 0.6f;

    [Header("Apariencia")]
    public Color colorBurbuja = new Color(0.3f, 0.7f, 1f, 0.35f);
    public int ordenDeDibujado = 5;

    private SpriteRenderer sr;
    private CircleCollider2D colCirculo;
    private readonly Dictionary<GameObject, float> ultimoGolpe = new Dictionary<GameObject, float>();

    void Awake()
    {
        colCirculo = GetComponent<CircleCollider2D>();
        colCirculo.isTrigger = true;

        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = gameObject.AddComponent<SpriteRenderer>();

        sr.sprite = GenerarSpriteCirculo();
        sr.color = colorBurbuja;
        sr.sortingOrder = ordenDeDibujado;

        AplicarRadio();
    }

    void OnValidate()
    {
        // Para poder ajustar "radio" en el Inspector y verlo reflejado al instante.
        if (colCirculo == null) colCirculo = GetComponent<CircleCollider2D>();
        if (colCirculo != null) AplicarRadio();
    }

    void AplicarRadio()
    {
        colCirculo.radius = 0.5f; // el sprite generado mide 1 unidad de diámetro a escala 1
        transform.localScale = Vector3.one * (radio * 2f);
    }

    // Genera un círculo suave (más translúcido hacia el borde) sin necesitar una imagen.
    Sprite GenerarSpriteCirculo()
    {
        int tam = 128;
        Texture2D textura = new Texture2D(tam, tam, TextureFormat.RGBA32, false);
        Vector2 centro = new Vector2(tam / 2f, tam / 2f);
        float radioPx = tam / 2f;

        for (int y = 0; y < tam; y++)
        {
            for (int x = 0; x < tam; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), centro);
                float alpha = Mathf.Clamp01(1f - (dist / radioPx));
                alpha = Mathf.SmoothStep(0f, 1f, alpha); // borde más suave
                textura.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        textura.Apply();
        return Sprite.Create(textura, new Rect(0, 0, tam, tam), new Vector2(0.5f, 0.5f), tam);
    }

    public void Activar()
    {
        gameObject.SetActive(true);
        ultimoGolpe.Clear();
    }

    public void Desactivar()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        GameObject enemigo = other.gameObject;

        if (ultimoGolpe.TryGetValue(enemigo, out float ultimaVez))
        {
            if (Time.time - ultimaVez < tiempoEntreDanos) return;
        }

        float dano = danoPorSegundo * tiempoEntreDanos;
        bool golpeado = false;

        SaludEnemigo saludEspiritu = enemigo.GetComponent<SaludEnemigo>();
        if (saludEspiritu != null) { saludEspiritu.RecibirDano(dano); golpeado = true; }

        ComportamientoSlime slime = enemigo.GetComponent<ComportamientoSlime>();
        if (slime != null) { slime.RecibirDano(dano); golpeado = true; }

        SaludDuendeCuchillo duende = enemigo.GetComponent<SaludDuendeCuchillo>();
        if (duende != null) { duende.RecibirDano(dano); golpeado = true; }

        if (golpeado)
        {
            ultimoGolpe[enemigo] = Time.time;

            // Empuje hacia afuera del centro de la burbuja.
            // Se mueve directamente el transform (no se usa AddForce) para que
            // funcione igual con enemigos de Rigidbody2D Kinematic o Dynamic.
            Vector2 direccion = ((Vector2)enemigo.transform.position - (Vector2)transform.position).normalized;
            enemigo.transform.position += (Vector3)(direccion * fuerzaEmpuje);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radio);
    }
}
