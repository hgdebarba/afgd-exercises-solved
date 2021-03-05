using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace AfGD.Exercise5
{

    public class ArmHierarchy : MonoBehaviour
    {

        Skeleton arm;
        // array used to store animation poses to interpolate across
        AnimationPose[] animationPoses;

        AnimationPose currentPose;


        [Range(0.1f, 10.0f)]
        public float animationTime = 2.0f;

        private void Start()
        {
            Init();
        }

        void Init()
        {
            if (animationPoses != null)
                return;

            // create the arm skeleton
            SkeletonJoint shoulder = new SkeletonJoint() { m_invBindPose = Matrix4x4.identity, m_name = "shoulder", m_iParent = 255 };
            SkeletonJoint elbow = new SkeletonJoint() { m_invBindPose = Matrix4x4.identity, m_name = "elbow", m_iParent = 0 };
            SkeletonJoint wrist = new SkeletonJoint() { m_invBindPose = Matrix4x4.identity, m_name = "wrist", m_iParent = 1 };
            SkeletonJoint[] joints = new SkeletonJoint[3] { shoulder, elbow, wrist };
            arm = new Skeleton() { m_aJoint = joints };

            // create animation frame 1 (index 0)
            JointPose shoulder1 = new JointPose{
                m_rot = Quaternion.AngleAxis(0.0f, Vector3.forward),
                m_pos = new Vector3(0.0f, 0.0f, 0.0f),
                m_scale = 1.0f
            };
            JointPose elbow1 = new JointPose { 
                m_rot = Quaternion.AngleAxis(0.0f, Vector3.forward),
                m_pos = new Vector3(1.0f, 0.0f, 0.0f), 
                m_scale = 1.0f 
            };
            JointPose wrist1 = new JointPose { 
                m_rot = Quaternion.AngleAxis(0.0f, Vector3.forward), 
                m_pos = new Vector3(1.0f, 0.0f, 0.0f),
                m_scale = 1.0f 
            };
            JointPose[] poses1 = new JointPose[3] { shoulder1, elbow1, wrist1 };
            AnimationPose frame1 = new AnimationPose { m_aLocalPose = poses1 };

            // create animation frame 2 (index 1)
            JointPose shoulder2 = new JointPose
            {
                m_rot = Quaternion.AngleAxis(60.0f, Vector3.forward),
                m_pos = new Vector3(0.0f, 0.0f, 0.0f),
                m_scale = 1.0f
            };
            JointPose elbow2 = new JointPose
            {
                m_rot = Quaternion.AngleAxis(-75.0f, Vector3.forward),
                m_pos = new Vector3(1.0f, 0.0f, 0.0f),
                m_scale = 1.0f
            };
            JointPose wrist2 = new JointPose
            {
                m_rot = Quaternion.AngleAxis(40.0f, Vector3.forward),
                m_pos = new Vector3(1.0f, 0.0f, 0.0f),
                m_scale = 1.0f
            };
            JointPose[] poses2 = new JointPose[3] { shoulder2, elbow2, wrist2 };
            AnimationPose frame2 = new AnimationPose { m_aLocalPose = poses2 };


            // create animation frame 3 (index 2)
            JointPose shoulder3 = new JointPose
            {
                m_rot = Quaternion.AngleAxis(30.0f, Vector3.forward),
                m_pos = new Vector3(1.0f, 0.0f, 0.0f),
                m_scale = 1.0f
            };
            JointPose elbow3 = new JointPose
            {
                m_rot = Quaternion.AngleAxis(-25.0f, Vector3.forward),
                m_pos = new Vector3(1.0f, 0.0f, 0.0f),
                m_scale = 1.0f
            };
            JointPose wrist3 = new JointPose
            {
                m_rot = Quaternion.AngleAxis(10.0f, Vector3.forward),
                m_pos = new Vector3(1.0f, 0.0f, 0.0f),
                m_scale = 1.0f
            };


            JointPose[] poses3 = new JointPose[3] { shoulder3, elbow3, wrist3 };
            AnimationPose frame3 = new AnimationPose { m_aLocalPose = poses3 };


            // array of animation poses, frame 1 at index 0 and frame 2 at index 1
            animationPoses = new AnimationPose[3] { frame1, frame2, frame3 };

            // initialize current pose to pose 1
            JointPose[] poses = new JointPose[3] { shoulder1, wrist1, wrist1 };
            currentPose = new AnimationPose { m_aLocalPose = poses };
        }

        private void Update()
        {
            // we get our normalized time (in the range [0, 1])
            float u = Time.time;
            u = u % animationTime / animationTime;


            // TODO exercise 5.3
            // for each joint:
            // Perform interpolation between two consecutive AnimationFrames (animationPoses)
            // It should interpolate rotation, translation and scale of each joint 
            // based on the normalized elapsed time u
            // Save the result in the currentPose object (used to draw the joint)
            // IMPLEMENTATION HERE (slide 37, how can we get poses in between our samples?)
            //    int frame1 = 0, frame2 = 1;
            
            //for (int i =0; i < currentPose.m_aLocalPose.Length; i++)
            //{
            //    Vector3 p1 = animationPoses[frame1].m_aLocalPose[i].m_pos;
            //    Vector3 p2 = animationPoses[frame2].m_aLocalPose[i].m_pos;
            //    currentPose.m_aLocalPose[i].m_pos = Vector3.Lerp(p1, p2, u);
            //     this is what the Lerp function does:
            //    currentPose.m_aLocalPose[i].m_pos = (1f - u) * p1 + u * p2;
            //     here represented as a point + vector operation, instead of a weighted sum:
            //    currentPose.m_aLocalPose[i].m_pos = p1 + (p1-p2) * u;;

            //    Quaternion q1 = animationPoses[frame1].m_aLocalPose[i].m_rot;
            //    Quaternion q2 = animationPoses[frame2].m_aLocalPose[i].m_rot;
            //    currentPose.m_aLocalPose[i].m_rot = Quaternion.Slerp(q1, q2, u);

            //    float s1 = animationPoses[frame1].m_aLocalPose[i].m_scale;
            //    float s2 = animationPoses[frame2].m_aLocalPose[i].m_scale;
            //    currentPose.m_aLocalPose[i].m_scale = Mathf.Lerp(s1, s2, u);
            //}

            // TODO (optional) exercise 5.3b
            // you have worked on the interpolation between two frames,
            // what happens if we want to have a third or a fourth frame?
            // create another frame and interpolate from the first to the last frame
            // in the animationPoses[] array
            // IMPLEMENTATION HERE
            int framesCount = (animationPoses.Length - 1);
            float uScaled = framesCount * u;

            int frame1 = Mathf.FloorToInt(uScaled); 
            int frame2 = (frame1+1);
            u = uScaled - frame1;
            for (int i = 0; i < currentPose.m_aLocalPose.Length; i++)
            {
                
                Vector3 p1 = animationPoses[frame1].m_aLocalPose[i].m_pos;
                Vector3 p2 = animationPoses[frame2].m_aLocalPose[i].m_pos;
                currentPose.m_aLocalPose[i].m_pos = Vector3.Lerp(p1, p2, u);
                // this is what the Lerp function does:
                // currentPose.m_aLocalPose[i].m_pos = (1f - u) * p1 + u * p2;
                // here represented as a point + vector operation, instead of a weighted sum:
                // currentPose.m_aLocalPose[i].m_pos = p1 + (p1-p2) * u;

                Quaternion q1 = animationPoses[frame1].m_aLocalPose[i].m_rot;
                Quaternion q2 = animationPoses[frame2].m_aLocalPose[i].m_rot;
                currentPose.m_aLocalPose[i].m_rot = Quaternion.Slerp(q1, q2, u);

                float s1 = animationPoses[frame1].m_aLocalPose[i].m_scale;
                float s2 = animationPoses[frame2].m_aLocalPose[i].m_scale;
                currentPose.m_aLocalPose[i].m_scale = Mathf.Lerp(s1, s2, u);
            }

        }

        private void OnDrawGizmos()
        {

            Init();

            // retrieve the current pose in 
            Matrix4x4[] globalPose = new Matrix4x4[arm.m_aJoint.Length];

            // TODO exercise 5.1 - implement the GetPoseInGlobalSpace function in the Skeleton class
            arm.GetPoseInGlobalSpace(currentPose, ref globalPose);

            int jointCount = arm.m_aJoint.Length;
            for (int i = 0; i < jointCount; i++)
            {
                // ith joint in world space
                Matrix4x4 P = transform.localToWorldMatrix * globalPose[i];
                // draw sphere at joint location
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(P.GetColumn(3), 0.05f);
                // draw local basis vectors
                DebugDraw.DrawFrame(P, .2f); 

                // draw a line connecting the joints
                byte parentId = arm.m_aJoint[i].m_iParent;
                // skip this one if it is the root
                if (parentId == 255) 
                    continue;
                Debug.DrawLine(P.GetColumn(3), 
                    (transform.localToWorldMatrix * globalPose[parentId]).GetColumn(3), Color.cyan);

            }

        }


    }
}