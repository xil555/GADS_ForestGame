using UnityEngine;

public class AxePickup : MonoBehaviour
{
    public GameObject axeOnPlayer;

    bool inRange;

    void Update()
    {
        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            axeOnPlayer.SetActive(true);

            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            inRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            inRange = false;
    }
}