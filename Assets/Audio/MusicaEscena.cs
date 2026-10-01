using System.Collections;
using UnityEngine;

// Reproduce una música en bucle al empezar la escena.
// Ponlo en un GameObject vacío (por ejemplo "MusicaJefe") de la escena.
[RequireComponent(typeof(AudioSource))]
public class MusicaEscena : MonoBehaviour
{
    [Header("Música")]
    [SerializeField] private AudioClip musica;
    [SerializeField, Range(0f, 1f)] private float volumen = 0.8f;
    [SerializeField] private bool repetir = true;

    [Header("Transición")]
    [SerializeField] private float duracionFadeIn = 1.5f;

    private AudioSource audioSource;
    private Coroutine fadeActual;
    public static MusicaEscena Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = repetir;
        audioSource.spatialBlend = 0f; // música 2D: suena igual en todo el mapa
        audioSource.clip = musica;
    }

    void Start()
    {
        Reproducir();
    }

    public void Reproducir()
    {
        if (musica == null) return;

        if (fadeActual != null) StopCoroutine(fadeActual);
        fadeActual = StartCoroutine(FadeIn());
    }

    // Baja el volumen poco a poco y detiene la música.
    public void Detener(float duracionFade = 1.5f)
    {
        if (fadeActual != null) StopCoroutine(fadeActual);
        fadeActual = StartCoroutine(FadeOut(duracionFade));
    }

    IEnumerator FadeIn()
    {
        audioSource.volume = 0f;
        audioSource.Play();

        float t = 0f;
        while (t < duracionFadeIn)
        {
            t += Time.unscaledDeltaTime; // funciona aunque el juego esté en pausa
            audioSource.volume = Mathf.Lerp(0f, volumen, t / duracionFadeIn);
            yield return null;
        }
        audioSource.volume = volumen;
    }

    IEnumerator FadeOut(float duracion)
    {
        float inicial = audioSource.volume;
        float t = 0f;
        while (t < duracion)
        {
            t += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(inicial, 0f, t / duracion);
            yield return null;
        }
        audioSource.Stop();
    }
}
