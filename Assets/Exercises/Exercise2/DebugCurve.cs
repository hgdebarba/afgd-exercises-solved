using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AfGD
{
    [ExecuteInEditMode]
    public class DebugCurve : MonoBehaviour
    {
        CurveSegment curve;
        [Tooltip("curve control points/vectors")]
        public Transform cp1, cp2, cp3, cp4;
        [Tooltip("Set the curve type")]
        public CurveType curveType = CurveType.BEZIER;


        // these variables are only used for visualization
        [Header("Debug variables")]
        [Range(2, 100)]
        public int debugSegments = 20;
        public bool drawPath = true;
        public Color pathColor = Color.magenta;
        public bool drawTangents = true;
        public Color tangentColor = Color.green;


        bool Init()
        {
            if (cp1 == null || cp2 == null || cp3 == null || cp4 == null)
                return false;
            curve = new CurveSegment(cp1.position, cp2.position, cp3.position, cp4.position, curveType);
            return true;
        }



        public static void DrawCurveSegments(CurveSegment curve,
            Color color, int segments = 50)
        {
            float interval = 1.0f / segments;
            Vector3 lastPos = curve.Evaluate(0);
            for (int i = 1; i <= segments; i++)
            {
                float u = interval * (float)i;
                Vector3 pos = curve.Evaluate(u);

                UnityEngine.Debug.DrawLine(lastPos, pos, color);
                lastPos = pos;
            }
        }

        public static void DrawTangents(CurveSegment curve,
            Color color, int segments = 50, float scale = 0.1f)
        {
            float interval = 1.0f / segments;

            for (int i = 0; i <= segments; i++)
            {
                float u = interval * (float)i;
                Vector3 pos = curve.Evaluate(u);
                Vector3 tangent = curve.EvaluateDv(u);

                UnityEngine.Debug.DrawLine(pos, pos + tangent * scale, color);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            Init();

        }

        // Update is called once per frame
        void Update()
        {
            if (Application.isEditor)
            {
                // reinitialize if we change somethign while not playing
                if (!Init())
                    return;
            }

            if(curveType == CurveType.HERMITE)
            {
                // control vectors
                Debug.DrawLine(cp1.position, cp2.position);
                Debug.DrawLine(cp4.position, cp3.position);
            }
            else
            {
                // line connecting control points
                Debug.DrawLine(cp1.position, cp2.position);
                Debug.DrawLine(cp2.position, cp3.position);
                Debug.DrawLine(cp3.position, cp4.position);
            }


            if (drawPath)
                DrawCurveSegments(curve, pathColor, debugSegments);
            if (drawTangents)
                DrawTangents(curve, tangentColor, debugSegments);

        }
    }
}