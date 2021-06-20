using System;
using System.Collections;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class TimerSystem
    {
        [Inject] private TimerCoroutinesContainer CoroutinesContainer { get; set; }
        
        public void RunAfter(float time, Action action)
        {
            CoroutinesContainer.StartCoroutine(ActionRunEnumerator(time, action));
        }

        private IEnumerator ActionRunEnumerator(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action();
        }
    }
}