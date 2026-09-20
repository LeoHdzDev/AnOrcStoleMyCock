using UnityEngine;

public class EspirituAgua : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 2.5f;
    public float distanciaFrenado = 4f;

    [Header("Ataque")]
    public GameObject prefabChorroAgua;
    public float tiempoEntreDisparos = 2f;
    private float proximoDisparo = 0f;

    private Transform jugador;
    private Animator animator; // 1. Agregamos la variable del Animator

    void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>(); // 2. Obtenemos el componente
    }

    void Update()
    {
        if (jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        // Moverse hacia el jugador si está lejos
        if (distancia > distanciaFrenado)
        {
            transform.position = Vector2.MoveTowards(transform.position, jugador.position, velocidad * Time.deltaTime);
            animator.SetBool("Caminando", true); // 3. Activamos animación de caminar
        }
        else
        {
            animator.SetBool("Caminando", false); // 4. Desactivamos animación de caminar
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
        animator.SetTrigger("Atacar"); // 5. Activamos el trigger del golpe
        Instantiate(prefabChorroAgua, transform.position, Quaternion.identity);
    }
}