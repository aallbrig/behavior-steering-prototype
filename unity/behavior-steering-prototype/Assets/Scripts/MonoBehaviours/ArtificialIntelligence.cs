using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MonoBehaviours
{
    public interface AiStateContext
    {
        public AiState CurrentAiState { get; }

        public void SetState(AiState nextAiState);
    }

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
        private float _wanderRadius = 4.0f;
        public IdleState(Material stateMaterial) : base(stateMaterial) {}

        public override void Start(ArtificialIntelligence context)
        {
            base.Start(context);
            _idleBehaviorCoroutine = IdleBehavior(context);
            context.StartCoroutine(_idleBehaviorCoroutine);
        }

        private IEnumerator IdleBehavior(ArtificialIntelligence context)
        {
            while (true)
            {
                var randomWanderDirection = Random.insideUnitCircle * _wanderRadius;
                var destination = context.transform.position + new Vector3(randomWanderDirection.x, 0, randomWanderDirection.y);

                context.UpdateDestination(destination);

                yield return new WaitForSeconds(_idleBehaviorInSeconds);
            }
        }

        public override void Stop(ArtificialIntelligence context)
        {
            base.Stop(context);
            context.StopCoroutine(_idleBehaviorCoroutine);
        }
        public override void Update(ArtificialIntelligence context)
        {
            var player = DetectPlayer(context);
            if (player != null)
            {
                Debug.Log("Player detected!");
                context._chasingState.Target = player;
                context.SetState(context._chasingState);
            }
            // else if (TimeToWander()) current time is >= the amount of time to wait since last decided to wander
        }

        public override void OnDrawGizmos(ArtificialIntelligence context)
        {
            base.OnDrawGizmos(context);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(context.gameObject.transform.position, _detectRange);
        }

        private GameObject DetectPlayer(ArtificialIntelligence context)
        {
            GameObject player = null;
            // Draw circle around gameObject. If player detected, return
            var hitColliders = Physics.OverlapSphere(context.gameObject.transform.position, _detectRange);
            foreach (var hitCollider in hitColliders)
            {
                var hitColliderGameObject = hitCollider.gameObject;
                player = hitColliderGameObject.GetComponent<Player>()?.gameObject;
                if (player != null) break;
            }

            return player;
        }
    }

    public class ChaseState : AiState
    {
        private IEnumerator _chaseBehaviorCoroutine;
        private readonly float _chaseUpdateWaitInSeconds = 2.0f;
        public ChaseState(Material stateMaterial) : base(stateMaterial) {}

        // Promote up to abstract class? 🤔
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
            while (true)
            {
                if (Target != null)
                {
                    context.UpdateDestination(Target.transform.position);
                }

                yield return new WaitForSeconds(_chaseUpdateWaitInSeconds);
            }
        }
    }

    public class CombatCloseState : AiState
    {
        public CombatCloseState(Material stateMaterial) : base(stateMaterial) {}
        public override void Update(ArtificialIntelligence context)
        {
            // Circle around player
            // Dot product between self and Target
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

    // FSM context
    public class ArtificialIntelligence : MonoBehaviour, AiStateContext
    {
        public ChaseState _chasingState;
        public CombatCloseState _closeCombatState;
        public DeadState _deadState;
        public IdleState _idleState;

        [SerializeField] private Material idleMaterial;
        [SerializeField] private Material chaseMaterial;
        [SerializeField] private Material closeCombatMaterial;
        [SerializeField] private Material deadMaterial;

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
        private void Update() => CurrentAiState?.Update(this);
        private void OnDrawGizmos() => CurrentAiState?.OnDrawGizmos(this);

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