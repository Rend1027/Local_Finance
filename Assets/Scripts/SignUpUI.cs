using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using System.Collections;

public class SignUpUI : MonoBehaviour
{
    public TMP_InputField nameField;
    public TMP_InputField emailField;
    public TMP_InputField passwordField;
    public TMP_InputField confirmPasswordField;
    public TMP_Text errorText;

    private FirebaseAuth auth;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    public void OnSignUpButtonPressed()
    {
        errorText.text = ""; // Clear previous errors
        Debug.Log("BUTTON CLICKED!");

        // Validate UI fields BEFORE calling Firebase
        if (passwordField.text != confirmPasswordField.text)
        {
            errorText.text = "Passwords do not match.";
            return;
        }

        if (passwordField.text.Length < 6)
        {
            errorText.text = "Password must be at least 6 characters.";
            return;
        }

        StartCoroutine(SignUpRoutine(emailField.text, passwordField.text, nameField.text));
    }

    private IEnumerator SignUpRoutine(string email, string password, string displayName)
    {
        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.Exception != null)
        {
            FirebaseException firebaseEx = registerTask.Exception.GetBaseException() as FirebaseException;
            AuthError error = (AuthError)firebaseEx.ErrorCode;

            errorText.text = FirebaseErrorMessage(error);
            errorText.color = Color.red;
        }
        else
        {
            Firebase.Auth.AuthResult result = registerTask.Result;
            Firebase.Auth.FirebaseUser newUser = result.User;

            Debug.Log("🔥 User Created: " + newUser.Email);

            // Update Display Name
            var profile = new UserProfile { DisplayName = displayName };
            var profileTask = newUser.UpdateUserProfileAsync(profile);
            yield return new WaitUntil(() => profileTask.IsCompleted);

            errorText.text = "Account created!";
            errorText.color = Color.green;

            // TODO: Navigate to next scene
            // SceneManager.LoadScene("HomePage");
        }
    }

    private string FirebaseErrorMessage(AuthError errorCode)
    {
        switch (errorCode)
        {
            case AuthError.EmailAlreadyInUse:
                return "Email is already in use.";
            case AuthError.InvalidEmail:
                return "Invalid email address.";
            case AuthError.WeakPassword:
                return "Password is too weak.";
            default:
                return "Sign-up failed. Please try again.";
        }
    }
}

