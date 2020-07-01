#define DEBUGGING
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    GameManager gm;
    BloodDrinker sword;

    [SerializeField]
    int playerLevel, playerStrength, kills, baseDamage, modifiedDamage;
    [SerializeField]
    uint playerExp;
    [SerializeField]
    float attackModifier;

    readonly Inventory inventory = new Inventory();
    
    [SerializeField]
    Text levelText, strengthText, killsText, expText;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        gm.SetPlayer(this);
        playerLevel = 1;
        playerStrength = 10;
        playerExp = 1;
        attackModifier = .01f;
        sword = Instantiate(Resources.Load("Assets/Phase1") as GameObject, new Vector3(0, 0, 0), Quaternion.LookRotation(Vector3.up)).GetComponent<BloodDrinker>();
        baseDamage = sword.GetWeaponDamage() + playerStrength;

        levelText.text = "Player Level: " + playerLevel;
        strengthText.text = "Player Strength: " + playerStrength;
        killsText.text = "Player Kills: " + kills;
        expText.text = "Player Exp: " + playerExp;

#if DEBUGGING
        AddInventoryMaterialItem(new ForgeMaterial() { name = "Material1", quantity = 15 });
        AddInventoryMaterialItem(new ForgeMaterial() { name = "Material2", quantity = 8 });
        AddInventoryMaterialItem(new ForgeMaterial() { name = "Material3", quantity = 4 });
        AddInventoryMaterialItem(new ForgeMaterial() { name = "Material4", quantity = 2 });
        AddInventoryMaterialItem(new ForgeMaterial() { name = "Material5", quantity = 1 });
#endif

    }

    public void ExpUp(int exp)
    {
        playerExp += (uint)exp;
        playerLevel = gm.ExpToLevel(playerExp);
        playerStrength = playerLevel * 2 + 10;
        UpdateDamage();

        levelText.text = "Player Level: " + playerLevel;
        strengthText.text = "Player Strength: " + playerStrength;
        killsText.text = "Player Kills: " + kills;
        expText.text = "Player Exp: " + playerExp;
    }

    public BloodDrinker GetSword()
    {
        return sword;
    }

    public void UpdateDamage()
    {
        baseDamage = sword.GetWeaponDamage() + playerStrength;
        modifiedDamage = baseDamage + (int)(attackModifier * baseDamage);
    }

    public int GetDamage()
    {
        return baseDamage + (int)(attackModifier * baseDamage);
    }

    public uint GetExp()
    {
        return playerExp;
    }

    public int GetLevel()
    {
        return playerLevel;
    }

    public int GetStrength()
    {
        return playerStrength;
    }

    public float GetAttackModifier()
    {
        return attackModifier;
    }

    public Dictionary<string, int> GetInventoryMaterials()
    {
        return inventory.materials;
    }

    public Dictionary<string, int> GetInventoryBackpack()
    {
        return inventory.backpack;
    }

    public void AddInventoryMaterialItem(ForgeMaterial material)
    {
        if(inventory.materials.ContainsKey(material.name))
        {
            inventory.materials[material.name] += material.quantity;
        }
        else
        {
            inventory.materials[material.name] = material.quantity;
        }
    }

    public void DiscardUsedItem(InventoryItem item)
    {

    }

    public void DiscardUsedMaterials(List<ForgeMaterial> materials)
    {
        foreach (ForgeMaterial material in materials)
        {
            inventory.materials[material.name] -= material.quantity;
            print($"Removed {material.quantity} {material.name} from Inventory");
            if(inventory.materials[material.name] == 0)
            {
                inventory.materials.Remove(material.name);
                print($"Removed {material.name}");
            }
        }
    }

    public void SetSword(BloodDrinker forgedSword)
    {
        sword = forgedSword;
    }

    public void AddKilltoPlayer()
    {
        kills += 1;
    }

    public void IncreaseAttackModifier(float increase)
    {
        attackModifier += increase;
    }
}

public class Inventory
{
    public Dictionary<string, int> materials = new Dictionary<string, int>();
    public Dictionary<string, int> backpack = new Dictionary<string, int>();
}

public class InventoryItem
{

}
