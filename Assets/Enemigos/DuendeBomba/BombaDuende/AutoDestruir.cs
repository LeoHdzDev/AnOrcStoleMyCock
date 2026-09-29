using UnityEngine;

public class AutoDestruir : MonoBehaviour
{
    public float tiempoDeVida = 0.5f; // Ajusta esto según lo que dure tu animación

    void Start()
    {
        // Destruye el objeto automáticamente después del tiempo indicado
        Destroy(gameObject, tiempoDeVida);
    }
}