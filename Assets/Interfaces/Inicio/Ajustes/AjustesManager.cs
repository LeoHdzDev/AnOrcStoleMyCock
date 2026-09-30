using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class AjustesManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Sliders")]
    public Slider sliderMusica;
    public Slider sliderSFX;

    [Header("Resolución")]
    public TMP_Dropdown dropdownResoluciones;

    private Resolution[] resolucionesDisponibles;

    private void Start()
    {
        // 1. Cargar volumen guardado (0.75 por defecto)
        float volMusica = PlayerPrefs.GetFloat("VolMusica", 0.75f);
        float volSFX = PlayerPrefs.GetFloat("VolSFX", 0.75f);

        if (sliderMusica != null)
        {
            sliderMusica.value = volMusica;
            SetVolumenMusica(volMusica);
        }

        if (sliderSFX != null)
        {
            sliderSFX.value = volSFX;
            SetVolumenSFX(volSFX);
        }

        // 2. Configurar Dropdown de resoluciones
        if (dropdownResoluciones != null)
        {
            ConfigurarResoluciones();
        }
    }

    public void SetVolumenMusica(float valor)
    {
        float db = Mathf.Log10(Mathf.Clamp(valor, 0.0001f, 1f)) * 20f;
        if (audioMixer != null)
        {
            audioMixer.SetFloat("VolumenMusica", db);
        }
        PlayerPrefs.SetFloat("VolMusica", valor);
    }

    public void SetVolumenSFX(float valor)
    {
        float db = Mathf.Log10(Mathf.Clamp(valor, 0.0001f, 1f)) * 20f;
        if (audioMixer != null)
        {
            audioMixer.SetFloat("VolumenSFX", db);
        }
        PlayerPrefs.SetFloat("VolSFX", valor);
    }

    private void ConfigurarResoluciones()
    {
        resolucionesDisponibles = Screen.resolutions;
        dropdownResoluciones.ClearOptions();

        TMP_Dropdown.OptionData[] opciones = new TMP_Dropdown.OptionData[resolucionesDisponibles.Length];
        int indiceActual = 0;

        for (int i = 0; i < resolucionesDisponibles.Length; i++)
        {
            string textoOpcion = resolucionesDisponibles[i].width + " x " + resolucionesDisponibles[i].height;
            opciones[i] = new TMP_Dropdown.OptionData(textoOpcion);

            if (resolucionesDisponibles[i].width == Screen.currentResolution.width &&
                resolucionesDisponibles[i].height == Screen.currentResolution.height)
            {
                indiceActual = i;
            }
        }

        // Se agrega usando el array directamente
        dropdownResoluciones.options.AddRange(opciones);
        dropdownResoluciones.value = indiceActual;
        dropdownResoluciones.RefreshShownValue();
    }

    public void CambiarResolucion(int indice)
    {
        if (resolucionesDisponibles != null && indice < resolucionesDisponibles.Length)
        {
            Resolution res = resolucionesDisponibles[indice];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        }
    }

    public void CerrarVentanaAjustes()
    {
        PlayerPrefs.Save();
        gameObject.SetActive(false);
    }
}