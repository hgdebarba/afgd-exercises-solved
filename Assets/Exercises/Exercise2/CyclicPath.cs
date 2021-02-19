using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AfGD
{
    // exercise 2.3
    // create a cyclic path using a collection of curve segments.
    // I made a new file so I would not need to erase the DebugCurve    
    [ExecuteInEditMode]
    public class CyclicPath : MonoBehaviour
    {
        // we use an array of transforms as control points
        [Tooltip("Path control points")]
        public Transform[] controlPoints;

        // for simplicity, only curves conecting the second and third points are supported
        // which means that a sequence of curve segments is generated with
        // [cp1, cp2, cp3, cp4], then [cp2, cp3, cp4, cp5] and so on
        public enum CurveTypeSimple { CATMULLROM = 1, BSPLINE = 3 };
        [Tooltip("Set the curve type")]
        public CurveTypeSimple curveType = CurveTypeSimple.CATMULLROM;

        // we will need an array of curves for the path (instead of a single curve)
        private CurveSegment[] curves;

        // exercise 2.5
        // this array is used to store the normalized, cumulative arclength of the path
        // it relates the array index to the arclength of the path, so we can think of it
        // as a table
        private float[] normArclength;
        private int arclengthEntries = 200; // the length of our table/array


        // these variables are only used for debugging
        [Header("Debug variables")]
        [Range(2, 100)]
        public int debugSegments = 20;
        public bool drawPath = true;
        public Color pathColor = Color.magenta;
        public bool drawTangents = true;
        public Color tangentColor = Color.green;
        public bool drawCurvature = true;
        public Color curvatureColor = Color.yellow;


        /// <summary>
        /// initialize the path
        /// </summary>
        /// <returns></returns>
        private bool Init()
        {
            if (controlPoints == null)
                return false; // can't initialize
            
            int points = controlPoints.Length;
            if (curves == null || curves.Length != points)
                curves = new CurveSegment[points];
             
            // insteantiate a curve segment for each valid sequence of points
            // since we only support curves that connect the second and third points 
            // (b-spline and Catmull-Rom) we can advance our index by only one
            for (int i = 0; i < points; i++)
            {
                // notice how the modulo operator ensures 'i' is in the range [0, points-1]
                // and is used to wrap around the list of points to make a closed path
                Vector3 cp1 = controlPoints[i].position;
                Vector3 cp2 = controlPoints[(i + 1) % points].position;
                Vector3 cp3 = controlPoints[(i + 2) % points].position;
                Vector3 cp4 = controlPoints[(i + 3) % points].position;

                curves[i] = new CurveSegment(cp1, cp2, cp3, cp4, (CurveType)curveType);
            }
            // compute the cumulative arclength, which we use to sample the curve
            // with a parameter that is linear with relation to the arclength of the curve
            UpdateArclength();

            return true;
        }


        /// <summary>
        /// we use the function to make the path responsive to changes in the
        /// conrtol points, even when the scene is not in play mode
        /// </summary>
        /// <returns></returns>
        private bool UpdateControlPoints()
        {
            // initialize the curve segments if the number of segment 
            // isn't the same as the number of control points
            if (curves == null || curves.Length != controlPoints.Length)
            {
                return Init();
            }

            // or if any control point has moved
            for (int i = 0; i < controlPoints.Length; i++)
            {
                if (controlPoints[i].hasChanged)
                {
                    return Init();
                }
            }

            return true;
        }


        /// <summary>
        /// Evaluate the point in the path at parameter 's', 
        /// 's' is transformed to 'u' if arclengthParameterization is set
        /// Evaluate the tangent if derivative==1, and the curvature if derivative==2
        /// </summary>
        /// <param name="s"></param>
        /// <param name="arclengthParameterization"></param>
        /// <param name="derivative"></param>
        /// <returns></returns>
        public Vector3 Evaluate(float s, bool arclengthParameterization = false, int derivative = 0)
        {
            if (curves == null)
                if (!Init()) return Vector3.zero;

            // exercise 2.5
            // should we use 's' as is or parameterize by the arclength of the path?
            float u = arclengthParameterization ? ParameterizeByArclength(s) : s;

            // scale 'u' from [0,1] to [0, curves.Length]
            float pathU = u * curves.Length;
            // round down pathU to retrieve the curve segment ID
            int curveID = (int)pathU;
            // ensure that the curveID is in a valid range
            curveID = Mathf.Clamp(curveID, 0, curves.Length - 1);
            // 'u' in the selected curve segment
            float curveU = pathU - (float)curveID;


            if (derivative == 1)
                return curves[curveID].EvaluateDv(curveU);
            if (derivative == 2)
                return curves[curveID].EvaluateDv2(curveU);

            return curves[curveID].Evaluate(curveU);
        }


        // exercise 2.5
        /// <summary>
        /// construct a table with the normalized, cumulative arclength of the curve
        /// </summary>
        void UpdateArclength()
        {
            // compute entry intervals so that interval * arclengthEntries == 1
            float interval = 1.0f / (float)arclengthEntries;

            // table initialization
            normArclength = new float[arclengthEntries];
            normArclength[0] = 0.0f;

            // start position in the path
            Vector3 lastPos = Evaluate(0, false);

            for (int i = 1; i < arclengthEntries; i++)
            {
                // sample path at fixed intervals
                float u = interval * (float)i;
                Vector3 pos = Evaluate(u, false);

                // compute the distance between two consecutive samples and 
                // accumulate in arclength
                float s = normArclength[i - 1] + Vector3.Distance(lastPos, pos);
                normArclength[i] = s;

                lastPos = pos;
            }
            // now the entries in normArclength are in the range [0, pathArclenght]

            // the last element in normArclength is the total arclength of the path
            float pathArclenght = normArclength[normArclength.Length - 1];

            // ensure there is no division by 0, 
            // could happen if all control points are in the same place
            if (pathArclenght <= float.Epsilon)
                throw new System.Exception("totalArclength must be positive and bigger than 0");

            // normalise the arclength values by dividing by the last element
            for (int i = 1; i < arclengthEntries; i++)
            {
                normArclength[i] = normArclength[i] / pathArclenght;
            }
            // now the entries in normArclength are in the range [0, 1]
        }


        // exercise 2.5
        /// <summary>
        /// Parameterize by arclength. Given the table of (normalized) cumulative 
        /// arclengths of the path and a parameter 's', returns a parameter 'u' 
        /// that evaluates the point at s*arclength of the path
        /// </summary>
        /// <param name="s"></param>
        /// <returns>returns the 'u' parameter</returns>
        float ParameterizeByArclength(float s)
        {
            // ensure tha s is in a valid range
            s = Mathf.Clamp(s, 0.0f, 1.0f);

            // we perform a binary search to find the entries in the 
            // table that are closer to 's'
            int totalSegments = normArclength.Length - 1;
            int min = 0;
            int max = totalSegments;
            int current = max / 2;

            while (true)
            {
                // if min and max are neighbours, we found the best approximation
                // of 's' in the table
                if (min == max - 1)
                {
                    // table the two best approximations of 's' in the table
                    // and compute the closest 'u' with a linear interpolation
                    // this means that 'u' is just an approximation of the correct parameter
                    float s1 = normArclength[min];
                    float s2 = normArclength[max];
                    float u1 = (float)min / (float)totalSegments;
                    float u2 = (float)max / (float)totalSegments;
                    float delta_s = (s - s1) / (s2 - s1);

                    return Mathf.Lerp(u1, u2, delta_s); ;
                }

                // adjust the bounds of our search range
                if (s > normArclength[current])
                    min = current;
                else
                    max = current;

                // index in the middle of our current search range
                current = min + (max - min) / 2;
            }
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

        public static void DrawCurveTangents(CurveSegment curve,
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

        public static void DrawCurveCurvatures(CurveSegment curve,
            Color color, int segments = 50, float scale = 0.1f)
        {
            float interval = 1.0f / segments;

            for (int i = 0; i <= segments; i++)
            {
                float u = interval * (float)i;
                Vector3 pos = curve.Evaluate(u);
                Vector3 curvature = curve.EvaluateDv2(u);

                UnityEngine.Debug.DrawLine(pos, pos + curvature * scale, color);
            }
        }


        private void Start()
        {
            Init();
        }


        private void Update()
        {
            if (Application.isEditor)
            {
                if (!UpdateControlPoints())
                    return;
            }

            for (int i = 0; i < curves.Length; i++)
            {
                if (drawPath)
                    DrawCurveSegments(curves[i], pathColor, debugSegments);
                if (drawTangents)
                    DrawCurveTangents(curves[i], tangentColor, debugSegments);
                if (drawCurvature)
                    DrawCurveCurvatures(curves[i], curvatureColor, debugSegments);
            }
        }

    }
}