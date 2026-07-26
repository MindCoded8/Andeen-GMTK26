using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private string dialogueMessage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            dialogueUI.ShowDialogue(dialogueMessage);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            dialogueUI.HideDialogue();
        }
    }
}