using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        private readonly float _detectRange = 7.0f;
        private IEnumerator _idleBehaviorCoroutine;
        private float _idleBehaviorInSeconds = ComputeIdleWaitTime();
        private readonly float _wanderRadius = 4.0f;
        private Vector3 _wanderTarget;
        public IdleState(Material stateMaterial) : base(stateMaterial) {}
        private static float ComputeIdleWaitTime() => 1.0f + Random.value * 5.0f;

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
                var randomWanderInterest = Random.insideUnitCircle * _wanderRadius;
                _wanderTarget = context.transform.position + new Vector3(randomWanderInterest.x, 0, randomWanderInterest.y);

                context.UpdateDestination(_wanderTarget);

                _idleBehaviorInSeconds = ComputeIdleWaitTime();
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
        }

        public override void OnDrawGizmos(ArtificialIntelligence context)
        {
            base.OnDrawGizmos(context);

            var position = context.gameObject.transform.position;

            // Draw aggro radius
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(position, _detectRange);

            // Draw current wander interest
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(position + Vector3.up, _wanderTarget - (position + Vector3.up));
            Gizmos.DrawSphere(_wanderTarget, 0.25f);
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
        private readonly float _chaseUpdateWaitInSeconds = 1.5f;
        private IEnumerator _chaseBehaviorCoroutine;
        private Vector3 _chaseTarget;
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
            else if (TargetOutOfChaseRange()) context.SetState(context._idleState);
            else if (TargetWithinCloseFollowRange()) context.SetState(context._closeFollowState);
        }

        public override void OnDrawGizmos(ArtificialIntelligence context)
        {
            base.OnDrawGizmos(context);

            var position = context.gameObject.transform.position;
            Gizmos.DrawRay(position + Vector3.up, _chaseTarget - (position + Vector3.up));
        }

        private bool TargetOutOfChaseRange() =>
            // Get the length of a ray between target and self.
            // Return if that length is greater than _closeFollowRange
            false;

        private bool TargetWithinCloseFollowRange() =>
            // Get the length of a ray between target and self.
            // Return if that length is greater than _closeFollowRange
            false;

        private IEnumerator ChaseBehavior(ArtificialIntelligence context)
        {
            while (true)
            {
                if (Target != null)
                {
                    _chaseTarget = Target.transform.position;
                    context.UpdateDestination(_chaseTarget);
                }

                yield return new WaitForSeconds(_chaseUpdateWaitInSeconds);
            }
        }
    }

    public class CloseFollowState : AiState
    {
        private IEnumerable<Vector3> _avoids = new List<Vector3>();
        private IEnumerable<Vector3> _interests = new List<Vector3>();
        public CloseFollowState(Material stateMaterial) : base(stateMaterial) {}
        public override void OnDrawGizmos(ArtificialIntelligence context) => base.OnDrawGizmos(context);
        public override void Update(ArtificialIntelligence context)
        {
            // Circle around player
            // Dot product between self and Target
            // https://youtu.be/6BrZryMz-ac?t=170
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

        [SerializeField] private Material idleMaterial;
        [SerializeField] private Material chaseMaterial;
        [SerializeField] private Material closeCombatMaterial;
        [SerializeField] private Material deadMaterial;

        private BTBot _btBot;
        public ChaseState _chasingState;
        public CloseFollowState _closeFollowState;
        public DeadState _deadState;
        public IdleState _idleState;
        private void Start()
        {
            _btBot = GetComponent<BTBot>();
            _idleState = new IdleState(idleMaterial);
            _chasingState = new ChaseState(chaseMaterial);
            _closeFollowState = new CloseFollowState(closeCombatMaterial);
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