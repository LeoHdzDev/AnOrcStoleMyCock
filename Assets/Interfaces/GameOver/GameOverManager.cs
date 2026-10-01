using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

// Colócalo en un GameObject vacío en la escena de juego, ej. "GestorGameOver".
public class GameOverManager : MonoBehaviour
{
    [Header("Panel de Game Over (empieza desactivado)")]
    [SerializeField] private GameObject panelGameOver;

    [Header("Retraso antes de mostrar la pantalla")]
    [SerializeField] private float retraso = 3f;

    [Header("Escenas")]
    [SerializeField] private string nombreEscenaMenu = "MenuInicio";
    [SerializeField] private string nombreEscenaStage = "Stage";

    [Header("Audio de Game Over")]
    [SerializeField] private AudioClip clipEfecto;   // AVISO: suena primero, cuando aparece el recuadro
    [SerializeField, Range(0f, 1f)] private float volumenEfecto = 1f;
    [SerializeField] private AudioClip clipMusica;   // GameOver: suena en bucle después del efecto
    [SerializeField, Range(0f, 1f)] private float volumenMusica = 0.6f;
    [Tooltip("Segundos desde que empieza el efecto hasta que entra la música. AVISO casi se apaga en el segundo 2.")]
    [SerializeField] private float segundosHastaMusica = 2f;

    [Header("Mixer (opcional)")]
    [SerializeField] private AudioMixerGroup grupoEfecto;  // Grupo SFX
    [SerializeField] private AudioMixerGroup grupoMusica;  // Grupo Musica

    private AudioSource fuenteEfecto;
    private AudioSource fuenteMusica;

    void Awake()
    {
        fuenteEfecto = CrearFuente(grupoEfecto, false);
        fuenteMusica = CrearFuente(grupoMusica, true);
    }

    AudioSource CrearFuente(AudioMixerGroup grupo, bool repetir)
    {
        AudioSource fuente = gameObject.AddComponent<AudioSource>();
        fuente.playOnAwake = false;
        fuente.loop = repetir;
        fuente.spatialBlend = 0f;
        fuente.outputAudioMixerGroup = grupo;
        return fuente;
    }

    // Llama esto desde PlayerHealth cuando el jugador muere.
    public void ProgramarGameOver()
    {
        // La música del nivel (mazmorra o jefe) se apaga al morir
        if (MusicaEscena.Instance != null) MusicaEscena.Instance.Detener(1f);

        StartCoroutine(RutinaGameOver());
    }

    // Efecto y música se programan con el reloj de audio, así no dependen
    // de Time.timeScale (que queda en 0 mientras se muestra el Game Over).
    void ReproducirAudioGameOver()
    {
        double inicio = AudioSettings.dspTime + 0.1;
        double inicioMusica = inicio;

        if (clipEfecto != null)
        {
            fuenteEfecto.clip = clipEfecto;
            fuenteEfecto.volume = volumenEfecto;
            fuenteEfecto.PlayScheduled(inicio);
            inicioMusica = inicio + segundosHastaMusica;
        }

        if (clipMusica != null)
        {
            fuenteMusica.clip = clipMusica;
            fuenteMusica.volume = volumenMusica;
            fuenteMusica.PlayScheduled(inicioMusica);
        }
    }

    IEnumerator RutinaGameOver()
    {
        yield return new WaitForSeconds(retraso);

        panelGameOver.SetActive(true);
        Time.timeScale = 0f;

        ReproducirAudioGameOver();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Conecta esto al botón "Menú" (evento OnClick).
    public void IrAMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscenaMenu);
    }

    // Conecta esto al botón "Retry" (evento OnClick).
    public void ReintentarStage()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscenaStage);
    }
}
