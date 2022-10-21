using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skel : MonoBehaviour
{
    [Header("Vars")]
    public float currentSpeed;
    public float walkSpeed;
    public bool isFacingRight = true;
    public Rigidbody2D rb;
    public Animator animator;
    public Transform groundDetection;
    public int curHel;
    public int maxHel;
    public float seeingRange;

    public SkelStates states;

    public enum SkelStates
    {
        idle,
        walking,
        attacking
    }

    private void Start()
    {
        curHel = maxHel;
    }

    private void FixedUpdate()
    {
        Movement();
        CheckRot();
    }

    private void Update()
    {
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, 0.05f);
        if(groundInfo.collider == false)
        {
            if(isFacingRight)
            {
                isFacingRight = false;
            }
            else
            {
                isFacingRight = true;
            }
        }

        RaycastHit2D checkFront = Physics2D.Raycast(transform.position, transform.forward, seeingRange);
        if (checkFront.collider.tag == "Player")
        {
            StartCoroutine(lookingForPlayer());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            states = SkelStates.attacking;
            animator.SetTrigger("Attack");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            states = SkelStates.walking;
        }
    }

    IEnumerator lookingForPlayer()
    {
        if (isFacingRight)
        {
            isFacingRight = true;
        }
        else
        {
            isFacingRight = false;
        }

        yield return new WaitForSeconds(2f);

        states = SkelStates.idle;

        yield return new WaitForSeconds(2f);
    }

    void Movement()
    {
        if(states == SkelStates.walking)
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

        if(curHel <= 0)
        {
            Destroy(gameObject);
        }
    }
}
