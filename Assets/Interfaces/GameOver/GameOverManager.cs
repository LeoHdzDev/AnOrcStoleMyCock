using System.Collections;
using UnityEngine;
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

    // Llama esto desde PlayerHealth cuando el jugador muere.
    public void ProgramarGameOver()
    {
        StartCoroutine(RutinaGameOver());
    }

    IEnumerator RutinaGameOver()
    {
        yield return new WaitForSeconds(retraso);

        panelGameOver.SetActive(true);
        Time.timeScale = 0f;

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
