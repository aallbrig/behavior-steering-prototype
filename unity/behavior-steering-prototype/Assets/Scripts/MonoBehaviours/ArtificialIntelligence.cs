using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MonoBehaviours
{
    public abstract class AiState
    {
        private readonly Material _stateMaterial;
        protected AiState(Material stateMaterial) => _stateMaterial = stateMaterial;

        // Common behavior
        public virtual void Start(ArtificialIntelligence context) => context.ChangeMaterial(_stateMaterial);
        public virtual void Stop(ArtificialIntelligence context)
        {
            // Do nothing
        }
        public abstract void Update(ArtificialIntelligence context);
        public virtual void OnDrawGizmos(ArtificialIntelligence context)
        {
            // Do nothing
        }
    }

    public class IdleState : AiState
    {
        private float _detectRange = 10.0f;
        private IEnumerator _idleBehaviorCoroutine;
        private readonly float _idleBehaviorInSeconds = 1.0f + Random.value * 5.0f;
        public IdleState(Material stateMaterial) : base(stateMaterial) {}
        public override void Start(ArtificialIntelligence context)
        {
            base.Start(context);
            _idleBehaviorCoroutine = IdleBehavior(context);
            context.StartCoroutine(_idleBehaviorCoroutine);
        }
        private IEnumerator IdleBehavior(ArtificialIntelligence context)
        {
            yield return new WaitForSeconds(_idleBehaviorInSeconds);
        }
        public override void Stop(ArtificialIntelligence context)
        {
            base.Stop(context);
            context.StopCoroutine(_idleBehaviorCoroutine);
        }
        public override void Update(ArtificialIntelligence context)
        {
            var player = DetectPlayer();
            if (player != null)
            {
                context._chasingState.Target = player;
                context.SetState(context._chasingState);
            }
            // else if (TimeToWander()) current time is >= the amount of time to wait since last decided to wander
        }
        private GameObject DetectPlayer() => null;
    }

    public class ChaseState : AiState
    {
        private IEnumerator _chaseBehaviorCoroutine;
        private readonly float _chaseUpdateWaitInSeconds = 2.0f;
        private float _inCombatRange = 2.0f;

        private float _outOfRange = 10.0f;
        public ChaseState(Material stateMaterial) : base(stateMaterial) {}

        public GameObject Target { get; set; }

        public override void Start(ArtificialIntelligence context)
        {
            base.Start(context);
            _chaseBehaviorCoroutine = ChaseBehavior(context);
            context.StartCoroutine(_chaseBehaviorCoroutine);
        }

        public override void Stop(ArtificialIntelligence context) => context.StopCoroutine(_chaseBehaviorCoroutine);

        public override void Update(ArtificialIntelligence context)
        {
            // Is _target null? setstate(Idle)
            if (Target == null) context.SetState(context._idleState);
            else if (TargetOutOfRange()) context.SetState(context._idleState);
            else if (TargetWithinRange()) context.SetState(context._closeCombatState);
        }

        private bool TargetOutOfRange() => false;

        private bool TargetWithinRange() => false;

        private IEnumerator ChaseBehavior(ArtificialIntelligence context)
        {
            if (Target == null) yield break;

            context.UpdateDestination(Target.transform.position);

            yield return new WaitForSeconds(_chaseUpdateWaitInSeconds);
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

        private BTBot _btBot;
        public ChaseState _chasingState;
        public CombatCloseState _closeCombatState;
        public DeadState _deadState;

        public IdleState _idleState;
        private void Start()
        {
            _btBot = GetComponent<BTBot>();
            _idleState = new IdleState(idleMaterial);
            _chasingState = new ChaseState(chaseMaterial);
            _closeCombatState = new CombatCloseState(closeCombatMaterial);
            _deadState = new DeadState(deadMaterial);
            SetState(_idleState);
        }
        private void Update() => CurrentAiState.Update(this);
        private void OnDrawGizmos() => CurrentAiState.OnDrawGizmos(this);

        public AiState CurrentAiState { get; private set; }

        public void SetState(AiState nextAiState)
        {
            CurrentAiState?.Stop(this);
            CurrentAiState = nextAiState;
            CurrentAiState.Start(this);
        }

        public void UpdateDestination(Vector3 position) => _btBot.SetDestination(position);

        public void ChangeMaterial(Material material) => _btBot.ChangeHeadMaterial(material);
    }
}