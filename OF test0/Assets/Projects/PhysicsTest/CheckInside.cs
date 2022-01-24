using UnityEngine;
namespace PhysicsTest
{
    public class CheckInside : MonoBehaviour
    {
        Collider m_collider;
        Transform point;
        Transform indicator;
        // Start is called before the first frame update
        void Start()
        {
            m_collider = GetComponent<Collider>();
            point = transform.GetChild( 0 );
            indicator = transform.GetChild( 1 );
        }

        void Update()
        {
            indicator.position = m_collider.ClosestPoint( point.position );
        }
    }
}
