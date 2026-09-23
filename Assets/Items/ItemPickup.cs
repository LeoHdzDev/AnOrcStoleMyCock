using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Atracción")]
    [SerializeField] private float velocidadAtraccion = 5f;
    [SerializeField] private float distanciaMinima = 0.1f;

    private Transform farmer;
    private Animator farmerAnimator;
    private bool atrayendo = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            farmer = other.transform;
            farmerAnimator = farmer.GetComponent<Animator>();

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

        if (farmerAnimator != null)
        {
            farmerAnimator.SetTrigger("PickupWater");
        }

        Destroy(gameObject);
    }
}