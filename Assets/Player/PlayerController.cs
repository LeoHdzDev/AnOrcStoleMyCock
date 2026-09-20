using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Estadísticas")]
    public float velocidad = 5f;
    public int danoAtaque = 5;

    [Header("Combate y Autoapuntado")]
    public float radioAutoApuntado = 3f;
    public LayerMask capaEnemigos; // Capa para que el radar detecte a quién pegarle

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movimiento;
    private bool estaAtacando = false;
    private bool estaEmpujado = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Si está atacando, ignoramos el movimiento para que se quede quieto
        if (estaAtacando) return; 

        // Movimiento base
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

        // Disparar ataque con Clic Izquierdo
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(RutinaAtaque());
        }
    }

    void FixedUpdate()
    {
        if (!estaAtacando && !estaEmpujado) // Modifica esta línea
        {
            rb.MovePosition(rb.position + movimiento.normalized * velocidad * Time.fixedDeltaTime);
        }
    }

    IEnumerator RutinaAtaque()
    {
        estaAtacando = true;
        rb.linearVelocity = Vector2.zero; // Frenamos al granjero de golpe

        AutoApuntar();

        animator.SetTrigger("Attack");

        // Pausa para que termine la animación (ajusta los 0.4s según dure tu animación real)
        yield return new WaitForSeconds(0.4f); 

        estaAtacando = false;
    }

    void AutoApuntar()
    {
        // Creamos un círculo invisible que detecta todo lo que esté en la 'capaEnemigos'
        Collider2D[] enemigos = Physics2D.OverlapCircleAll(transform.position, radioAutoApuntado, capaEnemigos);
        Debug.Log("Enemigos detectados en el radar: " + enemigos.Length);
        if (enemigos.Length > 0)
        {
            Transform enemigoMasCercano = null;
            float distanciaMinima = Mathf.Infinity;

            // Buscamos cuál es el más cercano
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
                // Voltear automáticamente hacia el enemigo
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

    // Dibuja el círculo rojo en la escena para que veas hasta dónde llega tu autoapuntado
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioAutoApuntado);
    }

    public IEnumerator RecibirEmpuje(Vector2 direccion, float fuerza, float duracion)
    {
        estaEmpujado = true;
        
        // Aplicar la fuerza física de golpe
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direccion * fuerza, ForceMode2D.Impulse);

        // Esperar a que pase el tiempo de empuje
        yield return new WaitForSeconds(duracion);

        rb.linearVelocity = Vector2.zero; // Frenar al granjero
        estaEmpujado = false; // Devolverle el control
    }
} // <-- Esta llave cierra la clase PlayerController