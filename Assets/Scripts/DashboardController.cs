using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DashboardController : MonoBehaviour
{
    [Header("Summary UI")]
    public TMP_Text incomeValueText;
    public TMP_Text expensesValueText;
    public TMP_Text netProfitValueText;

    [Header("Prefabs")]
    public GameObject transactionItemPrefab;
    public GameObject categoryItemPrefab;   // used for both expenses & income

    [Header("List Parents")]
    public Transform transactionsContent;
    public Transform topExpensesContent;
    public Transform incomeSourcesContent;

    private void OnEnable()
    {
        PopulateSummary();
        PopulateRecentTransactions();
        PopulateTopExpenses();
        PopulateIncomeSources();
    }

    void PopulateSummary()
    {
        float income = 10000f;
        float expenses = 5000f;
        float net = income - expenses;

        incomeValueText.text = "$" + income.ToString("N2");
        expensesValueText.text = "$" + expenses.ToString("N2");
        netProfitValueText.text = "$" + net.ToString("N2");
    }

    void ClearChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    void PopulateRecentTransactions()
    {
        ClearChildren(transactionsContent);

        var txs = new List<(string title, string desc, float amount, bool isIncome)>
        {
            ("Food supply", "Ingredients - 11/18/2025", -100f, false),
            ("Daily pizza sales", "Sales - 11/18/2025", 2000f, true),
            ("Catering", "Ingredients - 11/18/2025", 500f, true)
        };

        foreach (var tx in txs)
        {
            GameObject go = Instantiate(transactionItemPrefab, transactionsContent);

            var titleText = go.transform.Find("TextBlock/Title").GetComponent<TMP_Text>();
            var descText = go.transform.Find("TextBlock/Description").GetComponent<TMP_Text>();
            var amountText = go.transform.Find("AmountContainer/Amount").GetComponent<TMP_Text>();
            var statusDotImage = go.transform.Find("StatusDot").GetComponent<UnityEngine.UI.Image>();

            titleText.text = tx.title;
            descText.text = tx.desc;

            string sign = tx.amount >= 0 ? "+" : "-";
            amountText.text = sign + Mathf.Abs(tx.amount).ToString("N2");

            if (tx.isIncome)
            {
                amountText.color = Color.green;
                statusDotImage.color = Color.green;
            }
            else
            {
                amountText.color = Color.red;
                statusDotImage.color = Color.red;
            }
        }
    }

    void PopulateTopExpenses()
    {
        ClearChildren(topExpensesContent);

        var expenses = new List<(string name, float amount)>
        {
            ("Payroll", 4200f),
            ("Equipment", 1200f),
            ("Ingredients", 580.50f),
            ("Marketing", 450f),
            ("Utilities", 320.75f)
        };

        foreach (var e in expenses)
        {
            GameObject go = Instantiate(categoryItemPrefab, topExpensesContent);
            var nameText = go.transform.Find("CategoryNameText").GetComponent<TMP_Text>();
            var amountText = go.transform.Find("CategoryAmountText").GetComponent<TMP_Text>();

            nameText.text = e.name;
            amountText.text = "$" + e.amount.ToString("N2");
        }
    }

    void PopulateIncomeSources()
    {
        ClearChildren(incomeSourcesContent);

        var sources = new List<(string name, float amount)>
        {
            ("Sales", 7540.50f),
            ("Catering", 850f),
            ("Delivery Fees", 180.25f)
        };

        foreach (var s in sources)
        {
            GameObject go = Instantiate(categoryItemPrefab, incomeSourcesContent);
            var nameText = go.transform.Find("CategoryNameText").GetComponent<TMP_Text>();
            var amountText = go.transform.Find("CategoryAmountText").GetComponent<TMP_Text>();

            nameText.text = s.name;
            amountText.text = "$" + s.amount.ToString("N2");
        }
    }
}
