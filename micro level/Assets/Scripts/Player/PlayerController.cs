using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool FacingLeft { get { return facingLeft; } }

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float dashSpeed = 4f;
    [SerializeField] private TrailRenderer myTrailRenderer;
    [SerializeField] private Transform weaponCollider;

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRender;
    private Knockback knockback;
    private float startingMoveSpeed;

    private bool facingLeft = false;
    private bool isDashing = false;

    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRender = GetComponent<SpriteRenderer>();
        knockback = GetComponent<Knockback>();
    }

    private void Start()
    {
        playerControls.Combat.Dash.performed += _ => Dash();

        startingMoveSpeed = moveSpeed;

        // Make sure this exists in your project
        if (ActiveInventory.Instance != null)
        {
            ActiveInventory.Instance.EquipStartingWeapon();
        }
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        PlayerInput();
    }

    private void FixedUpdate()
    {
        AdjustPlayerFacingDirection();
        Move();
    }

    public Transform GetWeaponCollider()
    {
        return weaponCollider;
    }

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        if (myAnimator != null)
        {
            myAnimator.SetFloat("moveX", movement.x);
            myAnimator.SetFloat("moveY", movement.y);
        }
    }

    private void Move()
    {
        if ((knockback != null && knockback.GettingKnockedBack) || 
            (PlayerHealth.Instance != null && PlayerHealth.Instance.isDead))
        {
            return;
        }

        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }

   private void AdjustPlayerFacingDirection()
{
    if (Camera.main == null) return;

    Vector2 mousePos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
    Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

    if (mousePos.x < playerScreenPoint.x)
    {
        mySpriteRender.flipX = true;
        facingLeft = true;
    }
    else
    {
        mySpriteRender.flipX = false;
        facingLeft = false;
    }
}


    private void Dash()
    {
        if (!isDashing && Stamina.Instance != null && Stamina.Instance.CurrentStamina > 0)
        {
            Stamina.Instance.UseStamina();
            isDashing = true;
            moveSpeed *= dashSpeed;

            if (myTrailRenderer != null)
                myTrailRenderer.emitting = true;

            StartCoroutine(EndDashRoutine());
        }
    }

    private IEnumerator EndDashRoutine()
    {
        float dashTime = 0.2f;
        float dashCD = 0.25f;

        yield return new WaitForSeconds(dashTime);

        moveSpeed = startingMoveSpeed;

        if (myTrailRenderer != null)
            myTrailRenderer.emitting = false;

        yield return new WaitForSeconds(dashCD);

        isDashing = false;
    }
}
