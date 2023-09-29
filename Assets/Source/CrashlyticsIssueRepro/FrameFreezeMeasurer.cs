using System;
using System.Threading.Tasks;
using UnityEngine;

namespace CrashlyticsIssueRepro
{
    /// <summary>
    ///     Measures the time it takes to execute a method, waits a specified amount of time until a frame-freeze occurs, and
    ///     then measures the duration of that freeze.
    /// </summary>
    public class FrameFreezeMeasurer : MonoBehaviour
    {
        public bool IsFreezing { get; private set; }

        private void Start()
        {
            Time.maximumDeltaTime = float.MaxValue;
        }

        private void Update()
        {
            //heuristic: game is considered to be freezing if framerate is lower than half the target framerate.
            IsFreezing = Time.deltaTime > 1f / Application.targetFrameRate * 2f;
        }


        public async Task<TimeSpan> MeasureNextFrameFreeze()
        {
            var maxDeltaTime = 0f;

            var startTime = Time.time;

            //ignore period where game isn't freezing yet
            while (!IsFreezing)
            {
                //if no freeze occurs, continue
                if (Time.time - startTime > .5f) break;

                await Task.Yield();
            }


            while (IsFreezing)
            {
                if (Time.deltaTime > maxDeltaTime) maxDeltaTime = Time.deltaTime;

                await Task.Yield();
            }


            return TimeSpan.FromSeconds(maxDeltaTime);
        }
    }
}