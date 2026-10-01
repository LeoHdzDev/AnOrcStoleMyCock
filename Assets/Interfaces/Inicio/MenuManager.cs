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

    [Header("Escenas")]
    public string escenaIntroduccion = "Comic";

    public void Jugar()
    {
        // La música del menú sigue sonando durante el cómic
        if (musicaFondo != null)
        {
            if (musicaFondo.GetComponent<MusicaMenuPersistente>() == null)
                musicaFondo.gameObject.AddComponent<MusicaMenuPersistente>();
        }

        SceneManager.LoadScene(escenaIntroduccion);
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