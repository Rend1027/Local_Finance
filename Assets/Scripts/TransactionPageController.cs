using UnityEngine;
using TMPro;

public class TransactionPageController : MonoBehaviour
{
    public GameObject modalOverlay;
    public GameObject transactionFormPanel;

    public void OpenForm()
    {
        modalOverlay.SetActive(true);
        transactionFormPanel.SetActive(true);
    }

    public void CloseForm()
    {
        modalOverlay.SetActive(false);
        transactionFormPanel.SetActive(false);
    }

    public Transform transactionsContent;
    public GameObject transactionItemPrefab;

    private void Start()
    {
        LoadTransactions();
    }

    void LoadTransactions()
    {
        // Fake test data (for now)
        var transactions = new (string title, string desc, float amount, bool isIncome)[]
        {
            ("Weekend Sales", "Income • Sales", 3200f, true),
            ("Social media ads", "Expense • Marketing", -450f, false),
            ("Electricity and water", "Expense • Utilities", -320.75f, false),
        };

        foreach (var t in transactions)
        {
            GameObject go = Instantiate(transactionItemPrefab, transactionsContent);

            go.transform.Find("TextBlock/Title").GetComponent<TMP_Text>().text = t.title;
            go.transform.Find("TextBlock/Description").GetComponent<TMP_Text>().text = t.desc;

            var amountText = go.transform.Find("AmountContainer/Amount").GetComponent<TMP_Text>();
            amountText.text = (t.isIncome ? "+" : "-") + Mathf.Abs(t.amount).ToString("N2");
            amountText.color = t.isIncome ? Color.green : Color.red;

            var dot = go.transform.Find("StatusDot").GetComponent<UnityEngine.UI.Image>();
            dot.color = t.isIncome ? Color.green : Color.red;
        }
    }
}
