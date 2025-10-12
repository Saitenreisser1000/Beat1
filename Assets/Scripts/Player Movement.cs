using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // === Einstellungsmöglichkeiten im Inspector ===
    [Header("Bewegung")]
    [SerializeField] private float speed = 5f;

    [SerializeField] private AudioClip jumpSound;
    private AudioSource audioSource;

    [Header("Kopf-Check für Ducking")]
    [SerializeField] private float headCheckHeight = 1.2f;
    [SerializeField] private float headCheckRadius = 0.2f;

    // === Interne Referenzen & Zustände ===
    private Rigidbody2D body;
    private Animator anim;
    private bool grounded;
    private bool isFacingRight = true;

    // Für DPad-Eingabevergleich
    private Vector2 lastDPad = Vector2.zero;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        HandleMovementInput();
        HandleDuckAndCrawlInput();
        HandleJumpInput();
        UpdateAnimatorStates();
    }

    private void HandleMovementInput()
    {
        float horizontalRaw = Input.GetAxis("Horizontal");
        float horizontal = Mathf.Abs(horizontalRaw) < 0.05f ? 0f : horizontalRaw;
        bool crawling = anim.GetBool("crawling");

        float currentSpeed = crawling ? speed * 0.4f : speed;
        body.linearVelocity = new Vector2(horizontal * currentSpeed, body.linearVelocity.y);

        if (horizontal > 0.01f)
            isFacingRight = true;
        else if (horizontal < -0.01f)
            isFacingRight = false;

        anim.SetBool("facingright", isFacingRight);
        anim.SetBool("run", Mathf.Abs(horizontal) > 0.01f && !crawling && grounded);
        float normalizedSpeed = Mathf.Abs(body.linearVelocity.x) / speed;
        anim.SetFloat("runspeed", normalizedSpeed);
    }

    private void HandleDuckAndCrawlInput()
    {
        bool down = Input.GetKey(KeyCode.DownArrow) || Input.GetAxis("Vertical") < -0.5f;
        float horizontal = Input.GetAxis("Horizontal");
        bool ducking = down || (anim.GetBool("ducking") && !CanStandUp());

        bool crawling = ducking && Mathf.Abs(horizontal) > 0.05f;

        anim.SetBool("ducking", ducking);
        anim.SetBool("crawling", crawling);
    }

    private bool CanStandUp()
    {
        Vector2 checkPosition = (Vector2)transform.position + Vector2.up * headCheckHeight;
        return !Physics2D.OverlapCircle(checkPosition, headCheckRadius, LayerMask.GetMask("Ground"));
    }

    private void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump") && grounded && body.linearVelocity.y <= 0.01f)
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (jumpSound != null)
            audioSource.PlayOneShot(jumpSound);

        body.linearVelocity = new Vector2(body.linearVelocity.x, speed * 2.2f);
        grounded = false;
        StartCoroutine(PlayJumpAnimation());
    }

    private IEnumerator PlayJumpAnimation()
    {
        anim.SetBool("jumping", true);
        yield return new WaitForSeconds(0.04f);
        anim.SetBool("jumping", false);
    }

    private void UpdateAnimatorStates()
    {
        anim.SetBool("grounded", grounded);
        anim.SetBool("falling", !grounded && body.linearVelocity.y < -0.1f);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        grounded = false;
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
                grounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        grounded = false;
    }

    // Sprite-Wechsel für Themenwechsel im Editormode
    public void ChangeSprite(Sprite idleBeat)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && idleBeat != null)
        {
            spriteRenderer.sprite = idleBeat;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 checkPosition = (Vector2)transform.position + Vector2.up * headCheckHeight;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(checkPosition, headCheckRadius);
    }
}