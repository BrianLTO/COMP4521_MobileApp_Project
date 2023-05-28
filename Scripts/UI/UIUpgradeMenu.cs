using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIUpgradeMenu : MonoBehaviour
{
    GameController GC { get { return GameController.instance; } }
    PlayerController PC { get { return PlayerController.instance; } }

    //variables assigned in inspector
    public static UIUpgradeMenu instance;
    public TextMeshProUGUI healthCost, healthBonus;
    public TextMeshProUGUI attackCost, attackBonus;
    public TextMeshProUGUI attackSpeedCost, attackSpeedBonus;
    public TextMeshProUGUI projSpeedCost, projSpeedBonus;
    public TextMeshProUGUI movSpeedCost, movSpeedBonus;
    public TextMeshProUGUI reductionFlatCost, reductionFlatBonus;
    public TextMeshProUGUI coin;

    //scaling factor for upgrade cost
    int costScale = 5;

    void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    //update all text according to level
    void UpdateAllText()
    {
        //update coin count
        coin.text = "<color=yellow>" + GC.coins + " <color=white>x";

        //update bonus shown
        healthBonus.text = "<color=yellow>" + GC.healthLevel * 10 + "%";
        attackBonus.text = "<color=yellow>" + GC.attackLevel * 10 + "%";
        attackSpeedBonus.text = "<color=yellow>" + GC.attackSpeedLevel * 10 + "%";
        projSpeedBonus.text = "<color=yellow>" + GC.projSpeedLevel * 10 + "%";
        movSpeedBonus.text = "<color=yellow>" + GC.movSpeedLevel * 10 + "%";
        reductionFlatBonus.text = "<color=yellow>" + GC.reductionFlatLevel + "";

        //update costs shown
        healthCost.text = "<color=white>x<color=yellow>" + GetCost(GC.healthLevel);
        attackCost.text = "<color=white>x<color=yellow>" + GetCost(GC.attackLevel);
        attackSpeedCost.text = "<color=white>x<color=yellow>" + GetCost(GC.attackSpeedLevel);
        projSpeedCost.text = "<color=white>x<color=yellow>" + GetCost(GC.projSpeedLevel);
        movSpeedCost.text = "<color=white>x<color=yellow>" + GetCost(GC.movSpeedLevel);
        reductionFlatCost.text = "<color=white>x<color=yellow>" + GetCost(GC.reductionFlatLevel);
    }

    int GetCost(int level)
    {
        //get upgrade cost from level to level+1
        return (level + 1) * costScale;
    }

    bool CanAfford(int level)
    {
        //return boolean of can afford or not
        return GetCost(level) <= GC.coins;
    }

    //update all text when script if enabled
    private void OnEnable()
    {
        UpdateAllText();
    }

    //save changes of uprade to saveFile then write it to disk
    void SaveChange()
    {
        //save upgrade changes
        GC.saveFile.health = GC.healthLevel;
        GC.saveFile.attack = GC.attackLevel;
        GC.saveFile.attackSpeed = GC.attackSpeedLevel;
        GC.saveFile.projSpeed = GC.projSpeedLevel;
        GC.saveFile.movSpeed = GC.movSpeedLevel;
        GC.saveFile.reductionFlat = GC.reductionFlatLevel;
        GC.saveFile.coins = GC.coins;
        GC.WriteSaveData();

        //update UI elements
        UpdateAllText();
    }

    //attempt to change the upgrade level
    void AttemptChangeLevel(ref int currentLevel, bool isUpgrade)
    {
        if (isUpgrade)
        {
            //check if player can afford upgrade
            if (!CanAfford(currentLevel)) return; //return if cant afford
            GC.coins -= GetCost(currentLevel);
            currentLevel++;
            SaveChange();
        }
        else
        {
            //check if upgrade level is higher than 0 for refund
            if (currentLevel <= 0) return; //return if cant refund
            GC.coins += GetCost(currentLevel - 1);
            currentLevel--;
            SaveChange();
        }
    }

    //these global functions are for UI buttons to trigger attempt change level
    public void AddHP()
    {
        AttemptChangeLevel(ref GC.healthLevel, true);
    }
    public void MinusHP()
    {
        AttemptChangeLevel(ref GC.healthLevel, false);
    }
    public void AddAP()
    {
        AttemptChangeLevel(ref GC.attackLevel, true);
    }
    public void MinusAP()
    {
        AttemptChangeLevel(ref GC.attackLevel, false);
    }
    public void AddAS()
    {
        AttemptChangeLevel(ref GC.attackSpeedLevel, true);
    }
    public void MinusAS()
    {
        AttemptChangeLevel(ref GC.attackSpeedLevel, false);
    }
    public void AddPS()
    {
        AttemptChangeLevel(ref GC.projSpeedLevel, true);
    }
    public void MinusPS()
    {
        AttemptChangeLevel(ref GC.projSpeedLevel, false);
    }
    public void AddMS()
    {
        AttemptChangeLevel(ref GC.movSpeedLevel, true);
    }
    public void MinusMS()
    {
        AttemptChangeLevel(ref GC.movSpeedLevel, false);
    }
    public void AddFDR()
    {
        AttemptChangeLevel(ref GC.reductionFlatLevel, true);
    }
    public void MinusFDR()
    {
        AttemptChangeLevel(ref GC.reductionFlatLevel, false);
    }

    //function for main menu button
    public void MainMenu()
    {
        gameObject.SetActive(false);
        UIMainMenu.instance.gameObject.SetActive(true);
    }


}
