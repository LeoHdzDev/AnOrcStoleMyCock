using UnityEngine;

// Se agrega a CUALQUIER enemigo. No necesita que lo llames desde otro
// script: se activa solo, automáticamente, en el momento exacto en que
// Unity destruye al enemigo (sin importar cuál de tus 3 scripts de
// enemigo haya sido el que llamó a Destroy(gameObject)).
public class DropDeItems : MonoBehaviour
{
    [Header("Ítem que puede soltar")]
    [SerializeField] private GameObject prefabItemMaiz;

    [Header("Probabilidad de soltarlo")]
    [Tooltip("0.2 = 1 de cada 5 veces (20%).")]
    [Range(0f, 1f)]
    [SerializeField] private float probabilidadDrop = 0.2f;

    private bool saliendoDeLaAplicacion = false;

    void OnApplicationQuit()
    {
        saliendoDeLaAplicacion = true;
    }

    void OnDestroy()
    {
        // Evita que "suelte" ítems al cerrar el juego o al cambiar de escena,
        // ya que ahí también se destruyen los enemigos pero no por haber muerto.
        if (saliendoDeLaAplicacion) return;
        if (!gameObject.scene.isLoaded) return;

        if (prefabItemMaiz == null) return;

        if (Random.value <= probabilidadDrop)
        {
            Instantiate(prefabItemMaiz, transform.position, Quaternion.identity);
        }
    }
}
