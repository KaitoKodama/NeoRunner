using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CMN;
using State = StateMachine<Police>.State;

public class Police : Enemy
{
    [SerializeField] FromTo range = default;
    private SpriteRenderer render;
    private StateMachine<Police> stateMachine;
    private bool isStopAttack = false;


    //------------------------------------------
    // Unityランタイム
    //------------------------------------------
    new void Start()
    {
        base.Start();
        render = GetComponent<SpriteRenderer>();
        stateMachine = new StateMachine<Police>(this);

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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            render.color = Color.gray;
            isStopAttack = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            render.color = Color.white;
            isStopAttack = false;
        }
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
        Locomotion, Attack, Death
    }
    private class StateLocomotion : State
    {
        Vector3 circle, dir;
        float accept = 1f;
        protected override void OnEnter(State prevState)
        {
            circle = Utility.CircleHorizon(owner.Playerform.position, owner.range.from, owner.range.to);
            dir = (circle - owner.transform.position).normalized;
        }
        protected override void OnFixedUpdate()
        {
            owner.Force(dir, owner.data.Speed);
        }
        protected override void OnUpdate()
        {
            owner.RotateFwd();
            float distance = Vector2.Distance(owner.transform.position, circle);
            if (distance <= accept)
            {
                stateMachine.Dispatch(((int)Event.Attack));
            }
        }
    }
    private class StateAttack : State
    {
        Vector3 circle, dir;
        float time = 0;
        float accept = 1f;
        protected override void OnEnter(State prevState)
        {
            time = 0;
            circle = Utility.CircleHorizon(owner.Playerform.position, owner.range.from, owner.range.to);
            dir = (circle - owner.transform.position).normalized;
        }
        protected override void OnFixedUpdate()
        {
            owner.Force(dir, owner.data.Speed);
        }
        protected override void OnUpdate()
        {
            owner.RotateFwd();
            float distance = Vector2.Distance(owner.transform.position, circle);
            if (distance <= accept)
            {
                stateMachine.Dispatch(((int)Event.Locomotion));
            }
            else
            {
                if (!owner.isStopAttack)
                {
                    owner.GapUpdateOnFire(ref time, owner.data.BulletDuration, owner.data.BulletSpeed, owner.data.BulletPower);
                }
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
