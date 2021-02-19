using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AfGD
{
    // exercise 2.4
    // attach to an object so that it moves along a path
    public class MoveAlongPath : MonoBehaviour
    {
        [Tooltip("Path to move along")]
        public CyclicPath path;

        [Tooltip("How long should a full lap take?")]
        [Range(2, 60)]
        public float lapTime = 5.0f;

        [Tooltip("Should the object move?")]
        public bool stop = false;

        public bool parameterizeByArclength = true;
        public bool useEasingCurve = true;

        // we keep an internal clock for this object for higher flexibility
        private float localTime = 0;

        // Update is called once per frame
        void Update()
        {
            if (path == null || stop)
                return;

            localTime += Time.deltaTime;
            // one cycle every lapTime (modulo)
            localTime = localTime % lapTime;

            // time is normalimalized so [0, lapTime] -> [0,1]
            float t = localTime / lapTime;

            float s = t;
            // part of exercise 2.5, easing curve to control normalized time
            if (useEasingCurve)
                s = EasingFunctions.Crossfade(EasingFunctions.SmoothStart2, EasingFunctions.SmoothStop2, 0.5f, t);


            // exercise 2.4, move this object along the line            
            Vector3 pos = path.Evaluate(s, parameterizeByArclength);
            this.transform.position = pos;

        }
    }
}