using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using Firebase.Auth;
using Firebase.Firestore;

public class DashboardController : MonoBehaviour
{
    [Header("Summary UI")]
    public TMP_Text incomeValueText;
    public TMP_Text expensesValueText;
    public TMP_Text netProfitValueText;

    [Header("Prefabs")]
    public GameObject transactionItemPrefab;   // recent transactions (read-only card)
    public GameObject categoryItemPrefab;      // for top expenses + income sources

    [Header("List Parents")]
    public Transform transactionsContent;      // recent 5
    public Transform topExpensesContent;
    public Transform incomeSourcesContent;

    private FirebaseFirestore db;
    private FirebaseAuth auth;

    private void Awake()
    {
        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;
    }

    private void OnEnable()
    {
        _ = LoadAndPopulateAsync();
    }


    // MAIN LOADER
    private async Task LoadAndPopulateAsync()
    {
        ClearChildren(transactionsContent);
        ClearChildren(topExpensesContent);
        ClearChildren(incomeSourcesContent);

        var user = auth.CurrentUser;
        if (user == null)
        {
            Debug.LogWarning("DashboardController: No logged-in user.");
            PopulateSummary(0f, 0f);
            return;
        }

        List<TransactionModel> allTx = new List<TransactionModel>();

        try
        {
            // Load ALL transactions for calculations
            QuerySnapshot allSnapshot = await db
                .Collection("users")
                .Document(user.UserId)
                .Collection("transactions")
                .OrderByDescending("Timestamp")
                .GetSnapshotAsync();

            foreach (var doc in allSnapshot.Documents)
            {
                allTx.Add(doc.ConvertTo<TransactionModel>());
            }


            // Populate Summary (income, expenses, net)
            float incomeTotal = allTx.Where(t => t.IsIncome)
                                     .Sum(t => t.Amount);

            float expensesTotal = allTx.Where(t => !t.IsIncome)
                                       .Sum(t => Mathf.Abs(t.Amount));

            PopulateSummary(incomeTotal, expensesTotal);


            // Recent Transactions — ONLY TOP 5
            var recent = allTx.Take(5).ToList();

            foreach (var tx in recent)
                CreateDashboardTransactionItem(tx);


            // TOP EXPENSES + INCOME SOURCES
            PopulateTopExpenses(allTx);
            PopulateIncomeSources(allTx);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"DashboardController: Failed Firestore load: {e}");
        }
    }

    // UI HELPERS
    void PopulateSummary(float income, float expenses)
    {
        float net = income - expenses;
        incomeValueText.text = "$" + income.ToString("N2");
        expensesValueText.text = "$" + expenses.ToString("N2");
        netProfitValueText.text = "$" + net.ToString("N2");
    }

    void ClearChildren(Transform parent)
    {
        foreach (Transform child in parent)
            Destroy(child.gameObject);
    }

    // Recent Transactions (read-only card)
    void CreateDashboardTransactionItem(TransactionModel tx)
    {
        GameObject go = Instantiate(transactionItemPrefab, transactionsContent);

        go.transform.Find("LeftBlock/TextBlock/Title")
            .GetComponent<TMP_Text>().text = tx.Title;

        go.transform.Find("LeftBlock/TextBlock/Description")
            .GetComponent<TMP_Text>().text = tx.Description;

        string sign = tx.Amount >= 0 ? "+" : "-";
        go.transform.Find("RightBlock/AmountContainer/Amount")
            .GetComponent<TMP_Text>().text =
            sign + Mathf.Abs(tx.Amount).ToString("N2");

        Color c = tx.IsIncome ? Color.green : Color.red;
        go.transform.Find("RightBlock/AmountContainer/Amount")
            .GetComponent<TMP_Text>().color = c;

        go.transform.Find("LeftBlock/StatusDot")
            .GetComponent<UnityEngine.UI.Image>().color = c;
    }

    // TOP EXPENSES
    void PopulateTopExpenses(List<TransactionModel> allTx)
    {
        var expenses = allTx.Where(t => !t.IsIncome && !string.IsNullOrEmpty(t.Category));

        var grouped = expenses
            .GroupBy(t => t.Category)
            .Select(g => new
            {
                Category = g.Key,
                Total = g.Sum(x => Mathf.Abs(x.Amount))
            })
            .OrderByDescending(x => x.Total)
            .ToList();

        ClearChildren(topExpensesContent);

        foreach (var e in grouped)
        {
            GameObject go = Instantiate(categoryItemPrefab, topExpensesContent);

            go.transform.Find("CategoryNameText")
                .GetComponent<TMP_Text>().text = e.Category;

            go.transform.Find("CategoryAmountText")
                .GetComponent<TMP_Text>().text = "$" + e.Total.ToString("N2");
        }
    }

    // INCOME SOURCES
    void PopulateIncomeSources(List<TransactionModel> allTx)
    {
        var income = allTx.Where(t => t.IsIncome && !string.IsNullOrEmpty(t.Category));

        var grouped = income
            .GroupBy(t => t.Category)
            .Select(g => new
            {
                Category = g.Key,
                Total = g.Sum(x => x.Amount)
            })
            .OrderByDescending(x => x.Total)
            .ToList();

        ClearChildren(incomeSourcesContent);

        foreach (var e in grouped)
        {
            GameObject go = Instantiate(categoryItemPrefab, incomeSourcesContent);

            go.transform.Find("CategoryNameText")
                .GetComponent<TMP_Text>().text = e.Category;

            go.transform.Find("CategoryAmountText")
                .GetComponent<TMP_Text>().text = "$" + e.Total.ToString("N2");
        }
    }
}
