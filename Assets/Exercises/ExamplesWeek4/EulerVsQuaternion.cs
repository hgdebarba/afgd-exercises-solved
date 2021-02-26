using UnityEngine;

namespace AdGD
{
    public class EulerVsQuaternion : MonoBehaviour
    {
        public Transform startRot, endRot;
        public bool useQuaternion = false;
        [Range(1, 30)]
        public float duration = 3.0f;

        void Update()
        {
            float t = Time.time % duration / duration;

            Quaternion qFrom = startRot.rotation;
            Quaternion qTo = endRot.rotation;
            Quaternion qT = Quaternion.Slerp(qFrom, qTo, t);

            Vector3 eFrom = startRot.eulerAngles;
            Vector3 eTo = endRot.eulerAngles;
            Vector3 eT = Vector3.Lerp(eFrom, eTo, t);

            if (useQuaternion)
                transform.rotation = qT;
            else
                transform.rotation = Quaternion.Euler(eT);
        }
    }
}
