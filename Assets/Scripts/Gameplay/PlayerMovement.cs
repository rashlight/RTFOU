using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerMovement : MonoBehaviour
{
    public float forwardSpeed;
    public float backwardSpeed;
    public float translateCooldown;
    public float jumpForce;
    public float extraJumpForce;
    public float nudgeForce;
    public float jumpCooldown;
    public float fallMultiplier;
    public float lowJumpMultiplier;
    public Color nudgeColor;
    public GameObject nudgeObjectUp;
    public GameObject nudgeObjectDown;
    private float translateCooldownTimer;
    private float jumpCooldownTimer;
    private float nudgeLimit;
    private bool isGrounded;
    private bool isOverheat;
    [SerializeField] private AudioSource JumpSound;
    [SerializeField] private AudioSource nudgeSound;
    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private SpriteRenderer viewpointRenderer;
    [HideInInspector] public Rigidbody2D rb;
    private Player playerHandler;
    private BoxCollider2D col;
    private Coroutine overheatCoroutine;

    private void Start()
    {
        col = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        playerHandler = GetComponent<Player>();
    }

    bool IsGrounded() {
        RaycastHit2D hitGround = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, Vector2.down, 0.02f, GroundLayer);

        if (hitGround.collider != null) {
           return true;        
        }      
    
        return false;
    }

    void MovePlayer()
    {
        if (translateCooldownTimer > 0f)
        {
            translateCooldown = Mathf.Clamp(translateCooldownTimer - Time.fixedDeltaTime, 0f, Mathf.Infinity);
            return;
        }

        float x = Input.GetAxis("Horizontal");

        float moveVelocity = x;

        if (x >= 0)
        {
            moveVelocity *= forwardSpeed;
        }
        else
        {
            moveVelocity *= backwardSpeed;
        }

        rb.velocity = new Vector2(moveVelocity, rb.velocity.y);      
    }

    void JumpPlayer()
    {
        if (jumpCooldownTimer > 0f)
        {
            jumpCooldownTimer = Mathf.Clamp(jumpCooldownTimer - Time.fixedDeltaTime, 0f, Mathf.Infinity);
            return;
        }

        float jumpVelocity = 0f;

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            jumpVelocity = jumpForce;
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
            jumpCooldownTimer = jumpCooldown;
            isGrounded = false;
            JumpSound.Play();
        }    
    }

    private void NudgePlayer()
    {
        float yAxis = Input.GetAxisRaw("Vertical");

        if (!Input.GetButton("Jump") && yAxis != 0f && !IsGrounded() && !isOverheat)
        {
            if (yAxis > 0f)
            {
                rb.velocity += new Vector2(0f, yAxis * nudgeForce * (2f - nudgeLimit) * Time.deltaTime);
                nudgeLimit += 1.25f * Time.deltaTime;
                nudgeSound.pitch = 2f;
            }
            else if (yAxis < 0f)
            {
                rb.velocity += new Vector2(0f, yAxis * nudgeForce * Time.deltaTime);
                nudgeLimit += 0.45f * Time.deltaTime;
                nudgeSound.pitch = 1f;
            }

            nudgeObjectDown.SetActive(yAxis > 0f);
            nudgeObjectUp.SetActive(yAxis < 0f);

            if (!nudgeSound.isPlaying)
            {
                nudgeSound.Play();
            }
        }
        else  
        {
            nudgeObjectDown.SetActive(false);
            nudgeObjectUp.SetActive(false);
            if (nudgeSound.isPlaying) nudgeSound.Stop();
        }

        if (nudgeLimit > 0f) 
        {
            nudgeLimit = Mathf.Clamp01(nudgeLimit - Time.deltaTime / 50f);
            if (isOverheat)
            {
                if (Mathf.Approximately(nudgeLimit, 0f))
                {
                    StopCoroutine(overheatCoroutine);
                    viewpointRenderer.color = Color.white;
                    isOverheat = false;
                }
            }
            else
            {
                if (Mathf.Approximately(nudgeLimit, 1f))
                {
                    overheatCoroutine = StartCoroutine(NudgeOverHeat());
                    isOverheat = true;
                }
                else
                {
                    viewpointRenderer.color = new Color(
                        Mathf.Lerp(1f, nudgeColor.r, nudgeLimit), 
                        Mathf.Lerp(1f, nudgeColor.g, nudgeLimit), 
                        Mathf.Lerp(1f, nudgeColor.b, nudgeLimit)
                    );
                }
            }
        }
    }

    private void Update()
    {
        MovePlayer();
        JumpPlayer();
        NudgePlayer();
    }

    private void FixedUpdate()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }

    IEnumerator NudgeOverHeat()
    {
        while (true)
        {
            viewpointRenderer.color = Color.white;
            yield return new WaitForSeconds(0.5f);
            viewpointRenderer.color = new Color(
                nudgeColor.r,
                nudgeColor.g,
                nudgeColor.b
            );
            yield return new WaitForSeconds(0.5f);
        }
    }

    public IEnumerator SlowTimeAfterDeathSequence(float slowMultiplier)
    {
        float defaultFixedDeltaTime = Time.fixedDeltaTime;
        Coroutine slowPlayer = StartCoroutine(SlowInterpolation(slowMultiplier, 0.3f, defaultFixedDeltaTime));
        while (!IsGrounded()) yield return new WaitForEndOfFrame();
        StopCoroutine(slowPlayer);
        StartCoroutine(FastInterpolation(1f, 0.3f, defaultFixedDeltaTime));
        yield return null;
    }

    public IEnumerator SlowInterpolation(float interpolationTarget, float time, float fixedDeltaTimeTarget)
    {
        float interpolationAmount = Mathf.Abs(Time.timeScale - interpolationTarget);
        while (!Mathf.Approximately(Time.timeScale, interpolationTarget))
        {
            // When started immediately, Time.timescale will not be affected (rigidbody is still grounded), so delay required
            while (GlobalProc.self.pausePanel.activeSelf) yield return new WaitForFixedUpdate();
            Time.timeScale = Mathf.Clamp(Time.timeScale - interpolationAmount * Time.deltaTime / time, interpolationTarget, 1f);
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }

    public IEnumerator FastInterpolation(float interpolationTarget, float time, float fixedDeltaTimeTarget)
    {
        float interpolationAmount = Mathf.Abs(Time.timeScale - interpolationTarget);
        while (!Mathf.Approximately(Time.timeScale, interpolationTarget))
        {
            // When started immediately, Time.timescale will not be affected (rigidbody is still grounded), so delay required
            while (GlobalProc.self.pausePanel.activeSelf) new WaitForFixedUpdate();
            Time.timeScale = Mathf.Clamp(Time.timeScale + interpolationAmount * Time.deltaTime / time, 0f, interpolationTarget);
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }
}
