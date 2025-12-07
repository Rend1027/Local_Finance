using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Auth;
using System.Collections;
using UnityEngine.SceneManagement;


public class LoginUI : MonoBehaviour
{
    public TMP_InputField emailField;
    public TMP_InputField passwordField;
    public TMP_Text errorText;

    private FirebaseAuth auth;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    public void OnLoginButtonPressed()
    {
        errorText.text = ""; // Clear errors
        StartCoroutine(LoginRoutine());
    }

    private IEnumerator LoginRoutine()
    {
        string email = emailField.text.Trim();
        string password = passwordField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            errorText.text = "Please fill all fields.";
            yield break;
        }

        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            FirebaseException firebaseEx = (FirebaseException)loginTask.Exception.GetBaseException();
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            errorText.text = FirebaseErrorMessage(errorCode);
            yield break;
        }

        FirebaseUser user = loginTask.Result.User;

        // SUCCESS
        errorText.text = "Login Successful!";
        errorText.color = Color.green;

        // Load Dashboard scene after successful login
        SceneManager.LoadScene("Dashboard");
    }

    private string FirebaseErrorMessage(AuthError errorCode)
    {
        switch (errorCode)
        {
            case AuthError.InvalidEmail:
                return "Invalid email format.";
            case AuthError.WrongPassword:
                return "Incorrect password.";
            case AuthError.UserNotFound:
                return "Account not found.";
            case AuthError.MissingEmail:
                return "Email required.";
            case AuthError.MissingPassword:
                return "Password required.";
            default:
                return "Login failed. Please try again.";
        }
    }
}
