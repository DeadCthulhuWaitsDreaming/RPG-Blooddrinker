using System.Collections.Generic;
using UnityEngine;

public class UpgradeForge : MonoBehaviour
{
    GameManager gm;
    readonly List<ForgeMaterial> materialsNeeded = new List<ForgeMaterial>();
    bool done = false;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if(gm.GetPlayer() && !done)
        {
            GetForgeMaterialsNeeded();
            done = true;
        }
    }

    public void OnMouseDown()
    {
        GameObject nextPhase = gm.GetPlayer().GetSword().GetNextPhase();
        Vector3 position = gm.GetPlayer().GetSword().GetPosition();
        if (gm.GetPlayer().GetSword().IsUpgradable())
        {
            if (CheckMaterials())
            {
                gm.SetSwordLevel(gm.GetPlayer().GetSword().GetWeaponLevel());
                Destroy(gm.GetPlayer().GetSword().gameObject);
                gm.GetPlayer().SetSword(Instantiate(nextPhase, position, Quaternion.LookRotation(Vector3.up)).GetComponent<BloodDrinker>());
                //gm.GetPlayer().GetSword().SetOwnership(gm.GetPlayer());
                gm.GetPlayer().DiscardUsedMaterials(materialsNeeded);
            }
            else
            {
                print("You do not have all of the required materials");
            }
        }
        else
        {
            print("The bloodmeter is not yet full");
        }
    }

    bool CheckMaterials()
    {
        for (int x = 0; x < materialsNeeded.Count; x++)
        {
            if (gm.GetPlayer().GetInventoryMaterials().ContainsKey(materialsNeeded[x].name))
            {
                if (gm.GetPlayer().GetInventoryMaterials()[materialsNeeded[x].name] < materialsNeeded[x].quantity)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    void GetForgeMaterialsNeeded()
    {
        //TO-DO: Create sprites for materials, name materials
        string[] materials = new string[] { "Material1", "Material2", "Material3", "Material4", "Material5" };
        int[][] matQty = new int[][] { new int[]{ 5, 3, 1 }, new int[] { 10, 5, 3, 2, 1 } };
        if (gm.GetPlayer().GetSword().name == "Phase1(Clone)")
        {
            for(int x = 0; x < 3; x++)
            {
                materialsNeeded.Add(new ForgeMaterial() { name = materials[x], quantity = matQty[0][x] });
            }          
        }
        else if (gm.GetPlayer().GetSword().name == "Phase2(Clone)")
        {
            for (int x = 0; x < 5; x++)
            {
                materialsNeeded.Add(new ForgeMaterial() { name = materials[x], quantity = matQty[1][x] });
            }
        }
        print("Added Forge Materials");
    }
}

public class ForgeMaterial
{
    public string name;
    public int quantity;
}
