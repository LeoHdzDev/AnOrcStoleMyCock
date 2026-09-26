using UnityEngine;

// Este script evita que los enemigos se superpongan entre ellos.
// Funciona sin importar cómo se mueva cada uno (con transform.position
// directo, como Comportamiento_espiritus.cs o ComportamientoSlime.cs,
// o con Rigidbody2D, como DuendeCuchilloController.cs), porque se
// ejecuta al final del frame y simplemente los empuja si están
// demasiado cerca unos de otros.
//
// USO:
// 1) Añade este componente a CADA enemigo (o a su prefab).
// 2) En el campo "Capa Enemigos" del Inspector, selecciona la capa
//    (Layer) en la que están TODOS tus enemigos.
public class SeparacionEnemigos : MonoBehaviour
{
    [Header("Separación entre enemigos")]
    [Tooltip("Distancia mínima que debe haber entre el centro de este enemigo y el de otro.")]
    public float radioSeparacion = 0.7f;

    [Tooltip("Qué tan fuerte se empujan entre sí cuando se superponen.")]
    public float fuerzaSeparacion = 4f;

    [Tooltip("Capa (Layer) en la que están TODOS los enemigos.")]
    public LayerMask capaEnemigos;

    void LateUpdate()
    {
        Vector2 posicionActual = transform.position;
        Vector2 empujeTotal = Vector2.zero;

        Collider2D[] vecinos = Physics2D.OverlapCircleAll(posicionActual, radioSeparacion, capaEnemigos);

        foreach (Collider2D otro in vecinos)
        {
            if (otro.gameObject == gameObject) continue; // ignorarse a sí mismo

            Vector2 posicionOtro = otro.transform.position;
            Vector2 direccion = posicionActual - posicionOtro;
            float distancia = direccion.magnitude;

            // Si están literalmente en el mismo punto, elegimos una
            // dirección aleatoria para que no se queden "trabados".
            if (distancia < 0.001f)
            {
                direccion = UnityEngine.Random.insideUnitCircle.normalized;
                distancia = 0.001f;
            }

            // Cuanto más cerca estén, más fuerte es el empujón.
            float intensidad = Mathf.Clamp01((radioSeparacion - distancia) / radioSeparacion);
            empujeTotal += direccion.normalized * intensidad;
        }

        if (empujeTotal != Vector2.zero)
        {
            transform.position += (Vector3)(empujeTotal * fuerzaSeparacion * Time.deltaTime);
        }
    }

    // Para ver el radio de separación en la escena mientras seleccionas al enemigo.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radioSeparacion);
    }
}