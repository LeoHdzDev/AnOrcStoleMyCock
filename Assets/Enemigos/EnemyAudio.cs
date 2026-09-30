using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyAudio : MonoBehaviour
{
    [Header("Ataque")]
    [SerializeField] private AudioClip[] clipsAtaque;
    [SerializeField, Range(0f, 1f)] private float volumenAtaque = 1f;

    [Header("Herido")]
    [SerializeField] private AudioClip[] clipsHerido;
    [SerializeField, Range(0f, 1f)] private float volumenHerido = 1f;

    [Header("Muerte")]
    [SerializeField] private AudioClip[] clipsMuerte;
    [SerializeField, Range(0f, 1f)] private float volumenMuerte = 1f;

    [Header("Risa (solo la primera vez que ve al jugador)")]
    [SerializeField] private AudioClip[] clipsRisa;
    [SerializeField, Range(0f, 1f)] private float volumenRisa = 1f;

    [Header("Detecta al jugador (solo la primera vez)")]
    [SerializeField] private AudioClip[] clipsDeteccion;
    [SerializeField, Range(0f, 1f)] private float volumenDeteccion = 1f;

    [Header("Ataque preparado (antes de golpear)")]
    [SerializeField] private AudioClip[] clipsPreparado;
    [SerializeField, Range(0f, 1f)] private float volumenPreparado = 1f;

    [Header("Confundido")]
    [SerializeField] private AudioClip[] clipsConfundido;
    [SerializeField, Range(0f, 1f)] private float volumenConfundido = 1f;

    [Header("Lanzamiento (bomba)")]
    [SerializeField] private AudioClip[] clipsLanzamiento;
    [SerializeField, Range(0f, 1f)] private float volumenLanzamiento = 1f;

    [Header("Explosión")]
    [SerializeField] private AudioClip clipExplosion;
    [SerializeField, Range(0f, 1f)] private float volumenExplosion = 1f;

    [Header("Variación de pitch")]
    [SerializeField] private Vector2 rangoPitch = new Vector2(0.95f, 1.05f);

    private AudioSource audioSource;
    private int ultimoAtaque = -1;
    private int ultimoHerido = -1;
    private int ultimoRisa = -1;
    private int ultimoDeteccion = -1;
    private int ultimoPreparado = -1;
    private int ultimoConfundido = -1;
    private int ultimoLanzamiento = -1;
    private bool yaSeRio = false;
    private bool yaDetecto = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void PlayAttack() { ReproducirAleatorio(clipsAtaque, volumenAtaque, ref ultimoAtaque); }
    public void PlayHurt() { ReproducirAleatorio(clipsHerido, volumenHerido, ref ultimoHerido); }
    public void PlayPrepare() { ReproducirAleatorio(clipsPreparado, volumenPreparado, ref ultimoPreparado); }
    public void PlayLaunch() { ReproducirAleatorio(clipsLanzamiento, volumenLanzamiento, ref ultimoLanzamiento); }

    // Se puede llamar desde un Animation Event (sin parámetros)
    public void PlayConfused() { ReproducirAleatorio(clipsConfundido, volumenConfundido, ref ultimoConfundido); }

    // Suena solo la primera vez, aunque se llame muchas veces
    public void PlayLaughOnce()
    {
        if (yaSeRio) return;
        yaSeRio = true;
        ReproducirAleatorio(clipsRisa, volumenRisa, ref ultimoRisa);
    }

    // Suena cada vez que se llama (por ejemplo, en cada lanzamiento de bomba)
    public void PlayLaugh() { ReproducirAleatorio(clipsRisa, volumenRisa, ref ultimoRisa); }

    public void PlayDetectOnce()
    {
        if (yaDetecto) return;
        yaDetecto = true;
        ReproducirAleatorio(clipsDeteccion, volumenDeteccion, ref ultimoDeteccion);
    }

    // Muerte y explosión usan un objeto temporal para que no se corten si el enemigo se destruye
    public void PlayDeath()
    {
        if (clipsMuerte == null || clipsMuerte.Length == 0) return;
        AudioClip clip = clipsMuerte[Random.Range(0, clipsMuerte.Length)];
        PlayDetached(clip, volumenMuerte);
    }

    public void PlayExplosion()
    {
        PlayDetached(clipExplosion, volumenExplosion);
    }

    // Reproduce un clip en un objeto temporal 2D (mismo volumen sin importar la distancia a la cámara)
    public static void PlayDetached(AudioClip clip, float volumen)
    {
        if (clip == null) return;

        GameObject go = new GameObject("SFX_" + clip.name);
        AudioSource src = go.AddComponent<AudioSource>();
        src.clip = clip;
        src.volume = volumen;
        src.spatialBlend = 0f;
        src.playOnAwake = false;
        src.Play();
        Destroy(go, clip.length + 0.1f);
    }

    private void ReproducirAleatorio(AudioClip[] clips, float volumen, ref int ultimoIndice)
    {
        if (clips == null || clips.Length == 0) return;

        int indice = Random.Range(0, clips.Length);

        if (clips.Length > 1 && indice == ultimoIndice)
            indice = (indice + 1 + Random.Range(0, clips.Length - 1)) % clips.Length;

        ultimoIndice = indice;

        audioSource.pitch = Random.Range(rangoPitch.x, rangoPitch.y);
        audioSource.PlayOneShot(clips[indice], volumen);
    }
}
