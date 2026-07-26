using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    private void Awake()
    {
        dialoguePanel.SetActive(false);
    }

    public void ShowDialogue(string message)
    {
        dialogueText.text = message;
        dialoguePanel.SetActive(true);
    }

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
    }
}