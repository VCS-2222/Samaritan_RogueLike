using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("Factors of Health")]
    public int maxHealth;
    public int currentHealth;

    [Header("Factors of Logictics")]
    public float currentSpeed;
    public float walkSpeed;
    public float runSpeed;
    public float jumpPower;

    [Header("Bools of Actions")]
    public bool canJump;
    public bool canWalk;
    public bool canRun;
    public bool canAttack;
    public bool hasWeaponOut;
    public bool pressingShift;
    public bool canKick;
    public bool canBlock;
    public bool canRoll;
    public bool canAIR;

    [Header("Needed components")]
    public Rigidbody2D rb;
    public bool isGrounded;
    public LayerMask groundMask;
    public Animator animator;
    public CapsuleCollider2D capsuleCollider;
    public bool facingRight = true;

    [Header("Bools for Animator")]
    public bool isWalking;
    public bool isJumping;
    public bool isRunning;
    public bool isAttacking;

    [Header("Attack Stats")]
    public int attackDam;
    public float attackCD;
    public int availableAttacks = 1;
    public LayerMask enemies;
    public float attackRange;
    public Transform attackPoint;

    private void Start()
    {
        currentHealth = maxHealth;
        canWalk = true;
        canRun = true;
        canJump = true;
        canAttack = true;
        currentSpeed = walkSpeed;
        hasWeaponOut = false;
    }

    public PlayerStates state;

    public enum PlayerStates
    {
        idle,
        walking,
        running,
        jumping,
        roll,
        block,
        kick,
        attacking,
        damaging,
        attackInAir
    }

    public void Update()
    {
        if(Input.GetButton("Shift")) //shift
        {
            pressingShift = true;
        }
        else
        {
            pressingShift = false;
        }

        if(Input.GetButton("Right") && canWalk && hasWeaponOut) // walking
        {
            facingRight = true;
            state = PlayerStates.walking;
        }

        if(Input.GetButton("Left") && canWalk && hasWeaponOut)
        {
            facingRight = false;
            state = PlayerStates.walking;
        }

        if(Input.GetButtonUp("Right") && isWalking || Input.GetButtonUp("Left") && isWalking)
        {
            state = PlayerStates.idle;
        }

        if (Input.GetButton("Right") && canRun && !hasWeaponOut && pressingShift) // running
        {
            facingRight = true;
            state = PlayerStates.running;
        }

        if (Input.GetButton("Left") && canRun && !hasWeaponOut && pressingShift)
        {
            facingRight = false;
            state = PlayerStates.running;
        }

        if (Input.GetButtonUp("Right") && isRunning || Input.GetButtonUp("Left") && isRunning)
        {
            state = PlayerStates.idle;
        }

        if(Input.GetKeyDown(KeyCode.Space) && isGrounded && canJump) // jump
        {
            StartCoroutine(jumper());
        }

        if(Input.GetKeyDown(KeyCode.Mouse0) && isGrounded && hasWeaponOut)
        {
            animator.SetInteger("AttackIndex", Random.Range(0, availableAttacks));
            state = PlayerStates.attacking;
            isAttacking = true;
            currentSpeed = 0f;
            StartCoroutine(attack());
        }

        CheckForGround();
        CheckDirection();
        CheckForWeapon();
        AnimatorHandler();
    }

    IEnumerator jumper()
    {
        state = PlayerStates.jumping;

        yield return new WaitForSeconds(1f);

        state = PlayerStates.idle;
    }

    void FixedUpdate()
    {
        Jump();
        MovementRun();
        MovementWalk();
    }

    void MovementWalk()
    {
        if(state == PlayerStates.walking)
        {
            isWalking = true;
            currentSpeed = walkSpeed;
            rb.AddForce(transform.right * currentSpeed, ForceMode2D.Impulse);
        }
        else
        {
            isWalking = false;
        }
    }

    void MovementRun()
    {
        if (state == PlayerStates.running)
        {
            isRunning = true;
            currentSpeed = runSpeed;
            rb.AddForce(transform.right * currentSpeed, ForceMode2D.Impulse);
        }
        else
        {
            isRunning = false;
        }
    }

    void CheckDirection()
    {
        if(facingRight)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    void Jump()
    {
        if(state == PlayerStates.jumping)
        {
            isJumping = true;
            rb.AddForce(transform.up * jumpPower, ForceMode2D.Impulse);

            if (Input.GetButton("Right")) // walking
            {
                facingRight = true;
                rb.AddForce(transform.right * currentSpeed, ForceMode2D.Impulse);
            }

            if (Input.GetButton("Left"))
            {
                facingRight = false;
                rb.AddForce(transform.right * currentSpeed, ForceMode2D.Impulse);
            }
        }
        else
        {
            isJumping = false;
        }
    }

    IEnumerator attack()
    {
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackCD);

        state = PlayerStates.idle;
        isAttacking = false;
    }

    public void ExactAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemies);

        foreach (Collider2D hit in hitEnemies)
        {
            hit.GetComponent<Skel>().TakeDamage(attackDam);
        }
    }

    void CheckForWeapon()
    {
        if(Input.GetButtonDown("WeaponOut"))
        {
            hasWeaponOut = !hasWeaponOut;
        }

        if(hasWeaponOut)
        {
            animator.SetBool("hasWeaponOut", true);
            canRun = false;
            canWalk = true;
        }
        else
        {
            animator.SetBool("hasWeaponOut", false);
            canRun = true;
            canWalk = false;
        }

        if(isGrounded == false)
        {
            canJump = false;
        }

        if(hasWeaponOut && isGrounded)
        {
            canJump = false;
        }
        else if(!hasWeaponOut && isGrounded)
        {
            canJump = true;
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(attackPoint.position, attackRange);
    }

    void CheckForGround()
    {
        if(Physics2D.Raycast(capsuleCollider.bounds.center, Vector2.down, 0.4f, groundMask))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    public void AnimatorHandler()
    {
        if(isWalking == true)
        {
            animator.SetBool("Walking", true);
        }
        else
        {
            animator.SetBool("Walking", false);
        }

        if(isRunning == true)
        {
            animator.SetBool("Running", true);
            animator.SetBool("Walking", false);
        }
        else
        {
            animator.SetBool("Running", false);
        }

        if(isJumping)
        {
            animator.SetBool("Jump", true);
        }
        else if(!isJumping && isGrounded)
        {
            animator.SetBool("Jump", false);
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }
}
