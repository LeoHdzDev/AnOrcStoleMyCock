using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    [Header("Ataque")]
    [SerializeField] private AudioClip[] clipsAtaque;
    [SerializeField, Range(0f, 1f)] private float volumenAtaque = 1f;

    [Header("Variación de pitch")]
    [SerializeField] private Vector2 rangoPitch = new Vector2(0.95f, 1.05f);

    private AudioSource audioSource;
    private int ultimoAtaque = -1;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void PlayAttack()
    {
        if (clipsAtaque == null || clipsAtaque.Length == 0) return;

        int indice = Random.Range(0, clipsAtaque.Length);

        // Evita que el mismo clip suene dos veces seguidas
        if (clipsAtaque.Length > 1 && indice == ultimoAtaque)
            indice = (indice + 1 + Random.Range(0, clipsAtaque.Length - 1)) % clipsAtaque.Length;

        ultimoAtaque = indice;

        audioSource.pitch = Random.Range(rangoPitch.x, rangoPitch.y);
        audioSource.PlayOneShot(clipsAtaque[indice], volumenAtaque);
    }
}