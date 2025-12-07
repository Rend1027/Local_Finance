using UnityEngine;

public class AuthUIManager : MonoBehaviour
{
    public GameObject loginPanel;
    public GameObject signupPanel;

    private void Start()
    {
        // Make sure the Login panel is shown first
        ShowLogin();
    }

    public void ShowLogin()
    {
        loginPanel.SetActive(true);
        signupPanel.SetActive(false);
    }

    public void ShowSignup()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(true);
    }
}
