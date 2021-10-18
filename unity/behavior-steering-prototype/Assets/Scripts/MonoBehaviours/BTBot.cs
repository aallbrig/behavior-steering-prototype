using UnityEngine;
using UnityEngine.AI;

namespace MonoBehaviours
{
    // "BT Bot" stands for "Behavior Test Bot"
    [RequireComponent(typeof(NavMeshAgent))]
    public class BTBot : MonoBehaviour
    {
        [SerializeField] private GameObject _headRef;
        private NavMeshAgent _navMeshAgent;

        public Material HeadMaterial { get; private set; }

        public void ChangeHeadMaterial(Material material)
        {
            if (_headRef == null) Debug.LogError("BTBot: Head reference needs to be set");

            _headRef.GetComponent<Renderer>().material = material;
            HeadMaterial = material;
        }

        public void SetDestination(Vector3 position)
        {
            if (_navMeshAgent == null) return;
            _navMeshAgent.SetDestination(position);
        }

        private void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }
    }
}