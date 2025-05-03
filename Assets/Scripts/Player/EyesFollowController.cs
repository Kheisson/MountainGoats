using UnityEngine;

public class EyesFollowController : MonoBehaviour
{
    public Eye[] eyes;

    public Transform Target { get; set; }

    protected void Awake()
    {
        // Save original local positions
        foreach (var eye in eyes)
        {
            eye.eyeStartLocalPos = eye.eyeTransform.localPosition;
            eye.pupilStartLocalPos = eye.pupilTransform.localPosition;
        }
    }

    protected void Update()
    {
        if (Target == null) return;

        foreach (var eye in eyes)
        {
            Vector3 worldTargetPos = Target.position;
            Vector3 eyeWorldPos = eye.eyeTransform.parent.TransformPoint(eye.eyeStartLocalPos); // from parent local to world

            Vector3 direction = (worldTargetPos - eyeWorldPos).normalized;

            // Eye slight movement
            Vector3 desiredEyePos = eye.eyeStartLocalPos + direction * eye.eyeMoveAmount;
            eye.eyeTransform.localPosition = Vector3.Lerp(
                eye.eyeTransform.localPosition,
                desiredEyePos,
                Time.deltaTime * eye.eyeSmoothSpeed
            );

            // Pupil smaller movement inside the eye
            Vector3 desiredPupilPos = eye.pupilStartLocalPos + direction * eye.pupilMoveAmount;
            eye.pupilTransform.localPosition = Vector3.Lerp(
                eye.pupilTransform.localPosition,
                desiredPupilPos,
                Time.deltaTime * eye.pupilSmoothSpeed
            );
        }
    }
    
    [System.Serializable]
    public class Eye
    {
        public Transform eyeTransform;      // Eye GameObject
        public Transform pupilTransform;    // Pupil inside the eye
        [HideInInspector] public Vector3 eyeStartLocalPos;
        [HideInInspector] public Vector3 pupilStartLocalPos;
        public float eyeMoveAmount = 0.02f;   // How far the eye can move
        public float pupilMoveAmount = 0.04f; // How far the pupil can move inside the eye
        public float eyeSmoothSpeed = 10f;    // Eye smoothing speed
        public float pupilSmoothSpeed = 15f;  // Pupil smoothing speed
    }
}