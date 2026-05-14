using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WaterCollect : MonoBehaviour
{
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
    }

    void Update()
    {
        if (!playerNear || completedThisCycle)
            return;

        if (ObjectiveManager.Instance == null)
            return;

        if (!ObjectiveManager.Instance.HasObjective(ObjectiveType.Water))
        {
            holdTime = 0f;
            return;
        }

        if (Input.GetKey(KeyCode.F))
        {
            holdTime += Time.deltaTime;

            if (holdTime >= 5f)
            {
                ObjectiveManager.Instance.CompleteWater();
                completedThisCycle = true;
                enabled = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.F))
            holdTime = 0f;
    }

    void LateUpdate()
    {
        if (!playerNear || completedThisCycle)
            return;

        if (ObjectiveManager.Instance == null || !ObjectiveManager.Instance.HasObjective(ObjectiveType.Water))
            return;

        PromptManager.SubmitPromptCandidate("Hold F to collect water (5 s)", PromptManager.PrioritySurvivalHold);
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
