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
        private Camera _camera;

        private void Start()
        {
            _btBot = GetComponent<BTBot>();
            _camera = Camera.main;
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
            var position = _playerActions.Player.Position.ReadValue<Vector2>();
            Debug.Log("Interaction at position " + position);
            // HACK: initial tap/click is 0,0,0
            if (position == Vector2.zero) return;
            var ray = _camera.ScreenPointToRay(position);
            if (Physics.Raycast(ray, out var hit))
            {
                Debug.Log("Hit: " + hit);
                _btBot.SetDestination(hit.point);
            }
        }
    }
}