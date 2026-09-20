using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (estaAtacando) return; 

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
        if (!estaAtacando && !estaEmpujado) 
        {
            rb.MovePosition(rb.position + movimiento.normalized * velocidad * Time.fixedDeltaTime);
        }
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

                SaludEnemigo saludEnemigo = enemigoMasCercano.GetComponent<SaludEnemigo>();
                if (saludEnemigo != null)
                {
                    saludEnemigo.RecibirDano(danoAtaque);
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
    // --- EFECTO DE HIELO (RALENTIZACIÓN) ---
    public IEnumerator AplicarRalentizacion(float factorVelocidad, int duracion)
    {
        // Si ya está congelado, ignoramos el nuevo golpe para que no se acumule
        if (estaRalentizado) yield break; 

        estaRalentizado = true;
        
        // 1. Guardamos su velocidad original y lo hacemos más lento
        float velocidadOriginal = velocidad;
        velocidad *= factorVelocidad; 

        // 2. Lo pintamos de azul hielo (Cyan)
        SpriteRenderer spriteGranjero = GetComponent<SpriteRenderer>();
        if (spriteGranjero != null) spriteGranjero.color = Color.cyan;

        // 3. Esperamos los segundos de congelamiento
        yield return new WaitForSeconds(duracion);

        // 4. Restauramos su velocidad normal y su color
        velocidad = velocidadOriginal;
        if (spriteGranjero != null) spriteGranjero.color = Color.white;
        
        estaRalentizado = false;
    }
}