using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // === Einstellungsmöglichkeiten im Inspector ===
    [Header("Bewegung")]
    [SerializeField] private float speed = 5f;

    [Header("Skins")]
    [SerializeField] private AnimatorOverrideController punkController;
    [SerializeField] private AnimatorOverrideController trachtController;
    [SerializeField] private AnimatorOverrideController klassikController;
    [SerializeField] private AnimatorOverrideController reggaeController;
    [SerializeField] private AnimatorOverrideController metalController;

    // Referenz zum TileSkinManager für den Skinwechsel
    [SerializeField] private TileSkinManager tileSkinManager;

    [SerializeField] private AudioClip jumpSound;
    private AudioSource audioSource;

    [Header("Kopf-Check für Ducking")]
    [SerializeField] private float headCheckHeight = 1.2f;
    [SerializeField] private float headCheckRadius = 0.2f;

    // === Interne Referenzen & Zustände ===
    private AnimatorOverrideController originalController;
    private Rigidbody2D body;
    private Animator anim;
    private string currentSkin = "beat";
    private bool grounded;
    private bool isFacingRight = true;

    // Für DPad-Eingabevergleich
    private Vector2 lastDPad = Vector2.zero;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        originalController = new AnimatorOverrideController(anim.runtimeAnimatorController);
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        HandleMovementInput();
        HandleJumpInput();
        HandleDuckAndCrawlInput();
        UpdateAnimatorStates();
        HandleSkinChange();
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
        if (Input.GetButton("Jump") && grounded && body.linearVelocity.y <= 0.01f)
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

    // === Skinwechsel mit Priorisierung und Anti-Doppeltrigger ===
    private void HandleSkinChange()
    {
        Vector2 currentDPad = new Vector2(
            Input.GetAxisRaw("DPadHorizontal"),
            Input.GetAxisRaw("DPadVertical")
        );

        // Metal (↗ oder Taste 6)
        if (Input.GetKeyDown(KeyCode.Alpha6) ||
                 (currentDPad.x > 0.5f && currentDPad.y > 0.5f &&
                  !(lastDPad.x > 0.5f && lastDPad.y > 0.5f)))
        {
            TryChangeSkin("metal", metalController, new Color(0.3f, 0.3f, 0.3f));
        }
        // Reggae (↖ oder Taste 5)
        else if (Input.GetKeyDown(KeyCode.Alpha5) ||
            (currentDPad.x < -0.5f && currentDPad.y > 0.5f &&
             !(lastDPad.x < -0.5f && lastDPad.y > 0.5f)))
        {
            TryChangeSkin("reggae", reggaeController, new Color(0.8f, 0.7f, 0.4f));
        }
        // Beat (↑ oder Taste 1)
        else if (Input.GetKeyDown(KeyCode.Alpha1) ||
                 (currentDPad.y > 0.5f && !(lastDPad.y > 0.5f)))
        {
            TryChangeSkin("beat", originalController, new Color(0.5f, 0.5f, 0.5f));
        }
        // Tracht (← oder Taste 3)
        else if (Input.GetKeyDown(KeyCode.Alpha3) ||
                 (currentDPad.x < -0.5f && !(lastDPad.x < -0.5f)))
        {
            TryChangeSkin("tracht", trachtController, new Color(0.55f, 0.8f, 0.5f));
        }
        // Punk (→ oder Taste 2)
        else if (Input.GetKeyDown(KeyCode.Alpha2) ||
                 (currentDPad.x > 0.5f && !(lastDPad.x > 0.5f)))
        {
            TryChangeSkin("punk", punkController, new Color(0.8f, 0.3f, 0.3f));
        }
        // Klassik (↓ oder Taste 4)
        else if (Input.GetKeyDown(KeyCode.Alpha4) ||
                 (currentDPad.y < -0.5f && !(lastDPad.y < -0.5f)))
        {
            TryChangeSkin("klassik", klassikController, new Color(0.75f, 0.55f, 0.85f));
        }
      
        lastDPad = currentDPad; // DPad-Zustand für nächsten Frame merken
    }

    private void TryChangeSkin(string skinName, AnimatorOverrideController controller, Color backgroundColor)
    {
        if (currentSkin == skinName) return;

        ChangeSkin(controller, backgroundColor);
        FindFirstObjectByType<MusicManager>().PlayScratchAndChangeMusic(skinName);
        tileSkinManager.ApplySkin(char.ToUpper(skinName[0]) + skinName.Substring(1));
        currentSkin = skinName;
    }

    private void ChangeSkin(AnimatorOverrideController newController, Color backgroundColor)
    {
        bool ducking = anim.GetBool("ducking");
        bool crawling = anim.GetBool("crawling");
        bool groundedNow = grounded;
        bool facing = isFacingRight;

        AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
        float normalizedTime = currentState.normalizedTime;

        anim.runtimeAnimatorController = newController;
        anim.Rebind();
        anim.SetBool("facingright", facing);
        anim.SetBool("ducking", ducking);
        anim.SetBool("crawling", crawling);
        anim.SetBool("grounded", groundedNow);

        anim.Play(currentState.shortNameHash, 0, normalizedTime);
        Camera.main.backgroundColor = backgroundColor;
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 checkPosition = (Vector2)transform.position + Vector2.up * headCheckHeight;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(checkPosition, headCheckRadius);
    }
}