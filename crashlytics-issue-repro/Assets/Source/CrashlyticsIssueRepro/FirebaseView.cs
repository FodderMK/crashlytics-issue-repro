using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace CrashlyticsIssueRepro
{
    public class FirebaseView : MonoBehaviour
    {
        public TextMeshProUGUI TxtStatus;
        public TextMeshProUGUI TxtLastException;
    
        public Button BtnInitFirebase;
        public Button BtnThrowStack;
        public Button BtnThrowAsync;
        public Button BtnThrowManyThreads;
        public Button BtnFreezeFor3Seconds;

        private FirebaseInit _firebaseInit;
        private FrameFreezeMeasurer _measurer;

        private void Start()
        {
            Application.targetFrameRate = 30;

            _firebaseInit = FindObjectOfType<FirebaseInit>();
            _measurer = FindObjectOfType<FrameFreezeMeasurer>();


            BtnInitFirebase.onClick.AddListener(InitFirebase);

            ExceptionThrower thrower = new();
            BtnThrowStack.onClick.AddListener(() => ThrowAndMeasure(thrower.ThrowStack));
            BtnThrowAsync.onClick.AddListener(() => ThrowAndMeasure(thrower.ThrowAsync));
            BtnThrowManyThreads.onClick.AddListener(() => ThrowAndMeasure(thrower.ThrowManyThreads));
            BtnFreezeFor3Seconds.onClick.AddListener(() => ThrowAndMeasure(FreezeFor3Seconds));
            Application.logMessageReceivedThreaded += HandleMessageReceived;


            TxtStatus.text = "App Started";
            TxtLastException.text = string.Empty;
        }


        private async void InitFirebase()
        {
            TxtStatus.text = "Firebase Initializing...";
            try
            {
                await _firebaseInit.InitFirebase();
                TxtStatus.text = "Firebase Initialized";
                BtnInitFirebase.gameObject.SetActive(false);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                TxtStatus.text = "Error initializing Firebase";
            }
        }

        private void HandleMessageReceived(string condition, string stacktrace, LogType type)
        {
            if (type != LogType.Exception) return;
            var str = $"{condition}\n{stacktrace}";
            str = str.Length > 1500 ? str.Substring(0, 1500) : str;
            TxtLastException.text = str;
        }


        private void ThrowAndMeasure(Action action)
        {
            Task Action()
            {
                action();
                return Task.CompletedTask;
            }

            ThrowAndMeasure(Action);
        }

        private async void ThrowAndMeasure(Func<Task> action)
        {
            Stopwatch sw = Stopwatch.StartNew();

            try
            {
                await action();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            sw.Stop();
            TimeSpan frameFreeze = await _measurer.MeasureNextFrameFreeze();
            TimeSpan totalTime = sw.Elapsed + frameFreeze;
            TxtStatus.text = $"Exception Thrown (Duration: {totalTime})";
        }

        private static async void FreezeFor3Seconds()
        {
            await Task.Yield();
            Thread.Sleep(3000);
        }
    }
}