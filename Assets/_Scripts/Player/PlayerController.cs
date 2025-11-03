using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; 

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    // --- SINGLETON IMPLEMENTATION (NEW/MODIFIED) ---
    public static PlayerController Instance { get; private set; } 
    
    [Header("Health")] 
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public Vector2 moveInput;
    private Rigidbody rb;

    [Header("Look")]
    public float sensitivity = 100f;
    private Vector2 lookInput;
    private float xRotation = 0f;
    [SerializeField] private Transform playerCamera;

    [Header("Recoil")]
    [SerializeField] private float recoilKick = 2f;
    [SerializeField] private float recoilRecoverySpeed = 10f;
    private Vector3 recoilRotation;

    // --- PUBLIC PROPERTIES (ACCESS FIX) ---
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsAlive => currentHealth > 0;

    // --- UNITY LIFECYCLE METHODS ---
    private void Awake() // MODIFIED
    {
        // Set the static Instance for global access
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); 
            return;
        }

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        currentHealth = maxHealth;
        Time.timeScale = 1f; 
    }

    // --- Input Callbacks (Unchanged) ---
    void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    void OnLook(InputValue value) => lookInput = value.Get<Vector2>();

    // --- Movement (Unchanged) ---
    private void FixedUpdate()
    {
        Vector3 dir = new Vector3(moveInput.x, 0, moveInput.y);
        Vector3 move = dir * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + transform.TransformDirection(move));
    }

    // --- Look & Recoil Logic (Unchanged) ---
    private void Update()
    {
        float mouseX = lookInput.x * sensitivity * Time.deltaTime;
        float mouseY = lookInput.y * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        recoilRotation = Vector3.Lerp(recoilRotation, Vector3.zero, recoilRecoverySpeed * Time.deltaTime);
        
        playerCamera.localRotation = Quaternion.Euler(xRotation + recoilRotation.x, recoilRotation.y, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    // --- DAMAGE AND HEALING (Unchanged logic) ---
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