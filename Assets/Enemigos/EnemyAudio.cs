using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyAudio : MonoBehaviour
{
    [Header("Ataque")]
    [SerializeField] private AudioClip[] clipsAtaque;
    [SerializeField, Range(0f, 1f)] private float volumenAtaque = 1f;

    [Header("Muerte")]
    [SerializeField] private AudioClip[] clipsMuerte;
    [SerializeField, Range(0f, 1f)] private float volumenMuerte = 1f;

    [Header("Risa (solo la primera vez que ve al jugador)")]
    [SerializeField] private AudioClip[] clipsRisa;
    [SerializeField, Range(0f, 1f)] private float volumenRisa = 1f;

    [Header("Variación de pitch")]
    [SerializeField] private Vector2 rangoPitch = new Vector2(0.95f, 1.05f);

    private AudioSource audioSource;
    private int ultimoAtaque = -1;
    private int ultimoRisa = -1;
    private bool yaSeRio = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void PlayAttack()
    {
        ReproducirAleatorio(clipsAtaque, volumenAtaque, ref ultimoAtaque);
    }

    // Llamar cuando detecta al jugador; solo suena la primera vez
    public void PlayLaughOnce()
    {
        if (yaSeRio) return;
        yaSeRio = true;
        ReproducirAleatorio(clipsRisa, volumenRisa, ref ultimoRisa);
    }

    // Usa PlayClipAtPoint para que el sonido no se corte si el enemigo se destruye
    public void PlayDeath()
    {
        if (clipsMuerte == null || clipsMuerte.Length == 0) return;
        AudioClip clip = clipsMuerte[Random.Range(0, clipsMuerte.Length)];
        AudioSource.PlayClipAtPoint(clip, transform.position, volumenMuerte);
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