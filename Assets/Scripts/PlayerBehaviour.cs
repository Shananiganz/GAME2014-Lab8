using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("Movement Properties")]
    public float horizontalForce;
    public float horizontalSpeed;
    public float verticalForce;
    public float airFactor;
    public Transform groundPoint;
    public float groundRadius;
    public LayerMask groundLayerMask;
    public bool isGrounded;

    [Header("Animations")]
    public Animator animator;
    public PlayerAnimationState playerAnimationState;

    [Header("Controls")]
    public Joystick leftStick;
    [Range(0.1f, 1.0f)]
    public float verticalThreshold;

    private Rigidbody2D rigidbody2D;

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        leftStick = (Application.isMobilePlatform) ? GameObject.Find("LeftStick").GetComponent<Joystick>() : null;
    }

    void FixedUpdate()
    {
        var hit = Physics2D.OverlapCircle(groundPoint.position, groundRadius, groundLayerMask);
        isGrounded = hit;

        Move();
        Jump();
        AirCheck();
    }

    private void Move()
    {
        var x = Input.GetAxisRaw("Horizontal") + ((Application.isMobilePlatform) ? leftStick.Horizontal : 0.0f);

        if(x != 0.0f)
        {
            Flip(x);

            x = (x > 0.0f) ? 1.0f : -1.0f; // Sanitizing X (limiting X to 1 or -1)

            rigidbody2D.AddForce(Vector2.right * x * horizontalForce * ((isGrounded) ? 1.0f : airFactor));

            var clampedXVelocity = Mathf.Clamp(rigidbody2D.velocity.x, -horizontalSpeed, horizontalSpeed);

            rigidbody2D.velocity = new Vector2(clampedXVelocity, rigidbody2D.velocity.y);

            ChangeAnimation(PlayerAnimationState.RUN);
        }

        if((isGrounded) && (x == 0.0f))
        {
            ChangeAnimation(PlayerAnimationState.IDLE);
        }
    }

    private void Jump()
    {
        var y = Input.GetAxis("Jump") + ((Application.isMobilePlatform) ? leftStick.Vertical : 0.0f);

        if((isGrounded) && (y > verticalThreshold))
        {
            rigidbody2D.AddForce(Vector2.up * verticalForce, ForceMode2D.Impulse);
        }
    }

    private void AirCheck()
    {
        if(!isGrounded)
        {
            ChangeAnimation(PlayerAnimationState.JUMP);
        }
    }

    public void Flip(float x)
    {
        if(x != 0.0f)
        {
            transform.localScale = new Vector3((x > 0.0f) ? 1.0f : -1.0f, 1.0f, 1.0f);
        }
    }

    private void ChangeAnimation(PlayerAnimationState animationState)
    {
        playerAnimationState = animationState;
        animator.SetInteger("AnimationState", (int)playerAnimationState);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundPoint.position, groundRadius);
    }
}
