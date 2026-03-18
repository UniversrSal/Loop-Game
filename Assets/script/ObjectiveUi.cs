using UnityEngine;
using TMPro;

public class ObjectiveUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI objectiveText;

    void Awake()
    {
        // Make sure it starts hidden
        objectiveText.gameObject.SetActive(false);
    }

    public void ShowObjective(string text)
    {
        // Set the text
        objectiveText.text = "Objective: " + text;

        // Make it visible immediately
        objectiveText.gameObject.SetActive(true);
    }
}
