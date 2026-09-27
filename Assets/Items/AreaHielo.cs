using System.Collections.Generic;
using UnityEngine;

// Habilidad del elemento Hielo: círculo blanco translúcido alrededor del
// jugador que reduce la velocidad y el daño de los enemigos mientras
// estén dentro. El efecto en sí lo aplica el componente EstadoElemental
// de cada enemigo.
//
// USO: igual que BurbujaAgua / AreaFuego.
// 1) GameObject vacío hijo de Farmer_player, ej. "AreaHielo", en (0,0,0).
// 2) Agrégale un CircleCollider2D.
// 3) Agrégale este script.
// 4) Déjalo desactivado por defecto.
//
// IMPORTANTE: cada enemigo necesita el componente "EstadoElemental".
[RequireComponent(typeof(CircleCollider2D))]
public class AreaHielo : MonoBehaviour
{
    [Header("Tamaño del área")]
    public float radio = 2.5f;

    [Header("Ralentización")]
    [Range(0.05f, 1f)] public float factorVelocidad = 0.5f; // 0.5 = mitad de velocidad
    [Range(0.05f, 1f)] public float factorDano = 0.5f;       // 0.5 = mitad de daño
    [Tooltip("Margen para que el efecto no parpadee si el collider tarda un frame en salir.")]
    public float margenSinContacto = 0.2f;

    [Header("Daño")]
    [Tooltip("Daño por segundo mientras el enemigo esté dentro del área (solo mientras esté adentro, no persiste al salir).")]
    public float danoPorSegundo = 3f;
    [Tooltip("Cada cuánto se aplica un 'tick' de daño.")]
    public float intervaloTickDano = 0.5f;

    [Header("Apariencia")]
    public Color colorArea = new Color(1f, 1f, 1f, 0.35f);
    public int ordenDeDibujado = 5;

    private SpriteRenderer sr;
    private CircleCollider2D colCirculo;
    private readonly Dictionary<GameObject, float> ultimoTickDano = new Dictionary<GameObject, float>();

    void Awake()
    {
        colCirculo = GetComponent<CircleCollider2D>();
        colCirculo.isTrigger = true;

        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = gameObject.AddComponent<SpriteRenderer>();

        sr.sprite = GenerarSpriteCirculo();
        sr.color = colorArea;
        sr.sortingOrder = ordenDeDibujado;

        AplicarRadio();
    }

    void OnValidate()
    {
        if (colCirculo == null) colCirculo = GetComponent<CircleCollider2D>();
        if (colCirculo != null) AplicarRadio();
    }

    void AplicarRadio()
    {
        colCirculo.radius = 0.5f;
        transform.localScale = Vector3.one * (radio * 2f);
    }

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
                alpha = Mathf.SmoothStep(0f, 1f, alpha);
                textura.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        textura.Apply();
        return Sprite.Create(textura, new Rect(0, 0, tam, tam), new Vector2(0.5f, 0.5f), tam);
    }

    public void Activar()
    {
        gameObject.SetActive(true);
        ultimoTickDano.Clear();
    }

    public void Desactivar()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        EstadoElemental estado = other.GetComponent<EstadoElemental>();
        if (estado == null) return;

        estado.AplicarCongelamiento(factorVelocidad, factorDano, margenSinContacto);

        GameObject enemigo = other.gameObject;
        if (ultimoTickDano.TryGetValue(enemigo, out float ultimaVez) && Time.time - ultimaVez < intervaloTickDano)
            return;

        estado.RecibirGolpe(danoPorSegundo * intervaloTickDano);
        ultimoTickDano[enemigo] = Time.time;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, radio);
    }
}