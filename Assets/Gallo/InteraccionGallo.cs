using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

// Se coloca en el GameObject del Gallo (con un CircleCollider2D en modo Trigger).
// Cuando el jugador entra en el rango, muestra el mensaje "Presiona E para
// interactuar". Al presionar E, reproduce un video a pantalla completa y,
// quinte termina, carga la escena del menú principal.
public class InteraccionGallo : MonoBehaviour
{
    [Header("Mensaje de interacción (UI, empieza desactivado)")]
    [SerializeField] private GameObject promptUI;

    [Header("Video de cierre")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Escena a cargar al terminar el video")]
    [SerializeField] private string nombreEscenaMenu = "MenuInicio";

    private bool jugadorCerca = false;
    private bool reproduciendo = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            if (promptUI != null) promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            if (promptUI != null) promptUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (jugadorCerca && !reproduciendo && Input.GetKeyDown(KeyCode.E))
        {
            IniciarVideo();
        }
    }

    private void IniciarVideo()
    {
        reproduciendo = true;

        if (promptUI != null) promptUI.SetActive(false);

        // Pausa el juego (enemigos, jugador, etc.) mientras se ve el video.
        // El VideoPlayer sigue reproduciéndose normalmente aunque el tiempo
        // del juego esté congelado.
        Time.timeScale = 0f;

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoTerminado;
            videoPlayer.Play();
        }
        else
        {
            // Por si olvidaste asignar el VideoPlayer, no dejamos al jugador
            // atascado para siempre.
            OnVideoTerminado(null);
        }
    }

    private void OnVideoTerminado(VideoPlayer vp)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscenaMenu);
    }
}
