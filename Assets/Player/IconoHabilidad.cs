using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Controla UN ícono de habilidad en el HUD: mostrar/ocultar, el efecto
// de "pulso" al usarse, y el círculo/reloj de cooldown alrededor del ícono.
//
// ESTRUCTURA REQUERIDA (dentro del Canvas):
//
// IconoX                      <- este script va aquí
//   ├─ Icono   (Image)        <- arrástralo al campo "Img Icono"
//   └─ Cooldown (Image)       <- arrástralo al campo "Img Cooldown"
//        Image Type = Filled
//        Fill Method = Radial 360
//        Fill Origin = Top
//        Clockwise = activado (tilde puesto)
//
// El objeto "Cooldown" debe estar ENCIMA del "Icono" en la jerarquía
// (como hijo después) para que se dibuje por delante.
public class IconoHabilidad : MonoBehaviour
{
    [Header("Referencias (arrastra los hijos aquí)")]
    [SerializeField] private Image imgIcono;
    [SerializeField] private Image imgCooldown;

    [Header("¿Este ícono se ve desde el inicio del juego?")]
    [SerializeField] private bool visibleDesdeElInicio = false;

    [Header("Efecto de pulso")]
    [SerializeField] private float escalaPulso = 1.15f;
    [SerializeField] private float duracionPulso = 0.15f;

    [Header("Colores del ícono")]
    [SerializeField] private Color colorNormal = Color.white;
    [SerializeField] private Color colorEnCooldown = new Color(0.6f, 0.6f, 0.6f, 1f);

    private Vector3 escalaOriginal;
    private Coroutine pulsoCoroutine;
    private Coroutine cooldownCoroutine;

    void Awake()
    {
        escalaOriginal = transform.localScale;

        if (imgCooldown != null)
        {
            imgCooldown.fillAmount = 0f;
            imgCooldown.gameObject.SetActive(false);
        }

        gameObject.SetActive(visibleDesdeElInicio);
    }

    public void Mostrar()
    {
        gameObject.SetActive(true);
    }

    public void Ocultar()
    {
        gameObject.SetActive(false);
    }

    // Llama esto cada vez que se use la habilidad: hace el pulso y,
    // si duracionCooldown es mayor a 0, arranca el círculo de cooldown.
    public void Activar(float duracionCooldown)
    {
        if (pulsoCoroutine != null) StopCoroutine(pulsoCoroutine);
        pulsoCoroutine = StartCoroutine(RutinaPulso());

        if (duracionCooldown > 0f && imgCooldown != null)
        {
            if (cooldownCoroutine != null) StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = StartCoroutine(RutinaCooldown(duracionCooldown));
        }
    }

    IEnumerator RutinaPulso()
    {
        float t = 0f;
        Vector3 escalaGrande = escalaOriginal * escalaPulso;

        while (t < duracionPulso)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(escalaOriginal, escalaGrande, t / duracionPulso);
            yield return null;
        }

        t = 0f;
        while (t < duracionPulso)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(escalaGrande, escalaOriginal, t / duracionPulso);
            yield return null;
        }

        transform.localScale = escalaOriginal;
    }

    IEnumerator RutinaCooldown(float duracionCooldown)
    {
        imgCooldown.gameObject.SetActive(true);
        imgCooldown.fillAmount = 0f;
        if (imgIcono != null) imgIcono.color = colorEnCooldown;

        float t = 0f;
        while (t < duracionCooldown)
        {
            t += Time.deltaTime;
            imgCooldown.fillAmount = Mathf.Clamp01(t / duracionCooldown);
            yield return null;
        }

        imgCooldown.fillAmount = 1f;
        imgCooldown.gameObject.SetActive(false);
        if (imgIcono != null) imgIcono.color = colorNormal;
    }
}
