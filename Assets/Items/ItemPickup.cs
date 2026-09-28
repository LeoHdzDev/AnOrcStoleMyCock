using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public enum TipoItem
    {
        Agua,
        Fuego,
        Hielo
    }

    [Header("Tipo de objeto")]
    [SerializeField] private TipoItem tipoItem = TipoItem.Agua;

    [Header("Atracción")]
    [SerializeField] private float velocidadAtraccion = 5f;
    [SerializeField] private float distanciaMinima = 0.1f;

    private Transform farmer;
    private Animator farmerAnimator;
    private PlayerController farmerController;
    private bool atrayendo = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            farmer = other.transform;
            farmerAnimator = farmer.GetComponent<Animator>();
            farmerController = farmer.GetComponent<PlayerController>();

            atrayendo = true;
        }
    }

    private void Update()
    {
        if (!atrayendo || farmer == null)
            return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            farmer.position,
            velocidadAtraccion * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, farmer.position) <= distanciaMinima)
        {
            RecogerItem();
        }
    }

    private void RecogerItem()
    {
        atrayendo = false;

        if (farmerController != null)
        {
            farmerController.IniciarRecogida();

            switch (tipoItem)
            {
                case TipoItem.Agua:
                    farmerController.SetElemento(PlayerController.ElementType.Water);
                    farmerController.ActualizarVisualElemento();
                    break;

                case TipoItem.Fuego:
                    farmerController.SetElemento(PlayerController.ElementType.Fire);
                    farmerController.ActualizarVisualElemento();
                    break;

                case TipoItem.Hielo:
                    farmerController.SetElemento(PlayerController.ElementType.Ice);
                    farmerController.ActualizarVisualElemento();
                    break;
            }
        }

        if (farmerAnimator != null)
        {
            switch (tipoItem)
            {
                case TipoItem.Agua:
                    farmerAnimator.SetTrigger("PickupWater");
                    break;

                case TipoItem.Fuego:
                    farmerAnimator.SetTrigger("PickupFire");
                    break;

                case TipoItem.Hielo:
                    farmerAnimator.SetTrigger("PickupIce");
                    break;
            }
        }

        Destroy(gameObject);
    }
}