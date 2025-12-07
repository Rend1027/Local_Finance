using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TransactionItemUI : MonoBehaviour
{
    public string DocumentId;  // <-- Firestore document ID

    public Button EditButton;
    public Button DeleteButton;

    public void Init(string docId, System.Action<string> onEdit, System.Action<string> onDelete)
    {
        DocumentId = docId;

        EditButton.onClick.RemoveAllListeners();
        EditButton.onClick.AddListener(() => onEdit(DocumentId));

        DeleteButton.onClick.RemoveAllListeners();
        DeleteButton.onClick.AddListener(() => onDelete(DocumentId));
    }
}
