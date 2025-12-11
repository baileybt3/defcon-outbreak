using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class PartOrderingComputer : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private GameObject defaultSelectedButton;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Economy")]
    [Tooltip("Assign your PlayerController so we can charge/spend money.")]
    [SerializeField] private PlayerController player;
    [SerializeField] private int ammoCost = 50;
    [SerializeField] private int healthKitCost = 75;
    [SerializeField] private int weaponPartCost = 150;

    [Header("Prefabs & Spawn")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject ammoPrefab;
    [SerializeField] private GameObject healthKitPrefab;
    [SerializeField] private GameObject weaponPartPrefab;

   
    private void Start()
    {
        if (uiPanel) uiPanel.SetActive(true);
        SetStatus("Ready.");
    }

    // Called by PlayerController via F key raycast
    public void OnInteract(PlayerController pc)
    {
        if (pc != null) player = pc;
    }

    // --- Purchase Buttons ---
    public void BuyAmmo()
    {
        if (!EnsureReady(ammoPrefab, ammoCost)) return;
        if (player.TrySpend(ammoCost))
        {
            Spawn(ammoPrefab);
            SetStatus($"Ammo purchased (-${ammoCost}).");
        }
        else SetStatus("Not enough money for ammo.");
    }

    public void BuyHealthKit()
    {
        if (!EnsureReady(healthKitPrefab, healthKitCost)) return;
        if (player.TrySpend(healthKitCost))
        {
            Spawn(healthKitPrefab);
            SetStatus($"Health kit purchased (-${healthKitCost}).");
        }
        else SetStatus("Not enough money for health kit.");
    }

    public void BuyWeaponPart()
    {
        if (!EnsureReady(weaponPartPrefab, weaponPartCost)) return;
        if (player.TrySpend(weaponPartCost))
        {
            Spawn(weaponPartPrefab);
            SetStatus($"Weapon part purchased (-${weaponPartCost}).");
        }
        else SetStatus("Not enough money for weapon part.");
    }

    // --- Helpers ---
    private bool EnsureReady(GameObject prefab, int cost)
    {
        if (!player) { SetStatus("No player assigned."); return false; }
        if (!prefab) { SetStatus("Missing prefab on computer."); return false; }
        if (cost < 0) { SetStatus("Invalid cost."); return false; }
        return true;
    }

    private void Spawn(GameObject prefab)
    {
        Vector3 pos = spawnPoint ? spawnPoint.position : transform.position + transform.forward * 1.0f;
        Instantiate(prefab, pos, Quaternion.identity);
    }


    private void SetStatus(string msg)
    {
        if (statusText) statusText.text = msg;
    }

}
