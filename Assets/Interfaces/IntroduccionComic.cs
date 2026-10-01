using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroduccionComic : MonoBehaviour
{
    [Header("Componentes")]
    public Image imagenDisplay;

    [Header("Secuencia de Viñetas")]
    public Sprite[] vinetas;

    [Header("Siguiente Escena")]
    public string nombreEscenaJuego = "Stage";

    [Header("Música del menú")]
    [Tooltip("Segundos que tarda en apagarse la música del menú al entrar al juego.")]
    public float fadeMusicaMenu = 1f;

    private int indiceActual = 0;
    private bool cargando = false;
    private float tiempoInicio;

    private void Start()
    {
        Time.timeScale = 1f;
        tiempoInicio = Time.time;

        if (vinetas != null && vinetas.Length > 0)
            MostrarVineta(0);
    }

    private void Update()
    {
        if (cargando) return;

        // Evita que el clic que acaba de abrir la escena avance la primera viñeta
        if (Time.time - tiempoInicio < 0.25f) return;

        bool clic = Input.GetMouseButtonDown(0);

        // Si el clic cae sobre un botón (Skip), que lo maneje el botón y no avance la viñeta
        if (clic && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            clic = false;

        if (clic || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            AvanzarVineta();
    }

    public void AvanzarVineta()
    {
        if (cargando) return;

        indiceActual++;

        if (indiceActual < vinetas.Length)
            MostrarVineta(indiceActual);
        else
            CargarJuego();
    }

    // Botón SKIP: se salta toda la historia
    public void Omitir()
    {
        CargarJuego();
    }

    private void MostrarVineta(int indice)
    {
        if (imagenDisplay != null && vinetas[indice] != null)
            imagenDisplay.sprite = vinetas[indice];
    }

    public void CargarJuego()
    {
        if (cargando) return;
        cargando = true;

        // La música del menú venía sonando durante el cómic; ahora se apaga suavemente
        if (MusicaMenuPersistente.Instance != null)
            MusicaMenuPersistente.Instance.Detener(fadeMusicaMenu);

        SceneManager.LoadScene(nombreEscenaJuego);
    }
}
