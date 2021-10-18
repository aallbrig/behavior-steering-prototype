using UnityEngine;

namespace MonoBehaviours
{
    [RequireComponent(typeof(BTBot))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private Material playerControlledMaterial;

        private BTBot _btBot;

        private void Start()
        {
            _btBot = GetComponent<BTBot>();
            if (_btBot == null) Debug.LogError("Player: BT Bot is not found");
            SetPlayerControlledMaterial(playerControlledMaterial);
        }

        private void SetPlayerControlledMaterial(Material material)
        {
            if (_btBot == null) Debug.LogError("Player: BT Bot is not found");
            _btBot.ChangeHeadMaterial(material);
        }
    }
}