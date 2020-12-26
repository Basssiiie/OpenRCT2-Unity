using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    /// <summary>
    /// Script that can control UI elements as if something is loading.
    /// </summary>
    public class LoaderView : MonoBehaviour
    {
        [SerializeField] Text _text;
        [SerializeField] Text _counter;
        [SerializeField] Slider _slider;
        [SerializeField] int _updateInterval = 250;


        string _mostRecentText;
        int _currentProgress;
        int _maximumProgress;


        /// <summary>
        /// Gets or sets the text for the loader.
        /// </summary>
        public void SetText(string text)
        {
            _mostRecentText = text;
        }


        /// <summary>
        /// Sets the current progress of the loading bar.
        /// Resets any current progress.
        /// </summary>
        public void SetMaximumProgress(int maximumProgress)
        {
            _currentProgress = 0;
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
            int awaitTick = (Environment.TickCount + _updateInterval);
            _currentProgress = 0;

            while(innerCoroutine.MoveNext())
            {
                object current = innerCoroutine.Current;
                int currentTick = Environment.TickCount;

                _currentProgress++;

                if (current != null || awaitTick < currentTick)
                {
                    awaitTick = (currentTick + _updateInterval);
                    UpdateViews();
                    yield return current;
                }
            }

            gameObject.SetActive(false);
            _currentProgress = 0;
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
