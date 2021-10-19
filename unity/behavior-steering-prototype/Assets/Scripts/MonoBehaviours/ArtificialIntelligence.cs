using UnityEngine;

namespace MonoBehaviours
{
    public abstract class AiState {}

    public interface AiStateContext
    {
        public AiState CurrentAiState { get; }
        public void SetState(AiState nextAiState);
    }
    // FSM context
    public class ArtificialIntelligence : MonoBehaviour
    {
        [SerializeField] private Material idleMaterial;
        [SerializeField] private Material chaseMaterial;
        [SerializeField] private Material closeCombatMaterial;
        [SerializeField] private Material deadMaterial;

        private BTBot _btBot;
        private void Start()
        {
            _btBot = GetComponent<BTBot>();
            _btBot.ChangeHeadMaterial(idleMaterial);
        }
    }
}