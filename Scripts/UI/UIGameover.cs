using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIGameover : MonoBehaviour
{
    public static UIGameover instance;

    //assigned in inspector
    public TextMeshProUGUI gameoverText, finalStat, finalReward;
    public Button menuButton;
    public AudioClip popSound;

    Image bgImage;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        bgImage = GetComponent<Image>();
    }

    //called when player death animation is done
    public IEnumerator ShowEndScreen()
    {
        //calculate reward and save game data
        GameController.instance.isPlaying = false;
        GameController.instance.saveFile.inGame = false;
        int rewardAmount = Mathf.FloorToInt((GameController.instance.level - 1)*2 + GameController.instance.destroyCount/5);
        GameController.instance.coins += rewardAmount;
        GameController.instance.WriteSaveData();

        //animation of end screen
        for (float i = 0; i <= 1; i += Time.unscaledDeltaTime)
        {
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
            bgImage.color = new Color(0, 0, 0, i);
            Time.timeScale = 1 - i;
        }
        GameController.instance.audioSource.PlayOneShot(popSound);
        gameoverText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1f);

        GameController.instance.audioSource.PlayOneShot(popSound);
        finalStat.gameObject.SetActive(true);
        finalStat.text = "<color=white>Enemies destroyed: <color=yellow> " + GameController.instance.destroyCount + "\n";
        yield return new WaitForSecondsRealtime(0.5f);

        GameController.instance.audioSource.PlayOneShot(popSound);
        finalStat.text = finalStat.text + "<color=white>Levels conquered: <color=yellow> " + (GameController.instance.level-1) + "\n";
        yield return new WaitForSecondsRealtime(0.5f);

        GameController.instance.audioSource.PlayOneShot(popSound);
        finalStat.text = finalStat.text + "<color=white>Damage dealt: <color=yellow> " + PlayerController.instance.damageDealt + "\n";
        yield return new WaitForSecondsRealtime(0.5f);

        GameController.instance.audioSource.PlayOneShot(popSound);
        finalStat.text = finalStat.text + "<color=white>Damage taken: <color=yellow> " + (-PlayerController.instance.damageTaken) + "\n";
        yield return new WaitForSecondsRealtime(0.5f);

        GameController.instance.audioSource.PlayOneShot(popSound);
        finalStat.text = finalStat.text + "<color=white>Damage healed: <color=yellow> " + PlayerController.instance.damageHealed + "\n";
        yield return new WaitForSecondsRealtime(0.5f);

        GameController.instance.audioSource.PlayOneShot(popSound);
        finalReward.gameObject.SetActive(true);
        finalReward.text = "<color=white>FINAL REWARD\n";
        yield return new WaitForSecondsRealtime(1f);

        GameController.instance.audioSource.PlayOneShot(popSound);
        if (rewardAmount == 1) finalReward.text = finalReward.text + "<color=yellow>" + rewardAmount + "<color=white> COIN\n";
        else finalReward.text = finalReward.text + "<color=yellow>" + rewardAmount + "<color=white> COINS\n";
        yield return new WaitForSecondsRealtime(1f);

        GameController.instance.audioSource.PlayOneShot(popSound);
        menuButton.gameObject.SetActive(true);
    }

    //called when main menu button is hit
    public void FinishGame()
    {
        //end the game and go back to main menu
        GameController.instance.FinishGame();
    }
}
