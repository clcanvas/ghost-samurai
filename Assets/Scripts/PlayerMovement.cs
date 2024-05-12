using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Dashing")]
    [SerializeField] private float speed = 10f;
    [SerializeField] public float Range = 3f;
    [SerializeField] private float maxDashTime = 0.4f;
    private Rigidbody2D rb;
    [HideInInspector] public bool isDashing = false;
    private Vector3 initialDashPos;
    private Vector3 targetDashPosition;
    [SerializeField] private AudioSource sfx;
    private Animator animate;
    private Animator camAnimate;

    [Header("UI Elements")]
    [SerializeField] private Transform optionalCursor;
    [SerializeField] private Transform dashHintPivot;
    [SerializeField] private Transform dashHintCursor;
    [SerializeField] private Transform enemyCursor;
    [SerializeField] private float distBtwArrows;
    [SerializeField] private LayerMask validCollisions;
    private Camera mainCam;
    private int numOfArrows;
    private GameObject[] pointList;
    private Vector3 defaultArrowSize;

    // MISC
    private float playerScaleX;

    private void Start()
    {
        animate = GetComponentInChildren<SpriteRenderer>().gameObject.GetComponent<Animator>();
        numOfArrows = (int)(Range / distBtwArrows);
        pointList = new GameObject[numOfArrows];
        for (int i = 0; i < numOfArrows; i++)
        {
            pointList[i] = Instantiate(dashHintCursor, dashHintPivot).gameObject;
        }

        defaultArrowSize = pointList[0].transform.localScale;

        Cursor.visible = false;
        mainCam = Camera.main;
        playerScaleX = transform.localScale.x;
        rb = gameObject.GetComponent<Rigidbody2D>();
        camAnimate = mainCam.GetComponent<Animator>();
    }

    private void Update()
    {
        if(!PauseManager.isGamePaused) {
            //Controls dashing anim
            animate.SetBool("isDashing", isDashing);
            //Gets the mouse mosition
            Vector3 cursorPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            cursorPos.z = transform.position.z; // also prevents disappearing, most prob due to z pos being behind camera
            //Code for player dashing
            if (Input.GetMouseButtonDown(0) && !isDashing)
            {
                initialDashPos = transform.position;
                targetDashPosition = cursorPos;
                //targetDashPosition = hintController.DashHintCursor.position;
                targetDashPosition.z = transform.position.z; //Solves wierd problem with player disappearing
                StopAllCoroutines();
                StartCoroutine(EndDash());
                sfx.Play();
                isDashing = true;
            }
            if (optionalCursor != null)
            {
                // extra cursor, we can use unity built in cursor settings as well just used as quick demo
                optionalCursor.position = cursorPos;
            }

            RaycastHit2D[] mouseCollisions = Physics2D.RaycastAll(dashHintPivot.position, (cursorPos - dashHintPivot.position).normalized, Range + 1f, layerMask: validCollisions);
            Vector2 closestPoint = cursorPos;
            float closestPointDistance = Mathf.Clamp(AbsDistance(dashHintPivot.position, closestPoint), 0, Range);
            foreach (RaycastHit2D col in mouseCollisions)
            {

                if (col.collider.gameObject != gameObject && AbsDistance(dashHintPivot.position, col.point) <= closestPointDistance && !col.collider.gameObject.CompareTag("Enemy"))
                {
                    closestPoint = col.point;
                    closestPointDistance = Mathf.Clamp(AbsDistance(dashHintPivot.position, closestPoint), 0, Range);
                }

            }

            Vector2 direction = dashHintPivot.position - cursorPos;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 180;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            dashHintPivot.rotation = rotation;

            float distFromFirst = 0;
            float sizeMultiplier = .85f;
            float currentSize = sizeMultiplier;

            dashHintPivot.position = transform.position;
            foreach (GameObject arrow in pointList)
            {
                distFromFirst++;
                arrow.transform.localScale = defaultArrowSize;
                Vector3 nextCursorPos = dashHintCursor.position + (cursorPos - dashHintPivot.position).normalized * distFromFirst * distBtwArrows;
                if (AbsDistance(nextCursorPos, transform.position) - .5f > AbsDistance(closestPoint, transform.position))
                {
                    arrow.SetActive(false);
                }
                else if (!isDashing)
                {
                    arrow.SetActive(true);
                    arrow.transform.position = nextCursorPos;
                    arrow.transform.localScale *= currentSize;
                    currentSize *= sizeMultiplier;
                }
            }



            // two variables to make code a bit easier to read/write, essentially same checks
            bool reachedTargetPos = !isDashing || (isDashing && Mathf.Abs(Vector3.Distance(targetDashPosition, transform.position)) < .7f);
            bool reachedMaxDistance = AbsDistance(initialDashPos, transform.position) >= Range;

            if (isDashing)
            {
                rb.velocity = (targetDashPosition - transform.position).normalized * speed;
                if (reachedTargetPos || reachedMaxDistance)
                {
                    isDashing = false;
                }
            }
            else
            {
                float scaleX = optionalCursor.position.x < transform.position.x ? -playerScaleX : playerScaleX;
                transform.localScale = new Vector2(scaleX, transform.localScale.y);
                rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(0, 0), 40 * Time.deltaTime);
            }
        }
    }

    public float AbsDistance(Vector2 start, Vector2 end) // make code easier to read
    {
        return Mathf.Abs(Vector2.Distance(start, end));
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.gameObject.tag != "Enemy")
        {
            isDashing = false;
        }
        else if (isDashing)
        {
            other.collider.gameObject.GetComponent<Enemy>().Damage();
            camAnimate.SetTrigger("Shake");
        }
    }

    void OnCollisionStay2D(Collision2D other)
    {
        if (other.collider.gameObject.tag == "Enemy" && isDashing)
        {
            other.collider.gameObject.GetComponent<Enemy>().Damage();
            camAnimate.SetTrigger("Shake");
        }
    }

    private IEnumerator EndDash()
    {
        yield return new WaitForSeconds(maxDashTime);
        if (isDashing)
        {
            isDashing = false;
        }
    }

    public void Death()
    {
        camAnimate.SetTrigger("Shake");
        StartCoroutine(reloadScene());
        GetComponentInChildren<SpriteRenderer>().gameObject.GetComponent<Animator>().SetBool("isDead", true);
        rb.velocity = Vector2.zero;
        GetComponent<CapsuleCollider2D>().enabled = false;
        enabled = false;
    }

    IEnumerator reloadScene()
    {
        yield return new WaitForSeconds(.1f);
        FindObjectOfType<LevelManager>().ReloadLevel();
    }

}