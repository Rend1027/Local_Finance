using UnityEngine;
using UnityEngine.SceneManagement;

public class NavBarController : MonoBehaviour
{
    public void GoHome()
    {
        SceneManager.LoadScene("Dashboard");
    }

    public void GoTransactions()
    {
        SceneManager.LoadScene("TransactionPage");
    }

    public void GoReports()
    {
        Debug.Log("Reports Page Coming Soon!");
    }

    public void GoMore()
    {
        Debug.Log("More Page Coming Soon!");
    }
}
