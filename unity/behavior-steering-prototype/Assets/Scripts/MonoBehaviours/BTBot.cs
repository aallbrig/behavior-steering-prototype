using UnityEngine;

namespace MonoBehaviours
{
    // "BT Bot" stands for "Behavior Test Bot"
    public class BTBot : MonoBehaviour
    {
        [SerializeField] private GameObject _headRef;

        public Material HeadMaterial { get; private set; }

        public void ChangeHeadMaterial(Material material)
        {
            if (_headRef == null) Debug.LogError("BTBot: Head reference needs to be set");

            _headRef.GetComponent<Renderer>().material = material;
            HeadMaterial = material;
        }
    }
}