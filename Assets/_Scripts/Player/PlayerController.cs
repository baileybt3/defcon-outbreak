using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; 

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    public InputSystem_Actions InputActions => inputActions;
    private InputSystem_Actions inputActions;
    private CharacterController controller;
    
    [Header("Health")] 
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsAlive => currentHealth > 0;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public Vector2 moveInput;
    private Vector3 velocity;

    // Jumping
    public float gravity = -9.81f;
    public float jumpHeight = 1.0f;
    //Crouching
    private bool isCrouching = false;
    public float crouchSpeed = 2f;
    public float standingHeight = 2f;
    public float crouchingHeight = 1f;
    public Transform cameraTransform;

    [Header("Look")]
    public float sensitivity = 100f;
    private Vector2 lookInput;
    [SerializeField] private Transform playerCamera;
    private float pitch = 0f;

    [Header("Recoil")]
    [SerializeField] private float recoilKick = 2f;
    private Vector3 recoilRotation;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main?.transform;
        }
        
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); 
            return;
        }

    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.PlayerInputActions.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.PlayerInputActions.Move.canceled += _ => moveInput = Vector2.zero;

        inputActions.PlayerInputActions.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();

        inputActions.PlayerInputActions.Crouch.started += _ => StartCrouch();
        inputActions.PlayerInputActions.Crouch.canceled += _ => StopCrouch();

        //inputActions.PlayerInputActions.Interact.performed += ctx => TryInteract();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        currentHealth = maxHealth;
        Time.timeScale = 1f; 
    }

    private void Update()
    {
        // Look
        float mouseX = lookInput.x * sensitivity * Time.deltaTime;
        float mouseY = lookInput.y * sensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0, 0);

        lookInput = Vector2.zero;

        // Movement
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        float currentSpeed = isCrouching ? crouchSpeed : moveSpeed;

        // Ground check
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Jump
        if (inputActions.PlayerInputActions.Jump.triggered && controller.isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        // Final Move
        Vector3 finalMove = (move * currentSpeed) + (Vector3.up * velocity.y);
        controller.Move(finalMove * Time.deltaTime);

    }

    void StartCrouch()
    {
        controller.height = crouchingHeight;
        isCrouching = true;
    }

    void StopCrouch()
    {
        controller.height = standingHeight;
        isCrouching = false;
    }

    //void TryInteract()
    //{
    //    float interactRange = 5f;

    //    if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, interactRange))
    //    {
    //        PartOrderingComputer computer = hit.collider.GetComponent<PartOrderingComputer>();
    //        if (computer != null)
    //        {
    //            computer.ToggleUI();
    //            Debug.Log("Opened part ordering UI");
    //            return;
    //        }
    //    }
    //}

    // --- DAMAGE AND HEALING ---
    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        Debug.Log($"Player took {damage} damage. Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public bool Heal(int amount)
    {
        if (currentHealth >= maxHealth)
        {
            Debug.Log("Health is already full!");
            return false;
        }

        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        Debug.Log($"Player healed for {amount}. Health: {currentHealth}/{maxHealth}");
        return true;
    }

    private void Die()
    {
        Debug.Log("Player has died! Restarting level...");
        
        Time.timeScale = 0f; 
        moveSpeed = 0f;
        
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
    
    // --- RECOIL EXECUTION (Unchanged) ---
    public void DoRecoil()
    {
        float sideRecoil = Random.Range(-1f, 1f) * (recoilKick / 5f);
        recoilRotation += new Vector3(-recoilKick, sideRecoil, 0f);
        recoilRotation.x = Mathf.Clamp(recoilRotation.x, -recoilKick * 2, 0f);
    }
}