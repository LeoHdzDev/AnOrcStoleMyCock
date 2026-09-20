using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Personaje a seguir")]
    public Transform objetivo;

    [Header("Configuración")]
    public float velocidadSuavizado = 0.125f;
    // El -10 en Z es vital en 2D para que la cámara no se meta dentro del personaje
    public Vector3 compensacion = new Vector3(0f, 0f, -10f); 

    void LateUpdate()
    {
        if (objetivo != null)
        {
            // Calcula la posición a la que la cámara debe ir
            Vector3 posicionDeseada = objetivo.position + compensacion;
            
            // Mueve la cámara suavemente hacia esa posición
            transform.position = Vector3.Lerp(transform.position, posicionDeseada, velocidadSuavizado);
        }
    }
}