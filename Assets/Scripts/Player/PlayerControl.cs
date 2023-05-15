using System.Collections;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed;
    public LayerMask solidObjectLayer;
    public LayerMask grassLayer;
    public ParticleSystem encounter;
    public Grid world;
    private BattleSystem battleSystemReal;
    public GameObject battleSystemGO;
    public AudioClip transitionSound;
    public Canvas startScreen;

    public event Action OnEncountered;

    private bool isMoving;
    private Vector2 input;
    private AudioSource audioSource;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            startScreen.enabled = false;
            startScreen.GetComponent<AudioSource>().mute = true;
            world.GetComponent<AudioSource>().mute = false;
        }
        else if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);
                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if (IsWalkable(targetPos))
                {
                    StartCoroutine(Move(targetPos));
                }
            }
        }


        animator.SetBool("isMoving", isMoving);
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) 
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        isMoving = false;

        CheckForEncounters();
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos,0.1f,solidObjectLayer) != null)
        {
            return false;
        }
        return true;
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position,0.1f,grassLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 10) 
            {
                encounter.Play();
                animator.SetBool("encounter", true);
                world.GetComponent<AudioSource>().mute = true;
                audioSource.PlayOneShot(transitionSound);
                Invoke("runChangeScene",.5f);
            }
        }
    }
    private void runChangeScene()
    {
        world.enabled = false;
        battleSystemGO.SetActive(true);
        animator.SetBool("encounter", false);
        animator.SetBool("isMoving", false);
        OnEncountered();
    }
}
