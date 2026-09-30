using UnityEngine;
using UnityEngine.SceneManagement;

// Colócalo en un GameObject "vacío" dentro de la escena de juego (no en el
// panel mismo), por ejemplo uno llamado "GestorDePausa".
public class MenuPausa : MonoBehaviour
{
    [Header("Panel de pausa (empieza desactivado)")]
    [SerializeField] private GameObject panelPausa;

    [Header("Opcional: desactiva al jugador mientras está pausado")]
    [SerializeField] private PlayerController jugador;

    [Header("Escena del menú principal")]
    [SerializeField] private string nombreEscenaMenu = "MenuInicio";

    private bool pausado = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausado) Reanudar();
            else Pausar();
        }
    }

    public void Pausar()
    {
        pausado = true;
        panelPausa.SetActive(true);
        Time.timeScale = 0f;

        if (jugador != null) jugador.enabled = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Reanudar()
    {
        pausado = false;
        panelPausa.SetActive(false);
        Time.timeScale = 1f;

        if (jugador != null) jugador.enabled = true;
    }

    // Conecta esto al evento "On Value Changed" del Slider de volumen.
    public void CambiarVolumen(float valor)
    {
        AudioListener.volume = valor;
    }

    // Conecta esto al botón "Salir al menú principal".
    public void SalirAMenuPrincipal()
    {
        Time.timeScale = 1f; // MUY importante: si no, el menú principal carga congelado
        SceneManager.LoadScene(nombreEscenaMenu);
    }
}
