using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using State = StateMachine<Cop>.State;

public class Cop : Enemy
{
    private readonly int VelocityHash = Animator.StringToHash("Velocity");
    private readonly int IsAttackHash = Animator.StringToHash("IsAttack");

    private Animator animator;
    private FootCollider footCollider;
    private StateMachine<Cop> stateMachine;


    //------------------------------------------
    // Unityランタイム
    //------------------------------------------
    new void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        footCollider = GetComponentInChildren<FootCollider>();
        stateMachine = new StateMachine<Cop>(this);

        stateMachine.AddTransition<StateLocomotion, StateAttack>((int)Event.Attack);
        stateMachine.AddTransition<StateAttack, StateLocomotion>((int)Event.Locomotion);
        stateMachine.AddAnyTransition<StateDeath>(((int)Event.Death));
        stateMachine.Start<StateLocomotion>();
    }
    new void Update()
    {
        base.Update();
        stateMachine.Update();
    }
    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }


    //------------------------------------------
    // 継承先共有抽象関数
    //------------------------------------------
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
        Locomotion, Attack, Death
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
    private class StateAttack : State
    {
        float time = 0;
        protected override void OnEnter(State prevState)
        {
            time = 0;
            owner.animator.SetBool(owner.IsAttackHash, true);
            owner.Rigid.velocity = Vector2.zero;
        }
        protected override void OnFixedUpdate()
        {
            owner.ForceGravity(-9.81f);
        }
        protected override void OnUpdate()
        {
            owner.RotateFwd();
            if (!owner.IsInDistance(owner.data.ChaseDistance))
            {
                stateMachine.Dispatch(((int)Event.Locomotion));
            }
            else
            {
                owner.GapUpdateOnFire(ref time, owner.data.BulletDuration, owner.data.BulletSpeed, owner.data.BulletPower);
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
