using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using State = StateMachine<Player>.State;


public class Player : Actor, IItemReciever
{
    [SerializeField] AudioClip runSound = default;
    [SerializeField] AudioClip shootSound = default;
    [SerializeField] AudioClip damageSound = default;
    [SerializeField] AudioClip jumpSound = default;
    [SerializeField] AudioClip groundSound = default;
    [SerializeField] Transform tips = default;
    [SerializeField] FromTo screenRange = default;

    private AudioSource audioSource;
    private Animator animator;
    private SpriteRenderer render;
    private Rigidbody2D rigid;
    private FootCollider foot;
    private StateMachine<Player> stateMachine;

    private float bulletSpeed = 10;
    private float speed = 10;
    private float jumpSpeed = 25;
    private float gravity = -1;
    private float power = 5;
    private float maxBulletNum = 100;
    private float bulletNum;
    private int jumpCount = 0;
    private int jumpMaxTime = 3;
    private float shootTime;
    private float shootGap = 0.1f;
    private bool isJump = false;
    private bool isDash = false;

    private readonly int HorizontalHash = Animator.StringToHash("Horizontal");
    private readonly int RunSpeedHash = Animator.StringToHash("RunSpeed");
    private readonly int IsJumpHash = Animator.StringToHash("IsJump");
    private readonly int IsAttackHash = Animator.StringToHash("IsAttack");
    private readonly int IsRunningHash = Animator.StringToHash("IsRunning");


    //------------------------------------------
    // Unityランタイム
    //------------------------------------------
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        foot = GetComponentInChildren<FootCollider>();
        foot.OnGroundNotifyerHandler = OnGroundReciever;
    }
    private void Start()
    {
        hp = maxHP;
        bulletNum = maxBulletNum;

        stateMachine = new StateMachine<Player>(this);
        stateMachine.AddTransition<StateLocomotion, StateAttack>((int)Event.Attack);
        stateMachine.AddTransition<StateAttack, StateLocomotion>((int)Event.Locomotion);
        stateMachine.AddAnyTransition<StateDeath>(((int)Event.Death));
        stateMachine.Start<StateLocomotion>();
    }
    void Update()
    {
        stateMachine.Update();
        if (!IsDeath)
        {
            InScreen();
            animator.SetBool(IsJumpHash, !foot.IsGround);
            if (Input.GetKeyDown(KeyCode.Tab)) isDash = !isDash;
        }
    }
    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }


    //------------------------------------------
    // 外部共有関数
    //------------------------------------------
    public delegate void OnDeathNotifyer();
    public OnDeathNotifyer OnDeathNotifyerHandler;
    public float ClampBulletNum => bulletNum / maxBulletNum;
    public bool IsDash => isDash;


    //------------------------------------------
    // 内部共有関数
    //------------------------------------------
    private void RigidMove(float horizontal)
    {
        float dashAmount = isDash ? 1.5f : 1f;
        if (foot.IsGround)
        {
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
    private void ConformToJump()
    {
        if (jumpCount < jumpMaxTime - 1)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                audioSource.PlayOneShot(jumpSound);
                isJump = true;
            }
        }
        if (foot.IsGround)
        {
            jumpCount = 0;
        }
    }
    private void ConformForceToJump()
    {
        if (isJump)
        {
            rigid.velocity = transform.up * jumpSpeed;
            isJump = false;
            jumpCount++;
        }
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
            shootTime = shootGap;
        }
        if (Input.GetMouseButton(0))
        {
            if (bulletNum > 0)
            {
                shootTime += Time.deltaTime;
                if (shootTime >= shootGap)
                {
                    var bullet = Locator<BulletPool>.I.GetPlayerBullet();
                    if (bullet != null)
                    {
                        float eulerY = (transform.right.x == 1) ? 0 : 180;
                        var rotate = Quaternion.Euler(new Vector3(0, eulerY, 0));
                        float speed = bulletSpeed + Mathf.Abs(rigid.velocity.x);
                        var dir = transform.right * speed;
                        ExcuteBullet(bullet, tips.position, rotate, dir, power);

                        audioSource.PlayOneShot(shootSound);
                        bulletNum = Mathf.Clamp(bulletNum - 1, 0, int.MaxValue);
                        shootTime = 0;
                    }
                }
            }
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
    private void InScreen()
    {
        var rightTop = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        var leftTop = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
        var pos = transform.position;
        if(pos.x >= rightTop.x + screenRange.from)
        {
            pos.x = rightTop.x + screenRange.from;
        }
        if (pos.x <= leftTop.x + screenRange.to)
        {
            pos.x = leftTop.x + screenRange.to;
        }
        transform.position = pos;
    }


    //------------------------------------------
    // アニメーションイベント
    //------------------------------------------
    public void OnFootEvent()
    {
        audioSource.PlayOneShot(runSound);
    }


    //------------------------------------------
    // デリゲート通知
    //------------------------------------------
    private void OnGroundReciever()
    {
        audioSource.PlayOneShot(groundSound);
    }


    //------------------------------------------
    // インターフェイス
    //------------------------------------------
    public void ApplyRecover(float value)
    {
        hp = Mathf.Clamp(hp + value, 0, maxHP);
    }
    public void ApplyBullet(float num)
    {
        bulletNum = Mathf.Clamp(bulletNum + num, 0, maxBulletNum);
    }


    //------------------------------------------
    // 抽象関数
    //------------------------------------------
    public override void OnStateMachineExit()
    {
        stateMachine.Dispatch(((int)Event.Locomotion));
    }
    protected override void OnDamage()
    {
        Camera.main.DOShakePosition(0.05f, 0.5f);
        audioSource.PlayOneShot(damageSound);
        var sequence = DOTween.Sequence();
        sequence.
            Append(render.DOColor(Color.red, 0.05f)).
            Append(render.DOColor(Color.white, 0.05f));
        sequence.Play();
    }
    protected override void OnDeath()
    {
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
        float horizontal = default;

        protected override void OnFixedUpdate()
        {
            owner.ConformForceToJump();
            owner.RigidMove(horizontal);
        }
        protected override void OnUpdate()
        {
            horizontal = Input.GetAxis("Horizontal");
            if (Input.GetMouseButtonDown(0))
            {
                stateMachine.Dispatch(((int)Event.Attack));
            }

            owner.ConformToJump();
            owner.RotateFwd(horizontal);
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
            owner.ConformForceToJump();
            owner.RigidMove(horizontal);
        }
        protected override void OnUpdate()
        {
            horizontal = Input.GetAxis("Horizontal");

            owner.ConformToJump();
            owner.RotateFwd(horizontal);
            owner.OnShootFromButtonDown();
            owner.OnGroundThrowFromButtonDown();
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
            owner.rigid.velocity = Vector2.zero;
            var sequence = DOTween.Sequence();
            sequence.
                Append(owner.transform.DOScaleY(0, 0.2f)).
                Join(owner.render.DOColor(Color.red, 0.1f)).
                Append(owner.render.DOColor(Color.white, 0.1f));
            sequence.OnComplete(OnCompleted);
            sequence.Play();
        }
        private void OnCompleted()
        {
            owner.OnDeathNotifyerHandler?.Invoke();
        }
    }
}
