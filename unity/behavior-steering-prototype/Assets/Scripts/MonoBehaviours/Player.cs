using UnityEngine;
using UnityEngine.InputSystem;

namespace MonoBehaviours
{
    [RequireComponent(typeof(BTBot))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private Material playerControlledMaterial;

        private BTBot _btBot;
        private PlayerActionMap _playerActions;

        private void Start()
        {
            _btBot = GetComponent<BTBot>();
            SetPlayerControlledMaterial(playerControlledMaterial);
            BindControls();
        }
        private void Awake() => _playerActions = new PlayerActionMap();
        private void OnEnable() => _playerActions.Enable();
        private void OnDisable() => _playerActions.Disable();

        private void SetPlayerControlledMaterial(Material material)
        {
            if (_btBot == null) Debug.LogError("Player: BT Bot is not found");
            _btBot.ChangeHeadMaterial(material);
        }

        private void BindControls()
        {
            _playerActions.Player.Interaction.started += HandleInteraction;
        }

        private void HandleInteraction(InputAction.CallbackContext obj)
        {
            Debug.Log("Interaction at position " + _playerActions.Player.Position.ReadValue<Vector2>());
        }
    }
}