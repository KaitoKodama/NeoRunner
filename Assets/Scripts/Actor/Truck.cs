using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using State = StateMachine<Truck>.State;

public class Truck : Enemy
{
    private SpriteRenderer render;
    private StateMachine<Truck> stateMachine;


    //------------------------------------------
    // Unityランタイム
    //------------------------------------------
    new void Start()
    {
        base.Start();
        render = GetComponent<SpriteRenderer>();
        stateMachine = new StateMachine<Truck>(this);

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
        Locomotion, Death
    }
    private class StateLocomotion : State
    {
        protected override void OnFixedUpdate()
        {
            owner.Force(Vector2.left, owner.data.Speed);
        }
    }
    private class StateDeath : State
    {
        protected override void OnEnter(State prevState)
        {
            owner.GetComponent<Collider2D>().enabled = false;
            owner.Rigid.gravityScale = 0.4f;
            owner.RequestDeathEffect(80f);
        }
    }
}
