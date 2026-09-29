using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Contenedores")]
    public GameObject menuPrincipal;
    public GameObject panelAjustes;

    public void Jugar()
    {
        SceneManager.LoadScene(1);
    }

    public void AbrirAjustes()
    {
        menuPrincipal.SetActive(false);
        panelAjustes.SetActive(true);
    }

    public void CerrarAjustes()
    {
        panelAjustes.SetActive(false);
        menuPrincipal.SetActive(true);
    }

    public void Salir()
    {
        Application.Quit();
        Debug.Log("Cerrando el juego...");
    }
}