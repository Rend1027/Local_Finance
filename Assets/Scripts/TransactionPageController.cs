using UnityEngine;
using Firebase.Firestore;
using Firebase.Auth;
using TMPro;

public class TransactionPageController : MonoBehaviour
{
    public Transform transactionsContent;
    public GameObject transactionItemPrefab;

    FirebaseFirestore db;
    FirebaseAuth auth;

    private void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;

        LoadTransactions();
    }

    public void RefreshPage()
    {
        LoadTransactions();
    }

    // --------------------------------------------------------------------
    // 🔵 DELETE CALLBACK
    // --------------------------------------------------------------------
    private async void OnDeletePressed(string documentId)
    {
        var user = auth.CurrentUser;
        if (user == null) return;

        await db.Collection("users")
                .Document(user.UserId)
                .Collection("transactions")
                .Document(documentId)
                .DeleteAsync();

        Debug.Log("🔥 Deleted transaction: " + documentId);

        LoadTransactions();   // refresh page immediately
    }

    // --------------------------------------------------------------------
    // 🔵 EDIT CALLBACK
    // --------------------------------------------------------------------
    private void OnEditPressed(string documentId)
    {
        Debug.Log("✏️ Edit pressed for: " + documentId);

        var form = UnityEngine.Object.FindFirstObjectByType<TransactionFormController>();
        form.OpenFormForEdit(documentId);
    }

    // --------------------------------------------------------------------
    // 🔵 LOAD ALL TRANSACTIONS
    // --------------------------------------------------------------------
    async void LoadTransactions()
    {
        var user = auth.CurrentUser;
        if (user == null)
        {
            Debug.LogError("❌ Cannot load transactions: No logged-in user.");
            return;
        }

        Debug.Log("📥 Loading transactions from Firestore...");

        // Clear old items
        foreach (Transform child in transactionsContent)
            Destroy(child.gameObject);

        QuerySnapshot snapshot = await db
            .Collection("users")
            .Document(user.UserId)
            .Collection("transactions")
            .OrderByDescending("Timestamp")
            .GetSnapshotAsync();

        foreach (DocumentSnapshot doc in snapshot.Documents)
        {
            var tx = doc.ConvertTo<TransactionModel>();

            GameObject item = Instantiate(transactionItemPrefab, transactionsContent);

            // Fill UI ----------------------------------------------------------------
            item.transform.Find("LeftBlock/TextBlock/Title")
                .GetComponent<TMP_Text>().text = tx.Title;

            item.transform.Find("LeftBlock/TextBlock/Description")
                .GetComponent<TMP_Text>().text = tx.Description;

            item.transform.Find("RightBlock/AmountContainer/Amount")
                .GetComponent<TMP_Text>().text =
                (tx.IsIncome ? "+" : "-") + tx.Amount.ToString("N2");

            item.transform.Find("LeftBlock/StatusDot")
                .GetComponent<UnityEngine.UI.Image>().color =
                tx.IsIncome ? Color.green : Color.red;

            // ------------------------------------------------------------------------
            // 🔵 Attach callbacks + Firestore ID to UI element
            // ------------------------------------------------------------------------
            TransactionItemUI itemUI = item.GetComponent<TransactionItemUI>();
            itemUI.Init(doc.Id, OnEditPressed, OnDeletePressed);
        }

        Debug.Log("✅ Transactions loaded: " + snapshot.Count);
    }
}
