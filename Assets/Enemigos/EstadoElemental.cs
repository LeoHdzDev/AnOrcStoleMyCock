using System.Collections;
using UnityEngine;

// Componente COMPARTIDO para TODOS los enemigos (Espiritus, Slime, DuendeCuchillo).
// Se encarga de la quemadura (fuego) y el congelamiento/ralentización (hielo),
// incluyendo el color del sprite, SIN modificar los scripts de comportamiento
// de cada enemigo.
//
// USO: agrega este componente a cada GameObject enemigo (o a sus prefabs).
// No necesita configuración: detecta solo qué scripts tiene el enemigo.
public class EstadoElemental : MonoBehaviour
{
    [Header("Colores mientras dura el efecto")]
    public Color colorQuemado = new Color(1f, 0.35f, 0.35f);
    public Color colorCongelado = new Color(0.75f, 0.95f, 1f);

    // --- Referencias a los posibles scripts de comportamiento/salud del enemigo ---
    private Comportamiento_espiritus espiritu;
    private ComportamientoSlime slime;
    private DuendeCuchilloController duende;

    private SaludEnemigo saludEspiritu;
    private SaludDuendeCuchillo saludDuende;
    // Nota: ComportamientoSlime ya trae su propio RecibirDano(), no hace falta otro componente de salud.

    private SpriteRenderer sr;

    // --- Estado de Quemadura ---
    private bool estaQuemado = false;
    private float tiempoFinQuemadura = 0f;

    // --- Estado de Congelamiento ---
    private bool estaCongelado = false;
    private float tiempoFinCongelamiento = 0f;
    private float velocidadOriginal;
    private float danoOriginal;

    // --- Color base (antes de cualquier efecto) ---
    private Color colorBase = Color.white;
    private bool colorBaseGuardado = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        espiritu = GetComponent<Comportamiento_espiritus>();
        slime = GetComponent<ComportamientoSlime>();
        duende = GetComponent<DuendeCuchilloController>();

        saludEspiritu = GetComponent<SaludEnemigo>();
        saludDuende = GetComponent<SaludDuendeCuchillo>();
    }

    void LateUpdate()
    {
        // Se ejecuta al final del frame para "ganarle" a cualquier otro script
        // que también cambie el color del sprite (como el flash blanco al recibir daño).
        if (sr == null) return;

        if (estaQuemado)
        {
            sr.color = colorQuemado;
        }
        else if (estaCongelado)
        {
            sr.color = colorCongelado;
        }
        else if (colorBaseGuardado)
        {
            sr.color = colorBase;
            colorBaseGuardado = false;
        }
    }

    void AsegurarColorBase()
    {
        if (!colorBaseGuardado && sr != null)
        {
            colorBase = sr.color;
            colorBaseGuardado = true;
        }
    }

    // ================= QUEMADURA (FUEGO) =================

    // danoPorTick: cuánto daño se aplica cada "intervaloTick" segundos.
    // duracionSinContacto: cuánto sigue quemándose después de salir del área.
    public void AplicarQuemadura(float danoPorTick, float intervaloTick, float duracionSinContacto)
    {
        tiempoFinQuemadura = Time.time + duracionSinContacto;

        if (!estaQuemado)
        {
            estaQuemado = true;
            AsegurarColorBase();
            StartCoroutine(RutinaQuemadura(danoPorTick, intervaloTick));
        }
    }

    IEnumerator RutinaQuemadura(float danoPorTick, float intervaloTick)
    {
        while (Time.time < tiempoFinQuemadura)
        {
            AplicarDano(danoPorTick);
            yield return new WaitForSeconds(intervaloTick);
        }

        estaQuemado = false;
    }

    // ================= CONGELAMIENTO (HIELO) =================

    public void AplicarCongelamiento(float factorVelocidad, float factorDano, float duracion)
    {
        tiempoFinCongelamiento = Time.time + duracion;

        if (!estaCongelado)
        {
            estaCongelado = true;
            AsegurarColorBase();
            GuardarYAplicarFactores(factorVelocidad, factorDano);
            StartCoroutine(RutinaCongelamiento());
        }
    }

    IEnumerator RutinaCongelamiento()
    {
        while (Time.time < tiempoFinCongelamiento)
        {
            yield return null;
        }

        RestaurarValoresOriginales();
        estaCongelado = false;
    }

    void GuardarYAplicarFactores(float factorVelocidad, float factorDano)
    {
        if (espiritu != null)
        {
            velocidadOriginal = espiritu.velocidad;
            espiritu.velocidad *= factorVelocidad;
            // Los espíritus atacan con proyectiles a distancia; su daño no se
            // controla desde aquí. Si luego quieres reducir el daño de sus
            // proyectiles también, se puede ajustar en su script de disparo.
        }
        else if (slime != null)
        {
            velocidadOriginal = slime.velocidad;
            danoOriginal = slime.danoAtaque;
            slime.velocidad *= factorVelocidad;
            slime.danoAtaque *= factorDano;
        }
        else if (duende != null)
        {
            velocidadOriginal = duende.velocidad;
            danoOriginal = duende.danoAtaque;
            duende.velocidad *= factorVelocidad;
            duende.danoAtaque *= factorDano;
        }
    }

    void RestaurarValoresOriginales()
    {
        if (espiritu != null)
        {
            espiritu.velocidad = velocidadOriginal;
        }
        else if (slime != null)
        {
            slime.velocidad = velocidadOriginal;
            slime.danoAtaque = danoOriginal;
        }
        else if (duende != null)
        {
            duende.velocidad = velocidadOriginal;
            duende.danoAtaque = danoOriginal;
        }
    }

    // ================= DAÑO (compartido) =================

    // Para que un área (como AreaHielo) pueda hacer daño directo,
    // sin pasar por quemadura ni congelamiento.
    public void RecibirGolpe(float cantidad)
    {
        AplicarDano(cantidad);
    }

    void AplicarDano(float cantidad)
    {
        if (saludEspiritu != null) { saludEspiritu.RecibirDano(cantidad); return; }
        if (slime != null) { slime.RecibirDano(cantidad); return; }
        if (saludDuende != null) { saludDuende.RecibirDano(cantidad); return; }
    }
}