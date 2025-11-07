using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ShopButton : MonoBehaviour
{
    public enum Action { Ammo, HealthKit, WeaponPart }

    [Header("Setup")]
    [SerializeField] private PartOrderingComputer computer;
    [SerializeField] private Action action = Action.Ammo;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = false; 
    }

    
    public void OnInteract(PlayerController player)
    {
        if (!computer) computer = GetComponentInParent<PartOrderingComputer>();
        if (!computer) return;

        switch (action)
        {
            case Action.Ammo: computer.BuyAmmo(); break;
            case Action.HealthKit: computer.BuyHealthKit(); break;
            case Action.WeaponPart: computer.BuyWeaponPart(); break;
        }
    }

   
    public void Press() => OnInteract(PlayerController.Instance);
    public void Click() => OnInteract(PlayerController.Instance);
    public void Activate(PlayerController player) => OnInteract(player);
}
