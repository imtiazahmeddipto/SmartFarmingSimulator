using UnityEngine;

public class House : MonoBehaviour
{
    public GameObject Indicator;
    public GameObject homeCanvas;

    private bool playerInside = false;

    private void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            homeCanvas.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInside = true;
            Indicator.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInside = false;
            Indicator.SetActive(false);
        }
    }
}
