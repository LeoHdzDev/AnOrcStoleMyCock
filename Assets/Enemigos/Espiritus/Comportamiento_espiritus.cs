using UnityEngine;

public class Comportamiento_espiritus : MonoBehaviour // 1. Cambiamos el nombre de la clase
{
    [Header("Movimiento")]
    public float velocidad = 2.5f;
    public float distanciaFrenado = 4f;

    [Header("Ataque")]
    public GameObject prefabProyectil; // 2. Renombramos la variable para que sea general
    public float tiempoEntreDisparos = 2f;
    private float proximoDisparo = 0f;

    private Transform jugador;
    private Animator animator; 

    void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>(); 
    }

    void Update()
    {
        if (jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        // Moverse hacia el jugador si está lejos
        if (distancia > distanciaFrenado)
        {
            transform.position = Vector2.MoveTowards(transform.position, jugador.position, velocidad * Time.deltaTime);
            animator.SetBool("Caminando", true); 
        }
        else
        {
            animator.SetBool("Caminando", false); 
        }

        // Disparar
        if (Time.time >= proximoDisparo)
        {
            Atacar();
            proximoDisparo = Time.time + tiempoEntreDisparos;
        }
    }

    void Atacar()
    {
        animator.SetTrigger("Atacar"); 
        // 3. Usamos la nueva variable general al disparar
        Instantiate(prefabProyectil, transform.position, Quaternion.identity); 
    }
}