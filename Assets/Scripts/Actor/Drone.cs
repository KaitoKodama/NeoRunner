using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using State = StateMachine<Drone>.State;

public class Drone : Enemy
{
    [SerializeField] private float speed = 5f;
    [SerializeField] float bulletSpeed = 5f;
    [SerializeField] float power = 5f;
    [SerializeField] float attackDuration = 0.5f;
    [SerializeField] float attackBeginDistance;
    [SerializeField] float chaseBeginDistance;

    private SpriteRenderer render;
    private StateMachine<Drone> stateMachine;
    private float freezeBrokenCount = 0f;
    private float freezeBrokenTime = 0.2f;
    private bool isBroken = false;
    private bool isFreeze = false;


    //------------------------------------------
    // Unityランタイム
    //------------------------------------------
    new void Start()
    {
        base.Start();
        render = GetComponent<SpriteRenderer>();
        stateMachine = new StateMachine<Drone>(this);

        stateMachine.AddTransition<StateLocomotion, StateAttack>((int)Event.Attack);
        stateMachine.AddTransition<StateAttack, StateLocomotion>((int)Event.Locomotion);
        stateMachine.AddAnyTransition<StateDeath>(((int)Event.Death));
        stateMachine.Start<StateLocomotion>();
    }
    private void Update()
    {
        stateMachine.Update();
        if (isFreeze && !isBroken)
        {
            freezeBrokenCount += Time.deltaTime;
            if (freezeBrokenCount >= freezeBrokenTime)
            {
                isBroken = true;
                stateMachine.Dispatch(((int)Event.Death));
            }
        }
    }
    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            render.DOColor(Color.red, freezeBrokenTime);
            isFreeze = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            freezeBrokenCount = 0;
            render.DOColor(Color.white, freezeBrokenTime);
            isFreeze = false;
        }
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
            if (!owner.isFreeze)
            {
                owner.ForceToPlayer(owner.speed);
            }
            else
            {
                owner.Rigid.velocity = Vector2.zero;
            }
        }
        protected override void OnUpdate()
        {
            if (!owner.isFreeze)
            {
                owner.RotateFwd();
            }
            if (owner.IsInDistance(owner.attackBeginDistance))
            {
                stateMachine.Dispatch(((int)Event.Attack));
            }
        }
    }
    private class StateAttack : State
    {
        float time = 0;
        protected override void OnEnter(State prevState)
        {
            time = 0;
            owner.Rigid.velocity = Vector2.zero;
        }
        protected override void OnUpdate()
        {
            owner.RotateFwd();
            if (!owner.IsInDistance(owner.chaseBeginDistance))
            {
                stateMachine.Dispatch(((int)Event.Locomotion));
            }
            else
            {
                owner.GapUpdateOnFire(ref time, owner.attackDuration, owner.bulletSpeed, owner.power);
            }
        }
    }
    private class StateDeath : State
    {
        protected override void OnEnter(State prevState)
        {
            owner.GetComponent<Collider2D>().enabled = false;
            owner.Rigid.gravityScale = 0.4f;
            owner.RequestDeathEffect();
        }
    }
}
