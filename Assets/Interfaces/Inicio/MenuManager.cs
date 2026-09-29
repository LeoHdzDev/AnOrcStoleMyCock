using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Contenedores")]
    public GameObject menuPrincipal;
    public GameObject panelAjustes;

    [Header("Audio")]
    public AudioSource musicaFondo;
    public Slider sliderVolumen;

    void Start()
    {
        if (musicaFondo != null && sliderVolumen != null)
        {
            sliderVolumen.value = musicaFondo.volume;
        }
    }

    public void CambiarVolumen(float nuevoVolumen)
    {
        if (musicaFondo != null)
        {
            musicaFondo.volume = nuevoVolumen;
        }
    }

    public void Jugar()
    {
        SceneManager.LoadScene("Stage");
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