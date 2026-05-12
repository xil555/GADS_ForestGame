using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FireInteract : MonoBehaviour
{
    [SerializeField] GameObject fireEffect;

    bool playerNear;
    float holdTime;
    bool completedThisCycle;

    void Reset()
    {
        var c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    public void ResetForNewDay()
    {
        completedThisCycle = false;
        holdTime = 0f;
        enabled = true;
        if (fireEffect != null)
            fireEffect.SetActive(false);
    }

    void Update()
    {
        if (!playerNear || completedThisCycle)
            return;

        if (ObjectiveManager.Instance == null)
            return;

        if (!ObjectiveManager.Instance.HasObjective(ObjectiveType.Fire))
        {
            holdTime = 0f;
            return;
        }

        if (Input.GetKey(KeyCode.E))
        {
            holdTime += Time.deltaTime;

            if (holdTime >= 5f)
            {
                ObjectiveManager.Instance.CompleteFire();
                if (fireEffect != null)
                    fireEffect.SetActive(true);

                completedThisCycle = true;
                enabled = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.E))
            holdTime = 0f;
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
        holdTime = 0f;
    }
}
