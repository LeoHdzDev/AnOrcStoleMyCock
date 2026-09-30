using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
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

    [Header("Curación")]
    [SerializeField] private AudioClip[] clipsCuracion;
    [SerializeField, Range(0f, 1f)] private float volumenCuracion = 1f;

    [Header("Recoger objeto")]
    [SerializeField] private AudioClip[] clipsRecoger;
    [SerializeField, Range(0f, 1f)] private float volumenRecoger = 1f;

    [Header("Habilidades (aura)")]
    [SerializeField] private AudioClip clipAuraAgua;
    [SerializeField] private AudioClip clipAuraFuego;
    [SerializeField] private AudioClip clipAuraHielo;
    [SerializeField, Range(0f, 1f)] private float volumenAura = 1f;

    [Header("Variación de pitch")]
    [SerializeField] private Vector2 rangoPitch = new Vector2(0.95f, 1.05f);

    private AudioSource audioSource;
    private int ultimoAtaque = -1;
    private int ultimoHerido = -1;
    private int ultimoMuerte = -1;
    private int ultimoCuracion = -1;
    private int ultimoRecoger = -1;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void PlayAttack()
    {
        ReproducirAleatorio(clipsAtaque, volumenAtaque, ref ultimoAtaque);
    }

    public void PlayHurt()
    {
        ReproducirAleatorio(clipsHerido, volumenHerido, ref ultimoHerido);
    }

    public void PlayDeath()
    {
        ReproducirAleatorio(clipsMuerte, volumenMuerte, ref ultimoMuerte);
    }

    public void PlayHeal()
    {
        ReproducirAleatorio(clipsCuracion, volumenCuracion, ref ultimoCuracion);
    }

    public void PlayPickup()
    {
        ReproducirAleatorio(clipsRecoger, volumenRecoger, ref ultimoRecoger);
    }

    public void PlayAbility(PlayerController.ElementType elemento)
    {
        AudioClip clip = null;
        switch (elemento)
        {
            case PlayerController.ElementType.Water: clip = clipAuraAgua; break;
            case PlayerController.ElementType.Fire: clip = clipAuraFuego; break;
            case PlayerController.ElementType.Ice: clip = clipAuraHielo; break;
        }

        if (clip == null) return;

        audioSource.pitch = 1f; // las auras suenan siempre igual
        audioSource.PlayOneShot(clip, volumenAura);
    }

    private void ReproducirAleatorio(AudioClip[] clips, float volumen, ref int ultimoIndice)
    {
        if (clips == null || clips.Length == 0) return;

        int indice = Random.Range(0, clips.Length);

        // Evita repetir el mismo clip dos veces seguidas
        if (clips.Length > 1 && indice == ultimoIndice)
            indice = (indice + 1 + Random.Range(0, clips.Length - 1)) % clips.Length;

        ultimoIndice = indice;

        audioSource.pitch = Random.Range(rangoPitch.x, rangoPitch.y);
        audioSource.PlayOneShot(clips[indice], volumen);
    }
}