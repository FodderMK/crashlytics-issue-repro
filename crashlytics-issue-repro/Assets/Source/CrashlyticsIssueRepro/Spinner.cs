using UnityEngine;

namespace CrashlyticsIssueRepro
{
    public class Spinner : MonoBehaviour
    {
        // Update is called once per frame
        private void Update()
        {
            transform.Rotate(Vector3.forward * (720f * Time.deltaTime));
        }
    }
}