using UnityEngine;
using Firebase;
using Firebase.Auth;

public class FirebaseInitializer : MonoBehaviour
{
    public static FirebaseAuth Auth;
    public static FirebaseUser User;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }


    void Start()
    {
        InitializeFirebase();
    }

    void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var status = task.Result;

            if (status == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;

                // -------------------------------------------------------
                // 🔥 FIX: Force Firebase to use the real backend
                // -------------------------------------------------------
                PlayerPrefs.SetString("FIREBASE_AUTH_EMULATOR_HOST", "");
                PlayerPrefs.SetInt("USE_EMULATOR", 0);
                Debug.Log("Emulator disabled. Using live Firebase backend.");
                // -------------------------------------------------------

                Auth = FirebaseAuth.DefaultInstance;

                Debug.Log("Firebase is Ready");
            }
            else
            {
                Debug.LogError("Firebase dependency error: " + status);
            }
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
