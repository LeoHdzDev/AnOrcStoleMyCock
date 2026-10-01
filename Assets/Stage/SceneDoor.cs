using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class SceneDoor : MonoBehaviour
{
    [Header("Configuración de Escena")]
    [SerializeField] private string sceneToLoad = "Interaccion_jefe_orco"; // Nombre exacto de la escena

    [Header("Configuración de Tecla")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool isPlayerInside = false;

    private void Update()
    {
        if (isPlayerInside && Input.GetKeyDown(interactKey))
        {
            ChangeScene();
        }
    }

    private void ChangeScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log($"Cargando escena: {sceneToLoad}");
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("¡El nombre de la escena está vacío en el Inspector!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }
}