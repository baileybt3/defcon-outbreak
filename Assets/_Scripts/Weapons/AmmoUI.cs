using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    public PlayerCombat playerCombat;
    public Text ammoText;
    public string reloadMessage = "Reloading...";

    private void Update()
    {
        if (playerCombat == null || ammoText == null) return;

        if (playerCombat.IsReloading)
        {
            ammoText.text = reloadMessage;
        }
        else
        {
            ammoText.text = $"{playerCombat.CurrentAmmo} / {playerCombat.ReserveAmmo}";
        }
    }
}
