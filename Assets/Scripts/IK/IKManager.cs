using UnityEngine;

public class IKManager : MonoBehaviour
{
    /// <summary>
    /// Chain length of bones
    /// </summary>
    [SerializeField] private int ChainLength = 2;

    /// <summary>
    /// Target the chain should bent to
    /// </summary>
    [SerializeField] private Transform Target;

    [SerializeField] private Transform Pole;

    /// <summary>
    /// Solver iterations per update
    /// </summary>
    [Header("Solver Parameters")]
    [SerializeField] private int Iterations = 10;

    /// <summary>
    /// Distance when the solver stops
    /// </summary>
    [SerializeField] private float Delta = 0.001f;

    /// <summary>
    /// Strength of going back to the start position.
    /// </summary>
    [Range(0, 1)]
    [SerializeField] private float SnapBackStrength = 1f;

    protected float[] BonesLength; //Target to Origin
    protected float CompleteLength;
    protected Transform[] Bones;
    protected Vector3[] Positions;
    protected Vector3[] StartDirectionSucc;
    protected Quaternion[] StartRotationBone;
    protected Quaternion StartRotationTarget;
    protected Transform Root;

    // Start is called before the first frame update
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        //initial array
        Bones = new Transform[ChainLength + 1];
        Positions = new Vector3[ChainLength + 1];
        BonesLength = new float[ChainLength];
        StartDirectionSucc = new Vector3[ChainLength + 1];
        StartRotationBone = new Quaternion[ChainLength + 1];

        //find root
        Root = transform;
        for (int i = 0; i <= ChainLength; i++)
        {
            if (Root == null)
            {
                throw new UnityException("The chain value is longer than the ancestor chain!");
            }

            Root = Root.parent;
        }

        //init target
        if (Target == null)
        {
            Target = new GameObject(gameObject.name + " Target").transform;
            SetPositionRootSpace(Target, GetPositionRootSpace(transform));
        }
        StartRotationTarget = GetRotationRootSpace(Target);

        //init data
        Transform current = transform;
        CompleteLength = 0;
        for (int i = Bones.Length - 1; i >= 0; i--)
        {
            Bones[i] = current;
            StartRotationBone[i] = GetRotationRootSpace(current);

            if (i == Bones.Length - 1)
            {
                //leaf
                StartDirectionSucc[i] = GetPositionRootSpace(Target) - GetPositionRootSpace(current);
            }
            else
            {
                //mid bone
                StartDirectionSucc[i] = GetPositionRootSpace(Bones[i + 1]) - GetPositionRootSpace(current);
                BonesLength[i] = StartDirectionSucc[i].magnitude;
                CompleteLength += BonesLength[i];
            }

            current = current.parent;
        }
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        ResolveIK();
    }

    private void ResolveIK()
    {
        //get position
        for (int i = 0; i < Bones.Length; i++)
        {
            Positions[i] = GetPositionRootSpace(Bones[i]);
        }

        Vector3 targetPosition = GetPositionRootSpace(Target);
        Quaternion targetRotation = GetRotationRootSpace(Target);


        for (int i = 0; i < Positions.Length - 1; i++)
        {
            Positions[i + 1] = Vector3.Lerp(Positions[i + 1], Positions[i] + StartDirectionSucc[i], SnapBackStrength);
        }

        for (int iteration = 0; iteration < Iterations; iteration++)
        {
            Positions[Positions.Length - 1] = targetPosition;

            //back
            for (int i = Positions.Length - 2; i > 0; i--)
            {
                Positions[i] = Positions[i + 1] + (Positions[i] - Positions[i + 1]).normalized * BonesLength[i]; //set in line on distance
            }

            //forward
            for (int i = 1; i < Positions.Length; i++)
            {
                Positions[i] = Positions[i - 1] + (Positions[i] - Positions[i - 1]).normalized * BonesLength[i - 1];
            }

            //close enough?
            if ((Positions[Positions.Length - 1] - targetPosition).sqrMagnitude < Delta * Delta)
            {
                break;
            }
        }
        Vector3 polePosition = GetPositionRootSpace(Pole);

        for (int i = 1; i < Positions.Length - 1; i++)
        {
            Plane plane = new Plane(Positions[i + 1] - Positions[i - 1], Positions[i - 1]);
            Vector3 projectedPole = plane.ClosestPointOnPlane(polePosition);
            Vector3 projectedBone = plane.ClosestPointOnPlane(Positions[i]);
            float angle = Vector3.SignedAngle(projectedBone - Positions[i - 1], projectedPole - Positions[i - 1], plane.normal);
            Positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (Positions[i] - Positions[i - 1]) + Positions[i - 1];
        }

        //set position & rotation
        for (int i = 0; i < Positions.Length; i++)
        {
            if (i == Positions.Length - 1)
            {
                SetRotationRootSpace(Bones[i], Quaternion.Inverse(targetRotation) * StartRotationTarget * Quaternion.Inverse(StartRotationBone[i]));
            }
            else
            {
                SetRotationRootSpace(Bones[i], Quaternion.FromToRotation(StartDirectionSucc[i], Positions[i + 1] - Positions[i]) * Quaternion.Inverse(StartRotationBone[i]));
            }

            SetPositionRootSpace(Bones[i], Positions[i]);
        }
    }

    private Vector3 GetPositionRootSpace(Transform current) =>
        Quaternion.Inverse(Root.rotation) * (current.position - Root.position);

    private void SetPositionRootSpace(Transform current, Vector3 position) =>
        current.position = Root.rotation * position + Root.position;

    private Quaternion GetRotationRootSpace(Transform current) =>
        Quaternion.Inverse(current.rotation) * Root.rotation;

    private void SetRotationRootSpace(Transform current, Quaternion rotation) =>
        current.rotation = Root.rotation * rotation;

}