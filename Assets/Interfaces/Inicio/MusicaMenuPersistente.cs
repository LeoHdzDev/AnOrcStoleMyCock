using System.Collections;
using UnityEngine;

// Hace que la música del menú siga sonando al pasar a la escena del cómic.
// MenuManager.Jugar() lo agrega solo al objeto de la música: no hace falta ponerlo a mano.
[RequireComponent(typeof(AudioSource))]
public class MusicaMenuPersistente : MonoBehaviour
{
    public static MusicaMenuPersistente Instance { get; private set; }

    private AudioSource audioSource;

    void Awake()
    {
        // Si todavía quedaba una música anterior, gana la nueva
        if (Instance != null && Instance != this)
            Destroy(Instance.gameObject);

        Instance = this;
        audioSource = GetComponent<AudioSource>();

        // DontDestroyOnLoad solo funciona con objetos raíz
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // Baja el volumen poco a poco y destruye la música.
    public void Detener(float duracion = 1f)
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutYDestruir(duracion));
    }

    IEnumerator FadeOutYDestruir(float duracion)
    {
        float inicial = audioSource.volume;
        float t = 0f;
        while (t < duracion)
        {
            t += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(inicial, 0f, t / duracion);
            yield return null;
        }
        Destroy(gameObject);
    }
}
