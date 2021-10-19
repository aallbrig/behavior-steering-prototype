using UnityEngine;

namespace MonoBehaviours
{
    public abstract class AiState
    {
        private readonly Material _stateMaterial;
        protected AiState(Material stateMaterial)
        {
            _stateMaterial = stateMaterial;
        }

        public void Start(ArtificialIntelligence context)
        {
            context.ChangeMaterial(_stateMaterial);
        }
        public abstract void Update(ArtificialIntelligence context);
    }

    public class IdleState : AiState
    {
        private float _detectRange = 10.0f;
        public IdleState(Material stateMaterial) : base(stateMaterial) {}
        public override void Update(ArtificialIntelligence context)
        {
            // 
        }
    }

    public class ChaseState : AiState
    {
        private float _outOfRange = 10.0f;
        private float _inCombatRange = 2.0f;
        public ChaseState(Material stateMaterial) : base(stateMaterial) {}
        public override void Update(ArtificialIntelligence context)
        {
            // Navigate to player
        }
    }

    public class CombatCloseState : AiState
    {
        private float _outOfRange = 10.0f;
        public CombatCloseState(Material stateMaterial) : base(stateMaterial) {}
        public override void Update(ArtificialIntelligence context)
        {
            // Circle around player
        }
    }

    public class DeadState : AiState
    {
        public DeadState(Material stateMaterial) : base(stateMaterial) {}
        public override void Update(ArtificialIntelligence context)
        {
            // Do nothing
        }
    }

    public interface AiStateContext
    {
        public AiState CurrentAiState { get; }
        public void SetState(AiState nextAiState);
    }
    // FSM context
    public class ArtificialIntelligence : MonoBehaviour, AiStateContext
    {
        [SerializeField] private Material idleMaterial;
        [SerializeField] private Material chaseMaterial;
        [SerializeField] private Material closeCombatMaterial;
        [SerializeField] private Material deadMaterial;

        private AiState _idleState;
        private AiState _chasingState;
        private AiState _closeCombatState;
        private AiState _deadState;

        private BTBot _btBot;
        private void Start()
        {
            _btBot = GetComponent<BTBot>();
            _idleState = new IdleState(idleMaterial);
            _chasingState = new ChaseState(chaseMaterial);
            _closeCombatState = new CombatCloseState(closeCombatMaterial);
            _deadState = new DeadState(deadMaterial);
            SetState(_idleState);
        }

        public AiState CurrentAiState { get; private set; }

        public void SetState(AiState nextAiState)
        {
            CurrentAiState = nextAiState;
            CurrentAiState.Start(this);
        }

        public void ChangeMaterial(Material material) => _btBot.ChangeHeadMaterial(material);
        public void Update()
        {
            CurrentAiState.Update(this);
        }
    }
}