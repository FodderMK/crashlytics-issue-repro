using System.Threading.Tasks;
using Firebase;
using UnityEngine;

namespace CrashlyticsIssueRepro
{
    public class FirebaseInit : MonoBehaviour
    {
        private FirebaseApp app;

        public async Task InitFirebase()
        {
            DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();

            if (status == DependencyStatus.Available)
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = FirebaseApp.DefaultInstance;
            // Set a flag here to indicate whether Firebase is ready to use by your app.
            else
                Debug.LogError($"Could not resolve all Firebase dependencies: {status}");
            // Firebase Unity SDK is not safe to use here.
        }
    }
}