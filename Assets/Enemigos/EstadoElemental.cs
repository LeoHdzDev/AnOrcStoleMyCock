using System.Collections;
using UnityEngine;

// Componente COMPARTIDO para TODOS los enemigos.
// Se encarga de la quemadura (fuego) y el congelamiento/ralentización (hielo).
public class EstadoElemental : MonoBehaviour
{
    [Header("Colores mientras dura el efecto")]
    public Color colorQuemado = new Color(1f, 0.35f, 0.35f);
    public Color colorCongelado = new Color(0.75f, 0.95f, 1f);

    // --- Referencias a los posibles scripts de comportamiento ---
    private Comportamiento_espiritus espiritu;
    private ComportamientoSlime slime;
    private DuendeCuchilloController duende;

    // --- Referencias a los scripts de Salud ---
    private SaludEnemigo saludEspiritu;
    private SaludDuendeCuchillo saludDuende;
    private SaludDuendeBomba saludBomba; // <-- NUEVO
    private SaludJefeOrco saludJefe;     // <-- NUEVO

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

        // Comportamientos
        espiritu = GetComponent<Comportamiento_espiritus>();
        slime = GetComponent<ComportamientoSlime>();
        duende = GetComponent<DuendeCuchilloController>();

        // Salud
        saludEspiritu = GetComponent<SaludEnemigo>();
        saludDuende = GetComponent<SaludDuendeCuchillo>();
        saludBomba = GetComponent<SaludDuendeBomba>(); // <-- Conectamos el Duende Bomba
        saludJefe = GetComponent<SaludJefeOrco>();     // <-- Conectamos al Jefe Orco
    }

    void LateUpdate()
    {
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
        // Nota: Si en el futuro quieres que el hielo también ponga lentos al Duende Bomba 
        // y al Jefe, tendríamos que agregar sus scripts de comportamiento aquí.
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

    public void RecibirGolpe(float cantidad)
    {
        AplicarDano(cantidad);
    }

    void AplicarDano(float cantidad)
    {
        if (saludEspiritu != null) { saludEspiritu.RecibirDano(cantidad); return; }
        if (slime != null) { slime.RecibirDano(cantidad); return; }
        if (saludDuende != null) { saludDuende.RecibirDano(cantidad); return; }
        
        // --- AQUÍ ESTÁ LA MAGIA PARA LOS NUEVOS ENEMIGOS ---
        if (saludBomba != null) { saludBomba.RecibirDano(cantidad); return; }
        if (saludJefe != null) { saludJefe.RecibirDano(cantidad); return; }
    }
}