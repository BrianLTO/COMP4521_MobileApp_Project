using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIPause : MonoBehaviour
{
    public static UIPause instance;

    //variables assigned in inspector
    public TextMeshProUGUI playerDetails;

    PlayerController player { get { return PlayerController.instance; } }

    //called when the continue button is hit
    public void ResumeGame()
    {
        GameController.instance.resumeGame();
        PlayerMovement.instance.Calibrate();
    }

    //called when the main menu button is hit
    public void MainMenu()
    {
        GameController.instance.MainMenu();
    }

    //update the text box to show player details on enabled
    private void OnEnable()
    {
        //update player stats
        playerDetails.SetText(
            "<color=white>" + "Current health: " + "<color=yellow>" + player.health + "\n" +
            "<color=white>" + "Max health: " + "<color=yellow>" + player.maxHealth + "\n" +
            "<color=white>" + "Contact damage: " + "<color=yellow>" + player.contactDamage + "\n" +
            "<color=white>" + "Attack power: " + "<color=yellow>" + player.attack + "\n" +
            "<color=white>" + "Attack speed: " + "<color=yellow>" + player.attackSpeed.ToString("F2") + "\n" +
            "<color=white>" + "Projectile speed: " + "<color=yellow>" + player.projSpeed.ToString("F2") + "\n" +
            "<color=white>" + "Movement speed: " + "<color=yellow>" + player.movSpeed.ToString("F2") + "\n" +
            "<color=white>" + "Flat damage reduction: " + "<color=yellow>" + player.reductionFlat + "\n" +
            "<color=white>" + "% damage reduction: " + "<color=yellow>" + (player.reductionMulti*100).ToString("F2") + "\n" +
            "<color=white>" + "Explosion damage multiplier: " + "<color=yellow>" + player.explosionMulti.ToString("F2") + "\n" +
            "<color=white>" + "Bonus explosion range: " + "<color=yellow>" + player.explosionRange.ToString("F2") + "\n" +
            "<color=white>" + "Upgrade points: " + "<color=yellow>" + player.upgradePoint + "\n" +
            "<color=white>" + "Reroll points: " + "<color=yellow>" + player.rerollPoint + "\n"
            );
    }

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }
}
