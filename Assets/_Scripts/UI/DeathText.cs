using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class DamageText : MonoBehaviour
{
    public float destroyTime = 1f; 
    public Vector3 moveSpeed = new Vector3(0, 1, 0); 
    
    private TextMeshPro textMesh;
    private Color startColor;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        startColor = textMesh.color;
    }

    private void Start()
    {
        // Destroy the object after its display time
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

    public void SetDamageValue(int damage)
    {
        textMesh.text = damage.ToString();
    }
}