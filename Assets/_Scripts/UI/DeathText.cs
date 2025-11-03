using UnityEngine;
using TMPro; // REQUIRED: For TextMeshPro components

[RequireComponent(typeof(TextMeshPro))]
public class DamageText : MonoBehaviour
{
    // These fields are set in the Inspector on the Prefab
    public float destroyTime = 1f; // How long the text lasts
    public Vector3 moveSpeed = new Vector3(0, 1, 0); // Floats text upwards
    
    private TextMeshPro textMesh;
    private Color startColor;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        startColor = textMesh.color;
    }

    private void Start()
    {
        // Destroy the object after its display time is up
        Destroy(gameObject, destroyTime);
    }

    private void Update()
    {
        // Move the text upwards
        transform.position += moveSpeed * Time.deltaTime;

        // Fade the text out by reducing its alpha over time
        float timeRemaining = destroyTime - (Time.time - (Time.time - destroyTime)); 
        float alpha = startColor.a * (timeRemaining / destroyTime);
        textMesh.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
    }

    // Called by the EnemyController when the enemy is hit
    public void SetDamageValue(int damage)
    {
        textMesh.text = damage.ToString();
    }
}