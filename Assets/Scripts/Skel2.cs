using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skel2 : MonoBehaviour
{
    [Header("Vars")]
    public float currentSpeed;
    public float walkSpeed;
    public bool isFacingRight = true;
    public Rigidbody2D rb;
    public Animator animator;
    public Transform groundDetection;
    public Transform attackPoint;
    public Transform seeingTrans;
    public float seeingRange;
    public bool canSeePlayer;
    public LayerMask playerMask;
    public PlayerScript playerScript;

    [Header("Health")]
    public int curHel;
    public int maxHel;

    public SkelStates states;
    public enum SkelStates
    {
        idle,
        walking,
        attacking
    }

    private void Start()
    {
        playerScript = FindObjectOfType<PlayerScript>();
        states = SkelStates.walking;
        curHel = maxHel;
    }

    private void FixedUpdate()
    {
        Movement();
        CheckRot();
    }

    private void Update()
    {
        LayerMask mask = LayerMask.GetMask("Ground");

        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.right, 0.1f, mask);

        Debug.DrawRay(groundDetection.position, Vector2.right * 0.1f, Color.green);

        if (groundInfo.collider != null)
        {
            print("enemy is touching " + groundInfo.collider.name);

            if (isFacingRight)
            {
                isFacingRight = false;
            }
            else
            {
                isFacingRight = true;
            }
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            canSeePlayer = true;
            StartCoroutine(attack());
        }
        else
        {
            canSeePlayer = false;
            states = SkelStates.walking;
        }
    }

    IEnumerator attack()
    {
        states = SkelStates.attacking;
        animator.SetTrigger("Attack");
        rb.velocity = Vector3.zero;

        yield return new WaitForSeconds(2f);

        states = SkelStates.idle;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(attackPoint.position, 0.3f);
    }

    public void AttackPlayer()
    {
        Collider2D[] checkFront2 = Physics2D.OverlapCircleAll(attackPoint.position, 0.3f, playerMask);
        foreach (Collider2D col in checkFront2)
        {
            col.GetComponent<PlayerScript>().TakeDamage(1);
        }
    }

    void Movement()
    {
        if (states == SkelStates.walking)
        {
            currentSpeed = walkSpeed;
            animator.SetBool("Walking", true);
            rb.AddForce(transform.right * currentSpeed, ForceMode2D.Impulse);
        }
        else
        {
            currentSpeed = 0f;
            animator.SetBool("Walking", false);
        }
    }

    void CheckRot()
    {
        if (isFacingRight)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    public void TakeDamage(int dam)
    {
        curHel -= dam;

        if (curHel <= 0)
        {
            playerScript.AddHealth(3);
            Destroy(gameObject);
        }
    }
}
