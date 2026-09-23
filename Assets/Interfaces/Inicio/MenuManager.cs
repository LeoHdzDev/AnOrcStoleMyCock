using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void Jugar()
    {
        // Carga la siguiente escena en la lista de Build Settings
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Ajustes()
    {
        Debug.Log("Abriendo ajustes...");
        // Aquí podrás activar un panel de opciones más adelante
    }

    public void Salir()
    {
        Debug.Log("Cerrando el juego...");
        Application.Quit();
    }
}