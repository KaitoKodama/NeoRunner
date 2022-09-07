using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using State = StateMachine<Player>.State;


public class Player : Actor
{
    [SerializeField] Transform tips = default;
    [SerializeField] float bulletSpeed;
    [SerializeField] float speed, maxSpeed, jumpSpeed, gravity, power;
    private Animator animator;
    private Rigidbody2D rigid;
    private FootCollider foot;
    private StateMachine<Player> stateMachine;

    
    private readonly int HorizontalHash = Animator.StringToHash("Horizontal");
    private readonly int RunSpeedHash = Animator.StringToHash("RunSpeed");
    private readonly int IsJumpHash = Animator.StringToHash("IsJump");
    private readonly int IsAttackHash = Animator.StringToHash("IsAttack");
    private readonly int IsRunningHash = Animator.StringToHash("IsRunning");


    //------------------------------------------
    // Unityランタイム
    //------------------------------------------
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        foot = GetComponentInChildren<FootCollider>();

        stateMachine = new StateMachine<Player>(this);
        stateMachine.AddTransition<StateLocomotion, StateAttack>((int)Event.Attack);
        stateMachine.AddTransition<StateLocomotion, StateDamage>((int)Event.Damage);
        stateMachine.AddTransition<StateAttack, StateLocomotion>((int)Event.Locomotion);
        stateMachine.AddTransition<StateDamage, StateLocomotion>((int)Event.Locomotion);
        stateMachine.AddAnyTransition<StateDeath>(((int)Event.Death));
        stateMachine.Start<StateLocomotion>();
    }
    void Update()
    {
        stateMachine.Update();
        animator.SetBool(IsJumpHash, !foot.IsGround);
    }
    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }


    //------------------------------------------
    // 内部共有関数
    //------------------------------------------
    private void RigidMove(float horizontal)
    {
        float dashAmount = 1;
        if (foot.IsGround)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                dashAmount = 1.5f;
            }
            animator.SetFloat(RunSpeedHash, dashAmount);

            float velocity = Mathf.Clamp(rigid.velocity.x, -1, 1);
            animator.SetFloat(HorizontalHash, velocity);

            bool hasInput = Mathf.Abs(velocity) != 0;
            animator.SetBool(IsRunningHash, hasInput);
        }

        float x = horizontal * (speed * dashAmount);
        float y = rigid.velocity.y + gravity;
        var force = new Vector2(x, y);
        rigid.velocity = force;
    }
    private void RotateFwd(float side)
    {
        if (side < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (side > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }
    private void OnShootFromButtonDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            float eulerY = (transform.right.x == 1) ? 0 : 180;
            var rotate = Quaternion.Euler(new Vector3(0, eulerY, 0));
            float speed = bulletSpeed + Mathf.Abs(rigid.velocity.x);
            var dir = transform.right * speed;
            ExcuteBullet(tips, rotate, dir, power);
        }
    }
    private void OnGroundThrowFromButtonDown()
    {
        if (foot.IsThrowGround)
        {
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                var pos = transform.position;
                pos.y -= 2f;
                transform.position = pos;
            }
        }
    }


    //------------------------------------------
    // 抽象関数
    //------------------------------------------
    public override void OnStateMachineExit()
    {
        stateMachine.Dispatch(((int)Event.Locomotion));
    }


    //------------------------------------------
    // ステートマシン
    //------------------------------------------
    enum Event
    {
        Locomotion, Attack, Damage, Death
    }
    private class StateLocomotion : State
    {
        float horizontal = default;
        int jumpCount = 0;
        int jumpMaxTime = 3;
        bool isJump = false;

        protected override void OnFixedUpdate()
        {
            if (isJump)
            {
                owner.rigid.velocity = owner.transform.up * owner.jumpSpeed;
                isJump = false;
                jumpCount++;
            }
            owner.RigidMove(horizontal);
        }
        protected override void OnUpdate()
        {
            horizontal = Input.GetAxis("Horizontal");
            owner.RotateFwd(horizontal);

            //ジャンプ
            if (jumpCount < jumpMaxTime - 1 && Input.GetKeyDown(KeyCode.Space))
            {
                isJump = true;
            }
            //接地とリセット
            if (owner.foot.IsGround)
            {
                jumpCount = 0;
            }
            //攻撃遷移
            if (Input.GetMouseButtonDown(0))
            {
                stateMachine.Dispatch(((int)Event.Attack));
            }

            owner.OnGroundThrowFromButtonDown();
            owner.OnShootFromButtonDown();
        }
    }
    private class StateAttack : State
    {
        float horizontal = default;

        protected override void OnEnter(State prevState)
        {
            owner.animator.SetBool(owner.IsAttackHash, true);
        }
        protected override void OnFixedUpdate()
        {
            owner.RigidMove(horizontal);
        }
        protected override void OnUpdate()
        {
            horizontal = Input.GetAxis("Horizontal");
            owner.RotateFwd(horizontal);
            owner.OnShootFromButtonDown();
            owner.OnGroundThrowFromButtonDown();
        }
        protected override void OnExit(State nextState)
        {
            owner.animator.SetBool(owner.IsAttackHash, false);
        }
    }
    private class StateDamage : State { }
    private class StateDeath : State { }
}
