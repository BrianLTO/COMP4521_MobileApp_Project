using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgrade : MonoBehaviour
{
    public static UIUpgrade instance; 
    
    //variables assigned in inspector
    public TextMeshProUGUI augmentName, augmentDesc, augmentRank, upgradeLeft, RerollLeft;
    public Button[] augmentButtons;
    public AudioClip upgradeSound;
    public AudioClip rerollSound;
    public float rare2Chance = 0.2f;
    public float rare1Chance = 0.4f;

    //list of augment IDs according to rarity
    List<int> rarity0 = new List<int>();
    List<int> rarity1 = new List<int>();
    List<int> rarity2 = new List<int>();

    AugmentWrapper wrapper; //wrapper to contain all augments
    int[] rolledAugments; //array containing rolled augments
    int focusedAugment = -1; //augment focused by the player

    public void AttemptReroll()
    {
        //check for reroll points
        if (PlayerController.instance.rerollPoint <= 0) return; //return if not enough

        PlayerController.instance.rerollPoint--;
        GameController.instance.audioSource.PlayOneShot(rerollSound);
        SetRerollText(PlayerController.instance.rerollPoint);

        Reroll();
    }

    public void AttemptUpgrade()
    {
        //check for upgrade points
        if (PlayerController.instance.upgradePoint <= 0) return; //return if not enough
        if (focusedAugment < 0) return; //return if no augment is focused

        PlayerController.instance.upgradePoint--;
        GameController.instance.audioSource.PlayOneShot(upgradeSound);
        SetUpgradeText(PlayerController.instance.upgradePoint);

        Augment chosenAugment = wrapper.GetAugment(focusedAugment);
        if (PlayerController.instance.augments.Exists(x => x.GetType() == chosenAugment.GetType()))
        {
            //rank up player augment if already exist
            Augment playerAugment = PlayerController.instance.augments.Find(x => x.GetType() == chosenAugment.GetType());
            playerAugment.rank++;
        }
        else
        {
            //give player augment with rank 1
            PlayerController.instance.augments.Add(chosenAugment);
        }

        Reroll();
    }

    //function for debugging
    public void ForceUpgrade()
    {
        if (focusedAugment < 0) return; //return if no augment is focused

        GameController.instance.audioSource.PlayOneShot(upgradeSound);
        SetUpgradeText(PlayerController.instance.upgradePoint);

        Augment chosenAugment = wrapper.GetAugment(focusedAugment);
        if (PlayerController.instance.augments.Exists(x => x.GetType() == chosenAugment.GetType()))
        {
            //rank up player augment if already exist
            Augment playerAugment = PlayerController.instance.augments.Find(x => x.GetType() == chosenAugment.GetType());
            playerAugment.rank++;
        }
        else
        {
            //give player augment with rank 1
            PlayerController.instance.augments.Add(chosenAugment);
        }
    }

    //function for debugging
    public void ForceReroll()
    {
        Reroll();
    }

    //update text boxes according to the focused augment
    public void FocusAugment(int buttonId)
    {
        //get augment from wrapper
        Augment chosenAugment = wrapper.GetAugment(rolledAugments[buttonId]);
        focusedAugment = rolledAugments[buttonId];

        //set fields according to the chosen augment;
        SetName(chosenAugment);
        if (PlayerController.instance.augments.Exists(x => x.GetType() == chosenAugment.GetType()))
        {
            //if the player have this augment
            Augment playerAugment =  PlayerController.instance.augments.Find(x => x.GetType() == chosenAugment.GetType());
            SetDesc(playerAugment, playerAugment.rank + 1);
            SetRank(playerAugment.rank + 1);
        }
        else
        {
            //if the player doesnt have this augment
            SetDesc(chosenAugment);
            SetRank(chosenAugment.rank);
        }
    }

    //function for next level button
    public void NextLevel()
    {
        GameController.instance.NextLevel();
    }


    private void Awake()
    {
        instance = this;
        wrapper = GetComponent<AugmentWrapper>();
        rolledAugments = new int[augmentButtons.Length];
        gameObject.SetActive(false);
    }

    //reset UI on script enabled
    private void OnEnable()
    {
        ResetUI();
    }

    void ResetUI()
    {
        //update all text boxes and reroll
        SetUpgradeText(PlayerController.instance.upgradePoint);
        SetRerollText(PlayerController.instance.rerollPoint);
        Reroll();
    }

    void ResetList()
    {
        //reset lists for next reroll
        rarity0.Clear();
        rarity1.Clear();
        rarity2.Clear();

        //add augment ID to arrays according to rarity
        for (int i = 0; i < wrapper.numAugments; i++)
        {
            Augment augment = wrapper.GetAugment(i);
            switch (augment.rarity)
            {
                case 0:
                    rarity0.Add(i);
                    break;

                case 1:
                    rarity1.Add(i);
                    break;

                case 2:
                    rarity2.Add(i);
                    break;
                default:
                    break;
            }
        }
    }


    void Reroll()
    {
        ResetList();

        //get an augment for each button without replacement
        for (int i = 0; i < augmentButtons.Length; i++)
        {
            float chance = Random.Range(0, 1f);

            //get an augment with rarity 2
            if (chance < rare2Chance)
            {
                int index = Random.Range(0, rarity2.Count);
                rolledAugments[i] = rarity2[index];
                rarity2.RemoveAt(index);
            }
            //get an augment with rarity 1
            else if (chance < rare1Chance)
            {
                int index = Random.Range(0, rarity1.Count);
                rolledAugments[i] = rarity1[index];
                rarity1.RemoveAt(index);
            }
            //get an augment with rarity 0
            else
            {
                int index = Random.Range(0, rarity0.Count);
                rolledAugments[i] = rarity0[index];
                rarity0.RemoveAt(index);
            }
        }

        focusedAugment = -1; //remove focused augment

        //reset text boxes
        ResetName();
        ResetDesc();
        ResetRank();
        UpdateButtons();
    }

    void UpdateButtons()
    {
        //update the buttons with corresponding sprites
        for (int i = 0; i < augmentButtons.Length; i++)
        {
            augmentButtons[i].image.sprite = wrapper.GetAugmentSprite(rolledAugments[i]);
        }
    }

    //below are helper functions to set and reset text boxes
    void SetName (Augment augment)
    {
        augmentName.text = augment.augmentName;
    }

    void ResetName()
    {
        augmentName.text = "Upgrade Time!";
    }

    void SetDesc (Augment augment)
    {
        augmentDesc.text = augment.GetDescription();
    }
    
    void SetDesc (Augment augment, int level)
    {
        augmentDesc.text = augment.GetDescription(level);
    }
    void ResetDesc()
    {
        augmentDesc.text = "Choose an augment to see its effects!";
    }

    void SetRank (int rank)
    {
        augmentRank.text = "Rank<color=yellow> " + rank;
    }

    void ResetRank()
    {
        augmentRank.text = "Rank ???";
    }

    void SetUpgradeText(int num)
    {
        if (num <= 0) upgradeLeft.text = "<color=yellow> " + "no" + " <color=white>Upgrade Points Left";
        else upgradeLeft.text = "<color=yellow> " + num + " <color=white>Upgrade Points Left";
    }

    void SetRerollText(int num)
    {
        if (num <= 0) RerollLeft.text = "<color=yellow> " + "no" + " <color=white>Reroll Points Left";
        else RerollLeft.text = "<color=yellow> " + num + " <color=white>Reroll Points left";
    }
}
