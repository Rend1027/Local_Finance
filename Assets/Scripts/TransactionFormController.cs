using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Firestore;
using System;
using System.Threading.Tasks;

public class TransactionFormController : MonoBehaviour
{

    [Header("Modal")]
    public GameObject modalOverlay;
    public GameObject transactionFormPanel;

    [Header("Input Fields")]
    public TMP_Dropdown typeDropdown;
    public TMP_InputField dateField;
    public TMP_Dropdown categoryDropdown;
    public TMP_InputField amountField;
    public TMP_InputField descriptionField;
    public Toggle taxToggle;

    [Header("Transaction Page")]
    public Transform transactionsContent;
    public GameObject transactionItemPrefab;

    private GameObject itemBeingEdited = null;
    private string editingDocumentId = null;

    private TransactionPageController transactionPage;


    FirebaseFirestore db;
    FirebaseAuth auth;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;
        transactionPage = UnityEngine.Object.FindFirstObjectByType<TransactionPageController>();

    }

    public void OpenForm()
    {
        modalOverlay.SetActive(true);
        transactionFormPanel.SetActive(true);
    }

    public async void OpenFormForEdit(string documentId)
    {
        editingDocumentId = documentId;
        itemBeingEdited = null; // not needed but keeps logic clean

        var user = auth.CurrentUser;
        if (user == null) return;

        DocumentSnapshot doc = await db.Collection("users")
            .Document(user.UserId)
            .Collection("transactions")
            .Document(documentId)
            .GetSnapshotAsync();

        if (doc.Exists)
        {
            var tx = doc.ConvertTo<TransactionModel>();

            // Fill UI fields
            descriptionField.text = tx.Title;

            amountField.text = tx.Amount.ToString();

            typeDropdown.value = tx.IsIncome ? 0 : 1;
            categoryDropdown.value = 0; // optional, update if you store this field
            taxToggle.isOn = false;

            // Show modal
            modalOverlay.SetActive(true);
            transactionFormPanel.SetActive(true);
        }
    }


    public void CloseForm()
    {
        modalOverlay.SetActive(false);
        transactionFormPanel.SetActive(false);
        itemBeingEdited = null;
        editingDocumentId = null;
    }

    // ADD / SAVE TRANSACTION
    public async void AddTransaction()
    {
        Debug.Log("AddTransaction CALLED!");
        Debug.Log("transactionsContent = " + transactionsContent.name);

        if (string.IsNullOrWhiteSpace(amountField.text))
        {
            Debug.Log("Amount is required");
            return;
        }

        if (!float.TryParse(amountField.text, out float amount))
        {
            Debug.Log("Invalid amount");
            return;
        }

        string typeText = typeDropdown.options[typeDropdown.value].text;
        string categoryText = categoryDropdown.options[categoryDropdown.value].text;

        TransactionModel tx = new TransactionModel
        {
            Title = descriptionField.text,
            Description = $"{typeText} • {categoryText}",
            Amount = amount,
            IsIncome = typeText == "Income",
            Category = categoryText,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        var user = auth.CurrentUser;
        if (user == null)
        {
            Debug.LogError("User not logged in.");
            return;
        }

        DocumentReference txDoc;

        // UPDATE EXISTING TRANSACTION
        if (itemBeingEdited != null && editingDocumentId != null)
        {
            txDoc = db.Collection("users")
                      .Document(user.UserId)
                      .Collection("transactions")
                      .Document(editingDocumentId);

            await txDoc.SetAsync(tx);
            Debug.Log("Transaction updated!");
        }
        else
        {

            try
            {
                // ADD NEW TRANSACTION
                txDoc = await db.Collection("users")
                                .Document(user.UserId)
                                .Collection("transactions")
                                .AddAsync(tx);

                editingDocumentId = txDoc.Id;
                Debug.Log("🔥 Transaction ADDED to Firestore with ID: " + txDoc.Id);


            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            finally
            {

                // 🔄 Refresh Transaction Page
                if (transactionPage != null)
                {
                    transactionPage.RefreshPage();
                }
                Debug.Log("🔥 Closing form...");
                CloseForm();

            }

        }

    }


    // DELETE TRANSACTION
    public async void DeleteTransaction(string documentId)
    {
        var user = auth.CurrentUser;
        if (user == null) return;

        await db.Collection("users")
                .Document(user.UserId)
                .Collection("transactions")
                .Document(documentId)
                .DeleteAsync();

        Debug.Log("Transaction deleted!");
    }
}
