using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum ElementType
    {
        None,
        Water,
        Fire,
        Ice
    }

    [SerializeField] private ElementType elementoActual = ElementType.None;

    [SerializeField] private RuntimeAnimatorController controladorNormal;
    [SerializeField] private RuntimeAnimatorController controladorAgua;
    [SerializeField] private RuntimeAnimatorController controladorFuego;

    public void SetElemento(ElementType nuevoElemento)
    {
        elementoActual = nuevoElemento;
    }

    public void ActualizarVisualElemento()
    {
        if (animator == null)
            return;

        switch (elementoActual)
        {
            case ElementType.Water:
                animator.runtimeAnimatorController = controladorAgua;
                break;

            case ElementType.Fire:
                animator.runtimeAnimatorController = controladorFuego;
                break;

            case ElementType.None:
            case ElementType.Ice:
                animator.runtimeAnimatorController = controladorNormal;
                break;
        }
    }

    [Header("Estadísticas de Movimiento y Ataque")]
    public float velocidad = 5f;
    public int danoAtaque = 5;

    [Header("Combate y Autoapuntado")]
    public float radioAutoApuntado = 3f;
    public LayerMask capaEnemigos; 

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movimiento;
    private bool estaAtacando = false;
    private bool estaEmpujado = false;
    private bool estaRalentizado = false;
    private bool estaRecogiendoItem = false;
    [SerializeField] private float duracionBloqueoRecogida = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (estaAtacando || estaRecogiendoItem)
        {
            movimiento = Vector2.zero;
            animator.SetFloat("Speed", 0f);
            return;
        }

        movimiento.x = Input.GetAxisRaw("Horizontal");
        movimiento.y = Input.GetAxisRaw("Vertical");

        if (movimiento != Vector2.zero)
        {
            animator.SetFloat("Horizontal", movimiento.x);
            animator.SetFloat("Vertical", movimiento.y);
            animator.SetFloat("LastHorizontal", movimiento.x);
            animator.SetFloat("LastVertical", movimiento.y);
        }

        animator.SetFloat("Speed", movimiento.sqrMagnitude);

        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(RutinaAtaque());
        }
    }

    void FixedUpdate()
    {
        if (!estaAtacando && !estaEmpujado && !estaRecogiendoItem)
        {
            rb.MovePosition(
                rb.position + movimiento.normalized * velocidad * Time.fixedDeltaTime
            );
        }
    }

    public void IniciarRecogida()
    {
        estaRecogiendoItem = true;
        movimiento = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        animator.SetFloat("Speed", 0f);

        StartCoroutine(DesbloquearDespuesDeRecoger());
    }

    private IEnumerator DesbloquearDespuesDeRecoger()
    {
        yield return new WaitForSeconds(duracionBloqueoRecogida);
        estaRecogiendoItem = false;
    }

    public void TerminarRecogida()
    {
        estaRecogiendoItem = false;
    }

    IEnumerator RutinaAtaque()
    {
        estaAtacando = true;
        rb.linearVelocity = Vector2.zero; 

        AutoApuntar();

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.4f); 

        estaAtacando = false;
    }

    void AutoApuntar()
    {
        Collider2D[] enemigos = Physics2D.OverlapCircleAll(transform.position, radioAutoApuntado, capaEnemigos);
        
        if (enemigos.Length > 0)
        {
            Transform enemigoMasCercano = null;
            float distanciaMinima = Mathf.Infinity;

            foreach (Collider2D enemigo in enemigos)
            {
                float distancia = Vector2.Distance(transform.position, enemigo.transform.position);
                if (distancia < distanciaMinima)
                {
                    distanciaMinima = distancia;
                    enemigoMasCercano = enemigo.transform;
                }
            }

            if (enemigoMasCercano != null)
            {
                Vector2 direccion = (enemigoMasCercano.position - transform.position).normalized;
                
                if (Mathf.Abs(direccion.x) > Mathf.Abs(direccion.y))
                {
                    animator.SetFloat("LastHorizontal", Mathf.Sign(direccion.x));
                    animator.SetFloat("LastVertical", 0);
                }
                else
                {
                    animator.SetFloat("LastHorizontal", 0);
                    animator.SetFloat("LastVertical", Mathf.Sign(direccion.y));
                }

                // --- 1. Daño a los Espíritus ---
                SaludEnemigo saludEnemigo = enemigoMasCercano.GetComponent<SaludEnemigo>();
                if (saludEnemigo != null)
                {
                    saludEnemigo.RecibirDano(danoAtaque);
                }

                // --- 2. Daño al Slime ---
                ComportamientoSlime slime = enemigoMasCercano.GetComponent<ComportamientoSlime>();
                if (slime != null)
                {
                    slime.RecibirDano(danoAtaque);
                }

                // --- 3. Daño al Duende ---
                SaludDuendeCuchillo duende = enemigoMasCercano.GetComponent<SaludDuendeCuchillo>();
                if (duende != null)
                {
                    duende.RecibirDano(danoAtaque);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioAutoApuntado);
    }

    public IEnumerator RecibirEmpuje(Vector2 direccion, float fuerza, float duracion)
    {
        estaEmpujado = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direccion * fuerza, ForceMode2D.Impulse);
        yield return new WaitForSeconds(duracion);
        rb.linearVelocity = Vector2.zero; 
        estaEmpujado = false; 
    }

    public IEnumerator AplicarRalentizacion(float factorVelocidad, int duracion)
    {
        if (estaRalentizado) yield break; 

        estaRalentizado = true;
        
        float velocidadOriginal = velocidad;
        velocidad *= factorVelocidad; 

        SpriteRenderer spriteGranjero = GetComponent<SpriteRenderer>();
        if (spriteGranjero != null) spriteGranjero.color = Color.cyan;

        yield return new WaitForSeconds(duracion);

        velocidad = velocidadOriginal;
        if (spriteGranjero != null) spriteGranjero.color = Color.white;
        
        estaRalentizado = false;
    }
}