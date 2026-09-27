using UnityEngine;

// Habilidad del elemento Fuego: círculo rojo translúcido alrededor del
// jugador que aplica quemadura a los enemigos que entren en él.
// El daño real con el tiempo lo maneja el componente EstadoElemental
// de cada enemigo (así funciona incluso después de salir del área).
//
// USO: igual que BurbujaAgua.
// 1) GameObject vacío hijo de Farmer_player, ej. "AreaFuego", en (0,0,0).
// 2) Agrégale un CircleCollider2D.
// 3) Agrégale este script.
// 4) Déjalo desactivado por defecto (el PlayerController lo enciende/apaga).
//
// IMPORTANTE: cada enemigo necesita el componente "EstadoElemental" para
// que esta área le haga algo (si no lo tiene, simplemente lo ignora).
[RequireComponent(typeof(CircleCollider2D))]
public class AreaFuego : MonoBehaviour
{
    [Header("Tamaño del área")]
    public float radio = 2.5f;

    [Header("Quemadura")]
    [Tooltip("Daño por segundo mientras el enemigo esté quemándose.")]
    public float danoQuemaduraPorSegundo = 4f;
    [Tooltip("Cada cuánto se aplica un 'tick' de daño.")]
    public float intervaloTick = 1f;
    [Tooltip("Cuánto sigue quemándose el enemigo después de salir del área.")]
    public float duracionSinContacto = 2f;

    [Header("Apariencia")]
    public Color colorArea = new Color(1f, 0.25f, 0.2f, 0.35f);
    public int ordenDeDibujado = 5;

    private SpriteRenderer sr;
    private CircleCollider2D colCirculo;

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
    }

    public void Desactivar()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        EstadoElemental estado = other.GetComponent<EstadoElemental>();
        if (estado == null) return;

        estado.AplicarQuemadura(danoQuemaduraPorSegundo * intervaloTick, intervaloTick, duracionSinContacto);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radio);
    }
}
