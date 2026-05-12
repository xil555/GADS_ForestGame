using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MushroomCollect : MonoBehaviour
{
    bool playerNear;
    bool collectedThisCycle;

    void Reset()
    {
        var c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    public void ResetForNewDay()
    {
        collectedThisCycle = false;
        gameObject.SetActive(true);
        enabled = true;
    }

    void Update()
    {
        if (!playerNear || collectedThisCycle)
            return;

        if (ObjectiveManager.Instance == null)
            return;

        if (!ObjectiveManager.Instance.HasObjective(ObjectiveType.Mushroom))
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (ObjectiveManager.Instance.TryCollectMushroom())
            {
                collectedThisCycle = true;
                gameObject.SetActive(false);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNear = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerNear = false;
    }
}
