using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Optional: add to the Daily Tasks panel root. Sets a dark semi-transparent background on a <see cref="Image"/>.
/// </summary>
[RequireComponent(typeof(Image))]
public class DailyTasksPanelBackground : MonoBehaviour
{
    [SerializeField] Color panelColor = new Color(0f, 0f, 0f, 0.65f);

    void Awake()
    {
        var img = GetComponent<Image>();
        img.color = panelColor;
        img.raycastTarget = false;
    }
}
