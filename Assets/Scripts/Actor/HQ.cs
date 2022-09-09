using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CMN;
using State = StateMachine<HQ>.State;

public class HQ : Enemy
{
    [SerializeField] FromTo teleportRange = default;

    private readonly int VelocityHash = Animator.StringToHash("Velocity");
    private readonly int IsGroundHash = Animator.StringToHash("IsGround");
    private readonly int IsAttackHash = Animator.StringToHash("IsAttack");

    private Animator animator;
    private FootCollider footCollider;
    private SpriteRenderer render;
    private StateMachine<HQ> stateMachine;


    //------------------------------------------
    // Unityランタイム
    //------------------------------------------
    new void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();
        footCollider = GetComponentInChildren<FootCollider>();

        stateMachine = new StateMachine<HQ>(this);
        stateMachine.AddTransition<StateLocomotion, StateAttack>((int)Event.Attack);
        stateMachine.AddTransition<StateAttack, StateTeleport>((int)Event.Teleport);
        stateMachine.AddTransition<StateAttack, StateLocomotion>((int)Event.Locomotion);
        stateMachine.AddTransition<StateTeleport, StateLocomotion>((int)Event.Locomotion);
        stateMachine.AddAnyTransition<StateDeath>(((int)Event.Death));
        stateMachine.Start<StateLocomotion>();
    }
    new void Update()
    {
        base.Update();
        stateMachine.Update();
        animator.SetBool(IsGroundHash, footCollider.IsGround);
    }
    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }


    //------------------------------------------
    // 継承先共有抽象関数
    //------------------------------------------
    protected override void OnDamage()
    {
        var sequence = DOTween.Sequence();
        sequence.
            Append(render.DOColor(Color.red, 0.05f)).
            Append(render.DOColor(Color.white, 0.05f));
        sequence.Play();
    }
    protected override void OnDeath()
    {
        base.OnDeath();
        stateMachine.Dispatch(((int)Event.Death));
    }


    //------------------------------------------
    // ステートマシン
    //------------------------------------------
    enum Event
    {
        Locomotion, Teleport, Attack, Death,
    }
    private class StateLocomotion : State
    {
        protected override void OnFixedUpdate()
        {
            float gravity = owner.footCollider.IsGround ? -1f : -9.81f;
            owner.ForceToPlayer(owner.data.Speed, gravity);
        }
        protected override void OnUpdate()
        {
            if (owner.IsInDistance(owner.data.AttackDistance))
            {
                stateMachine.Dispatch(((int)Event.Attack));
            }
            owner.RotateFwd();
            owner.animator.SetFloat(owner.VelocityHash, Mathf.Abs(owner.Rigid.velocity.x));
        }
    }
    private class StateTeleport : State
    {
        float elapseTime = 3f;
        Color transparent = new Color(1, 1, 1, 0);
        protected override void OnEnter(State prevState)
        {
            owner.animator.SetFloat(owner.VelocityHash, 0);
            owner.Rigid.velocity = Vector3.zero;
            owner.render.DOColor(transparent, elapseTime / 2).OnComplete(ExcuteTeleport);
        }
        protected override void OnFixedUpdate()
        {
            owner.ForceGravity(-9.81f);
        }
        protected override void OnUpdate()
        {
            owner.RotateFwd();
        }
        private void ExcuteTeleport()
        {
            if (owner.Playerform != null)
            {
                var rnd = Utility.CircleHorizon(owner.Playerform.position, owner.teleportRange.from, owner.teleportRange.to);
                rnd.y = Mathf.Clamp(rnd.y, 3, float.MaxValue);
                owner.transform.position = rnd;
            }
            owner.render.DOColor(Color.white, elapseTime / 2).OnComplete(TeleportCompleted);
        }
        private void TeleportCompleted()
        {
            stateMachine.Dispatch(((int)Event.Locomotion));
        }
    }
    private class StateAttack : State
    {
        float time = 0;
        float stopDistance = 10;
        protected override void OnEnter(State prevState)
        {
            time = 0;
            owner.animator.SetBool(owner.IsAttackHash, true);
            owner.Rigid.velocity = Vector2.zero;
        }
        protected override void OnFixedUpdate()
        {
            if (owner.IsInDistance(stopDistance))
            {
                owner.Rigid.velocity = Vector2.zero;
            }
            else
            {
                float gravity = owner.footCollider.IsGround ? -1f : -9.81f;
                owner.ForceToPlayer(owner.data.Speed, gravity);
            }
        }
        protected override void OnUpdate()
        {
            if (!owner.IsInDistance(owner.data.ChaseDistance))
            {
                stateMachine.Dispatch(((int)Event.Teleport));
            }
            else
            {
                owner.RotateFwd();
                owner.GapUpdateOnFire(ref time, owner.data.BulletDuration, owner.data.BulletSpeed, owner.data.BulletPower);
                owner.animator.SetFloat(owner.VelocityHash, Mathf.Abs(owner.Rigid.velocity.x));
            }
        }
        protected override void OnExit(State nextState)
        {
            owner.animator.SetBool(owner.IsAttackHash, false);
        }
    }
    private class StateDeath : State
    {
        protected override void OnEnter(State prevState)
        {
            owner.GetComponent<Collider2D>().enabled = false;
            owner.Rigid.velocity = Vector2.zero;
            owner.Rigid.isKinematic = true;
            owner.RequestDeathEffect();
        }
    }
}
