using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System.Threading.Tasks;

public class FirebaseInitializer : MonoBehaviour
{
    public static FirebaseAuth Auth;
    public static FirebaseUser User;
    public static FirebaseFirestore DB;

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);
        await InitializeFirebase();
    }

    private async Task InitializeFirebase()
    {
        Debug.Log("🔥 Initializing Firebase…");

        var status = await FirebaseApp.CheckAndFixDependenciesAsync();

        if (status != DependencyStatus.Available)
        {
            Debug.LogError("❌ Firebase dependency error: " + status);
            return;
        }

        FirebaseApp app = FirebaseApp.DefaultInstance;

        Auth = FirebaseAuth.DefaultInstance;
        DB = FirebaseFirestore.DefaultInstance;

        Debug.Log("🔥 Firebase READY");
        Debug.Log("🔥 Project ID = " + app.Options.ProjectId);
        Debug.Log("🔥 Firestore instance = " + DB);
    }
}
