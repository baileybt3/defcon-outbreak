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
    public float moveSpeed = 4f;
    public Vector2 moveInput;
    private Vector3 velocity;

    private bool isSprinting = false;
    public float sprintSpeed = 7f;

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

    [Header("Economy")]
    [SerializeField] private int startingMoney = 0;
    private int money;
    public int Money => money;

    [Header("Interaction")]
    [SerializeField] private float interactRange = 5f;
    [SerializeField] private LayerMask interactMask = ~0;
    [SerializeField] private string keyTag = "Key";
    public bool HasKey { get; private set; } = false;

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

        inputActions.PlayerInputActions.Sprint.started += _ => StartSprint();
        inputActions.PlayerInputActions.Sprint.canceled += _ => StopSprint();

        inputActions.PlayerInputActions.Interact.performed += ctx => TryInteract();
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
        money = startingMoney;
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
        float currentSpeed = moveSpeed;

        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        else if (isSprinting)
        {
            currentSpeed = sprintSpeed;
        }

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

    void StartSprint()
    {
        isSprinting = true;
    }

    void StopSprint()
    {
        isSprinting = false;
    }

    void TryInteract()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactMask))
        {
            // Key Pickup
            if (!HasKey && (!string.IsNullOrWhiteSpace(keyTag) && hit.collider.CompareTag(keyTag)))
            {
                PickupKey(hit.collider.gameObject);
                return;
            }

            //var computer = hit.collider.GetComponent<PartOrderingComputer>();
            //if (computer == null)
            //{
            //    computer = hit.collider.GetComponentInParent<PartOrderingComputer>();
            //}
            //if(computer != null)
            //{
            //    computer.ToggleUI();
            //    return;
            //}

            hit.collider.gameObject.SendMessage("OnInteract", this, SendMessageOptions.DontRequireReceiver);
            hit.collider.gameObject.SendMessage("Press", SendMessageOptions.DontRequireReceiver);
            hit.collider.gameObject.SendMessage("Click", SendMessageOptions.DontRequireReceiver);
            hit.collider.gameObject.SendMessage("Activate", this, SendMessageOptions.DontRequireReceiver);
        }
    }

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
        Debug.Log("Player has died");
        inputActions.Disable();
    }
    
    // --- Recoil --
    public void DoRecoil()
    {
        float sideRecoil = Random.Range(-1f, 1f) * (recoilKick / 5f);
        recoilRotation += new Vector3(-recoilKick, sideRecoil, 0f);
        recoilRotation.x = Mathf.Clamp(recoilRotation.x, -recoilKick * 2, 0f);
    }

    public void AddMoney(int amount)
    {
        if (amount <= 0) return;
        money += amount;
    }

    public bool TrySpend(int amount)
    {
        if (amount <= 0) return true;
        if (money >= amount)
        {
            money -= amount;
            return true;
        }
        Debug.Log("Not enough money.");
        return false;
    }

    private void PickupKey(GameObject key)
    {
        HasKey = true;

        var col = key.GetComponent<Collider>();
        if (col) col.enabled = false;

        var rbKey = key.GetComponent<Rigidbody>();
        if (rbKey)
        {
            rbKey.linearVelocity = Vector3.zero;
            rbKey.angularVelocity = Vector3.zero;
            rbKey.isKinematic = true;
        }

        Debug.Log("Picked up key.");

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex + 1);

    }
}