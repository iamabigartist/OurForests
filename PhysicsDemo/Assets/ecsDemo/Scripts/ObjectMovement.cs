using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class ObjectMovement : MonoBehaviour
{
    //test 
    [SerializeField]
    bool Has_Player;
    //cube
    [SerializeField]
    private List<GameObject> CubesPrefab = new List<GameObject>();
    [SerializeField]
    private int Test_Num;
    [SerializeField]
    private int least_x;
    [SerializeField]
    private int most_x;
    [SerializeField]
    private int least_z;
    [SerializeField]
    private int most_z;

    //player
    [SerializeField]
    GameObject PlayerPrefab;

    [SerializeField]
    private float m_Gravitational_Acceleration = -9.8f;

    //jump
    bool Is_Jump = true;
    [SerializeField]
    float Jump_Frozen_Constant;
    float Jump_Frozen_Time;
    [SerializeField]
    private float m_Jump_Speed;

    //move
    float m_horizontalMovementInput;
    float m_verticalMovementInput;
    [SerializeField]
    float MovemnentSpeed;

    //test
    [SerializeField]
    float skinWidth;

    //Floor
    private float m_FloorNum;
    private NativeArray<Vector3> m_FloorPosition;
    private NativeArray<Vector3> m_FloorScalar;

    [SerializeField]
    private List<GameObject> Floor = new List<GameObject>();

    public TransformAccessArray m_TransformsAccessArray;
    public NativeArray<Vector3> m_Velocities;
    private NativeArray<Vector3> m_Acceleration;

    private MoveObjectJob m_Job;
    private JobHandle m_PositionJobHandle;

    public struct MoveObjectJob : IJobParallelForTransform
    {
        public NativeArray<Vector3> velocity;
        public float deltaTime;

        [ReadOnly]
        public bool Is_jump;
        [ReadOnly]
        public float Gravitational_Acceleration;
        [ReadOnly]
        public float Floor_Num;

        [ReadOnly]
        public NativeArray<Vector3> Floor_Position;
        [ReadOnly]
        public NativeArray<Vector3> Floor_Scalar;
        public void Execute(int index, TransformAccess transform)
        {
            Vector3 m_velocity = velocity[index];
            //drop when the height > 0

            int NP;
            //stay still if the object is grounded
            m_velocity.y += 10 * Gravitational_Acceleration * deltaTime;
            velocity[index] = m_velocity;
            float groundCheckDistance = m_velocity.y * deltaTime;
            NP = (groundCheckDistance > 0) ? 1 : -1;
            for (int i = 0; i < Floor_Num; ++i)
            {
                if(FindCertainFloor(transform, Floor_Position[i], Floor_Scalar[i]))
				{
                    if (IsGrounded(groundCheckDistance, transform, Floor_Position[i], Floor_Scalar[i], NP))
                    {
                        m_velocity.y = 0f;
                        velocity[index] = m_velocity;
                        Vector3 m_position = transform.position;
                        m_position.y = Floor_Position[i].y + (transform.localScale.y * 0.5f + Floor_Scalar[i].y * 0.5f)*(-NP);
                        transform.position = m_position;
                        break;
                    }
                }
            }

            //move right
            float rightCheckDistance = m_velocity.x * deltaTime;
            NP = (rightCheckDistance > 0) ? 1 : -1;
            for (int i = 0; i < Floor_Num; ++i)
            {
                if (FindCollisionRight(transform, Floor_Position[i], Floor_Scalar[i]))
                {
                    if (IsCollidedRight(rightCheckDistance, transform, Floor_Position[i], Floor_Scalar[i], NP))
                    {
                        m_velocity.x = 0f;
                        velocity[index] = m_velocity;
                        Vector3 m_position = transform.position;
                        m_position.x = Floor_Position[i].x + (transform.localScale.x * 0.5f +  Floor_Scalar[i].x * 0.5f)*(-NP);
                        transform.position = m_position;
                        break;
                    }
                }
            }

            //move forward
            float ForwardCheckDistance = m_velocity.z * deltaTime;
            NP = (ForwardCheckDistance > 0) ? 1 : -1;
            for (int i = 0; i < Floor_Num; ++i)
            {
                if (FindCollisionForward(transform, Floor_Position[i], Floor_Scalar[i]))
                {
                    if (IsCollidedForward(ForwardCheckDistance, transform, Floor_Position[i], Floor_Scalar[i], NP))
                    {
                        m_velocity.z = 0f;
                        velocity[index] = m_velocity;
                        Vector3 m_position = transform.position;
                        m_position.z = Floor_Position[i].z + (transform.localScale.z * 0.5f + Floor_Scalar[i].z * 0.5f)*(-NP);
                        transform.position = m_position;
                        break;
                    }
                }
            }

            transform.position += velocity[index] * deltaTime;
            
        }

        //isGround
        bool IsGrounded(float groundCheckDistance, TransformAccess Player_transform, Vector3 Floor_Position, Vector3 Floor_Scale, int NP)
        {
            bool IsGround = false;
           
            // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
            if (Player_transform.position.y - Floor_Position.y <= Player_transform.localScale.y * 0.5f + Floor_Scale.y * 0.5f + Mathf.Abs(groundCheckDistance) && NP < 0 && Player_transform.position.y >= Floor_Position.y)
            {
                IsGround = true;   
            }
            else if(Floor_Position.y - Player_transform.position.y <= Player_transform.localScale.y * 0.5f + Floor_Scale.y * 0.5f + Mathf.Abs(groundCheckDistance) && NP > 0 && Floor_Position.y >= Player_transform.position.y)
			{
                IsGround = true;
            }
            return IsGround;
        }


        bool FindCertainFloor(TransformAccess Player_transform, Vector3 Floor_Position, Vector3 Floor_Scale)
        {
            return ((Player_transform.position.x > Floor_Position.x - Floor_Scale.x * 0.5f) && (Player_transform.position.x < Floor_Position.x + Floor_Scale.x * 0.5f)
                && (Player_transform.position.z > Floor_Position.z - Floor_Scale.z * 0.5f) && (Player_transform.position.z < Floor_Position.z + Floor_Scale.z * 0.5f));

        }

        bool IsCollidedRight(float groundCheckDistance, TransformAccess Player_transform, Vector3 Floor_Position, Vector3 Floor_Scale, int NP)
        {
            bool IsCollided = false;
            // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
            if (Player_transform.position.x - Floor_Position.x <= Player_transform.localScale.x * 0.5f + Floor_Scale.x * 0.5f + Mathf.Abs(groundCheckDistance) && Player_transform.position.x >= Floor_Position.x && NP < 0)
            {

                IsCollided = true;
            }
            else if (Floor_Position.x - Player_transform.position.x <= Player_transform.localScale.x * 0.5f + Floor_Scale.x * 0.5f + Mathf.Abs(groundCheckDistance) && Floor_Position.x >= Player_transform.position.x && NP > 0)
            {

                IsCollided = true;
            }
            return IsCollided;
        }

        bool FindCollisionRight(TransformAccess Player_transform, Vector3 Floor_Position, Vector3 Floor_Scale)
        {
            return ((Player_transform.position.y > Floor_Position.y - Floor_Scale.y * 0.5f) && (Player_transform.position.y < Floor_Position.y + Floor_Scale.y * 0.5f)
                && (Player_transform.position.z > Floor_Position.z - Floor_Scale.z * 0.5f) && (Player_transform.position.z < Floor_Position.z + Floor_Scale.z * 0.5f));
        }

        bool IsCollidedForward(float groundCheckDistance, TransformAccess Player_transform, Vector3 Floor_Position, Vector3 Floor_Scale, int NP)
        {
            bool IsCollided = false;

            // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
            if (Player_transform.position.z - Floor_Position.z <= Player_transform.localScale.z * 0.5f + Floor_Scale.z * 0.5f + Mathf.Abs(groundCheckDistance) && Player_transform.position.z >= Floor_Position.z && NP < 0)
            {

                IsCollided = true;
            }
            else if (Floor_Position.z - Player_transform.position.z <= Player_transform.localScale.z * 0.5f + Floor_Scale.z * 0.5f + Mathf.Abs(groundCheckDistance) && Floor_Position.z >= Player_transform.position.z && NP > 0)
            {

                IsCollided = true;
            }
            return IsCollided;
        }


        bool FindCollisionForward(TransformAccess Player_transform, Vector3 Floor_Position, Vector3 Floor_Scale)
        {
            return ((Player_transform.position.x > Floor_Position.x - Floor_Scale.x * 0.5f) && (Player_transform.position.x < Floor_Position.x + Floor_Scale.x * 0.5f)
                && (Player_transform.position.y > Floor_Position.y - Floor_Scale.y * 0.5f) && (Player_transform.position.y < Floor_Position.y + Floor_Scale.y * 0.5f));

        }
    }

    private void Start()
    {
        m_FloorNum = Floor.Count;

        m_Acceleration = new NativeArray<Vector3>(Test_Num, Allocator.Persistent);
        m_Velocities = new NativeArray<Vector3>(Test_Num, Allocator.Persistent);
        m_FloorPosition = new NativeArray<Vector3>(Floor.Count, Allocator.Persistent);
        m_FloorScalar = new NativeArray<Vector3>(Floor.Count, Allocator.Persistent);

        var transform = new Transform[Test_Num];
        GameObject Object;
        if (Has_Player)
		{
            Object = Instantiate(PlayerPrefab);
            Object.transform.position = new Vector3(0, 3, 0);
            transform[0] = Object.transform;
        }
        //instantiate cube
        for (int i = 1; i < Test_Num; ++i)
        { 
            Object = Instantiate(CubesPrefab[Random.Range(0, CubesPrefab.Count)]);
            Object.transform.position = new Vector3(Random.Range(least_x, most_x), 30, Random.Range(least_z, most_z));
           
            transform[i] = Object.transform;

        }
        m_TransformsAccessArray = new TransformAccessArray(transform);

        int index = 0;
        foreach(GameObject i in Floor){
            m_FloorPosition[index] = i.transform.position;
            m_FloorScalar[index] = i.transform.localScale;
            index++;
		}
    }

    private void Update()
    {
        if (Has_Player)
        {
            Jump_Frozen_Time -= Time.deltaTime;
            Is_Jump = false;
            //player
            Vector3 m_MovementVelocity = m_Velocities[0];
            //jump
            if (Input.GetButtonDown("Jump") && Jump_Frozen_Time <= 0)
            {
                Is_Jump = true;
                Jump_Frozen_Time = Jump_Frozen_Constant;
                m_MovementVelocity.y = m_Jump_Speed;
            }
            else
            {
                m_MovementVelocity.y = 0;
            }
            //movement input
            m_horizontalMovementInput = Input.GetAxis("Horizontal");
            m_verticalMovementInput = Input.GetAxis("Vertical");
            m_MovementVelocity.x = m_horizontalMovementInput * MovemnentSpeed;
            m_MovementVelocity.z = m_verticalMovementInput * MovemnentSpeed;
            m_Velocities[0] = m_MovementVelocity;
        }

        m_Job = new MoveObjectJob()
        {
            deltaTime = Time.deltaTime,
            velocity = m_Velocities,
            Gravitational_Acceleration = m_Gravitational_Acceleration,
            Floor_Num = m_FloorNum,
            Is_jump = Is_Jump,
            Floor_Position = m_FloorPosition,
            Floor_Scalar = m_FloorScalar
            
        };

        m_PositionJobHandle = m_Job.Schedule(m_TransformsAccessArray);

    }

    private void LateUpdate()
    {
        m_PositionJobHandle.Complete();
    }

    private void OnDestroy()
    {
        m_Acceleration.Dispose();
        m_Velocities.Dispose();
        m_FloorPosition.Dispose();
        m_FloorScalar.Dispose();
        m_TransformsAccessArray.Dispose();
    }

    


}
