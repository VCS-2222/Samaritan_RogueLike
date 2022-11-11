using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    [Header("Factors of Health")]
    public int maxHealth;
    public int currentHealth;
    public bool canTakeDamage;
    public Slider slider;

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
    public bool isRolling;

    [Header("Needed components")]
    public Rigidbody2D rb;
    public bool isGrounded;
    public LayerMask groundMask;
    public Animator animator;
    public CapsuleCollider2D capsuleCollider;
    public bool facingRight = true;
    public BoxCollider2D shieldCol;

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

    [Header("Death")]
    public bool isDead = false;
    public CapsuleCollider2D capcal;

    private void Start()
    {
        currentHealth = maxHealth;
        canWalk = true;
        canRun = true;
        canJump = false;
        canAttack = true;
        currentSpeed = walkSpeed;
        hasWeaponOut = false;
        canTakeDamage = true;
        shieldCol.enabled = false;
        rb.simulated = true;
        capcal.enabled = true;
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
        if (isDead)
        {
            this.tag = "Untagged";
            rb.simulated = false;
            capcal.enabled = false;
            animator.Play("Player_death");
        }

        SetSliderHealth();

        if (isDead)
            return;

        PlayerInput();
        CheckForGround();
        CheckDirection();
        CheckForWeapon();
        Roll();
        Block();
        Kick();
        Jump();
        AnimatorHandler();
    }
    void FixedUpdate()
    {
        if(isDead)
            return;

        MovementRun();
        MovementWalk();
    }

    void PlayerInput()
    {
        if (Input.GetButton("Shift")) //shift
        {
            pressingShift = true;
        }
        else
        {
            pressingShift = false;
        }

        if (Input.GetButton("Right")) // rot
        {
            facingRight = true;
        }

        if (Input.GetButton("Left"))
        {
            facingRight = false;
        }

        if (Input.GetButton("Right") && canWalk && hasWeaponOut) // walking
        {
            state = PlayerStates.walking;
        }

        if (Input.GetButton("Left") && canWalk && hasWeaponOut)
        {
            state = PlayerStates.walking;
        }

        if (Input.GetButtonUp("Right") && isWalking || Input.GetButtonUp("Left") && isWalking)
        {
            state = PlayerStates.idle;
        }

        if (Input.GetButton("Right") && canRun && !hasWeaponOut && pressingShift) // running
        {
            state = PlayerStates.running;
        }

        if (Input.GetButton("Left") && canRun && !hasWeaponOut && pressingShift)
        {
            state = PlayerStates.running;
        }

        if (Input.GetButtonUp("Right") && isRunning || Input.GetButtonUp("Left") && isRunning)
        {
            state = PlayerStates.idle;
        }

        if(Input.GetButtonDown("RollKey") && isGrounded && canRoll && !hasWeaponOut) //roll
        {
            state = PlayerStates.roll;
        }

        if(Input.GetKey(KeyCode.Mouse1) && isGrounded && canBlock && !isRolling) //block
        {
            state = PlayerStates.block;
        }

        if (Input.GetKeyUp(KeyCode.Mouse1) && isGrounded && canBlock)
        {
            state = PlayerStates.idle;
        }

        if(Input.GetButtonDown("KickKey") && isGrounded && canKick && hasWeaponOut) //kick
        {
            animator.Play("Player_kick");
            state = PlayerStates.kick;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && isGrounded && hasWeaponOut)
        {
            animator.SetInteger("AttackIndex", Random.Range(0, availableAttacks));
            state = PlayerStates.attacking;
            isAttacking = true;
            currentSpeed = 0f;
            StartCoroutine(attack());
        }
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
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && canJump && !hasWeaponOut)
        {
            state = PlayerStates.jumping;
            isJumping = true;
            rb.AddForce(transform.up * jumpPower, ForceMode2D.Impulse);

            if (Input.GetButton("Right") && !pressingShift) // moving in air
            {
                rb.AddForce(transform.right * 80, ForceMode2D.Impulse);
            }

            if (Input.GetButton("Left") && !pressingShift)
            {
                rb.AddForce(transform.right * 80, ForceMode2D.Impulse);
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
            if(hit.gameObject.tag == "Skel")
            {
                hit.GetComponent<Skel>().TakeDamage(attackDam);
            }
            
            if(hit.gameObject.tag == "FinalSkel")
            {
                hit.GetComponent<FinalSkel>().TakeDamage(attackDam);
            }
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
            canJump = false;
        }
        else
        {
            animator.SetBool("hasWeaponOut", false);
            canRun = true;
            canWalk = false;
            canJump = true;
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
            animator.SetBool("Running", false);
            animator.SetBool("Walking", false);
            animator.SetBool("Jump", true);
        }
        else
        {
            animator.SetBool("Jump", false);
        }
    }

    public void TakeDamage(int damage)
    {
        if(canTakeDamage)
        {
            currentHealth -= damage;
        }

        if(currentHealth <= 0)
        {
            isDead = true;
        }
    }

    public void AddHealth( int health)
    {
        currentHealth += health;
        if(currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(attackPoint.position, attackRange);
    }

    void Roll()
    {
        if(state == PlayerStates.roll)
        {
            StartCoroutine(rolling());
            animator.Play("Player_roll_no_weapon");
        }
    }

    IEnumerator rolling()
    {
        state = PlayerStates.roll;
        rb.AddForce(transform.right * 1.2f, ForceMode2D.Impulse);
        Physics2D.IgnoreLayerCollision(3, 7, true);
        canTakeDamage = false;
        isRolling = true;
        canJump = false;

        yield return new WaitForSeconds(1.2f); 

        canTakeDamage = true;
        isRolling = false;
        rb.velocity = Vector3.zero;
        canJump = true;
        Physics2D.IgnoreLayerCollision(3, 7, false);
        state = PlayerStates.idle;
    }

    void Block()
    {
        if(state == PlayerStates.block)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            currentSpeed = 0;
            animator.SetBool("isBlocking", true);
            shieldCol.enabled = true;
            capsuleCollider.enabled = false;
            this.tag = "Untagged";
        }
        else
        {
            shieldCol.enabled = false;
            capsuleCollider.enabled = true;
            currentSpeed = walkSpeed;
            rb.isKinematic = false;
            animator.SetBool("isBlocking", false);
            this.tag = "Player";
        }
    }

    void Kick()
    {
        if(state == PlayerStates.kick)
        {
            rb.velocity = Vector3.zero;
            StartCoroutine(leKick());
        }
    }

    IEnumerator leKick()
    {
        yield return new WaitForSeconds(0.23f);

        state = PlayerStates.idle;
    }

    public void UseKickInAnim()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPoint.position, 0.05f, enemies);

        foreach (Collider2D enemy in colliders)
        {
            if (enemy.GetComponent<Skel>().isFacingRight)
                enemy.GetComponent<Rigidbody2D>().AddForce(transform.right * 270f, ForceMode2D.Impulse);
            else
                enemy.GetComponent<Rigidbody2D>().AddForce(transform.right * 270f, ForceMode2D.Impulse);
        }
    }

    public void UseShieldForceInAnim()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPoint.position, 0.05f, enemies);

        foreach (Collider2D enemy in colliders)
        {
            if (enemy.GetComponent<Skel>().isFacingRight)
                enemy.GetComponent<Rigidbody2D>().AddForce(transform.right * 270f, ForceMode2D.Impulse);
            else
                enemy.GetComponent<Rigidbody2D>().AddForce(transform.right * 270f, ForceMode2D.Impulse);
        }
    }

    public void SetSliderHealth()
    {
        slider.maxValue = maxHealth;
        slider.value = currentHealth;
    }
}
