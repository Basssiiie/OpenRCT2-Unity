using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

#nullable enable

namespace OpenRCT2.Utilities
{
    /// <summary>
    /// Script that can control UI elements as if something is loading.
    /// </summary>
    public class LoaderScript : MonoBehaviour
    {
        [SerializeField] Text? _text;
        [SerializeField] Text? _counter;
        [SerializeField] Slider? _slider;
        [SerializeField] int _updateInterval = 250;


        string _mostRecentText = string.Empty;
        int _currentProgress;
        int _maximumProgress;


        /// <summary>
        /// Sets the text and progress for the loader.
        /// </summary>
        public void SetLoader(string text, int progress, int maximumProgress)
        {
            _mostRecentText = text;
            _currentProgress = progress;
            _maximumProgress = maximumProgress;
        }


        /// <summary>
        /// Runs a coroutine that updates a loader.
        /// </summary>
        public void RunCoroutine(IEnumerator coroutine)
        {
            gameObject.SetActive(true);

            // Wrap the coroutine for performance; yields are skipped within the specified timeframe.
            StartCoroutine(CoroutineRunner(coroutine));
        }


        /// <summary>
        /// The runner that runs the coroutine and updates the loader every interval.
        /// </summary>
        IEnumerator CoroutineRunner(IEnumerator innerCoroutine)
        {
            int awaitTick = Environment.TickCount + _updateInterval;
            while (innerCoroutine.MoveNext())
            {
                object current = innerCoroutine.Current;
                int currentTick = Environment.TickCount;

                if (current != null || awaitTick < currentTick)
                {
                    awaitTick = currentTick + _updateInterval;
                    UpdateViews();
                    yield return current;
                }
            }

            gameObject.SetActive(false);
        }


        /// <summary>
        /// Updates the view elements (text and slider).
        /// </summary>
        void UpdateViews()
        {
            if (_text != null)
            {
                _text.text = _mostRecentText;
            }
            if (_counter != null)
            {
                _counter.text = $"{_currentProgress}/{_maximumProgress}";
            }
            if (_slider != null)
            {
                _slider.value = Mathf.Clamp01((float)_currentProgress / _maximumProgress);
            }
        }
    }
}
