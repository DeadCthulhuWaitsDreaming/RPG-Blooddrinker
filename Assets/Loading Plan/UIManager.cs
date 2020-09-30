//#define TESTING
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System.Diagnostics;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    Transform infoPanel, qtyPanel, newCustomerPanel, customerSelect;
    readonly Transform[] transforms = new Transform[9];

    readonly Dictionary<Transform, int[]> enteredQty = new Dictionary<Transform, int[]>();
    readonly CustomerInfo ci = new CustomerInfo();
    List<Customer> customers = new List<Customer>();
    Dropdown cd;
    //List<Dropdown.OptionData> dataList = new List<Dropdown.OptionData>();

    int[] rowsPerPallet;
#if TESTING
    [SerializeField]
#endif
    int[] flavorCases = new int[] { 99, 99, 400, 36, 3, 0, 20, 0, 293, 0, 131, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    readonly int[] backup = new int[33];

    readonly string[] toggles = new string[] { 
        "Croissants", "Mini Croissants (L)", "Mini Croissants (S)", "Multi Croissants", "Bagel Chips (S)", "Bagel Chips (L)", "Pizzeti (S)", "Pizzeti (L)", "Racks", "Shippers" 
    }, 
        flavors = new string[] { 
        "Chocolate Croissants", "Vanilla Croissants", "Strawberry Croissants", "Caramel Croissants", "PBJ Croissants", "PBC Croissants",
        "Chocolate LG MINI Croissants", "Vanilla LG MINI Croissants",
        "Chocolate SM MINI Croissants", "Vanilla SM MINI Croissants", "Cherry SM MINI Croissants",
        "Chocolate Multi Croissants", "Vanilla Multi Croissants", "Strawberry Multi Croissants",
        "Garlic Bagel Chips SMALL", "Sea Salt Bagel Chips SMALL", "Everything Bagel Chips SMALL", "Cinnamon Bagel Chips SMALL",
        "Garlic Bagel Chips LARGE", "Sea Salt Bagel Chips LARGE", "Everything Bagel Chips LARGE", "Cinnamon Bagel Chips LARGE",
        "Olive Pizzeti SMALL", "Garlic Pizzeti SMALL", "Tomato Pizzeti SMALL",
        "Olive Pizzeti LARGE", "Garlic Pizzeti LARGE", "Tomato Pizzeti LARGE",
        "Racks",
        "Shipper Croissant", "Shipper Bagel Chips SMALL", "Shippers Bagel Chips LARGE", "Shippers Pizzeti LARGE"
    };

    
    readonly bool[] activePanels = new bool[] { false, false, false, false, false, false, false, false, false, false };
    bool sort = false;

    void Start()
    {
        cd = infoPanel.GetChild(0).GetChild(0).GetComponent<Dropdown>();
        for (int x = 0; x < qtyPanel.transform.childCount - 2; x++)
        {
            transforms[x] = qtyPanel.transform.GetChild(x);
            enteredQty.Add(transforms[x], new int[transforms[x].childCount - 1]);
            for (int y = 0; y < transforms[x].childCount - 1; y++)
            {
                transforms[x].GetChild(y).GetChild(0).GetComponent<InputField>().text = "0";
                enteredQty[transforms[x]][y] = 0;
            }
        }
        customers = RetriveCustomerList();
        //UpdateDropdown();
    }

    public void PrintFiles()
    {

        string path = "C:\\Users\\Henry\\Documents\\GitHub Projects\\RPG-Blooddrinker\\Assets\\Loading Plan\\Loading Plan.txt";
        /*ProcessStartInfo info = new ProcessStartInfo(path)
        {
            Verb = "print",
            CreateNoWindow = false,
            WindowStyle = ProcessWindowStyle.Hidden
        };*/

        Process p = new Process();
        p.StartInfo.FileName = path;
        p.StartInfo.ErrorDialog = true;
        p.Start();
        p.WaitForExit();
        //process.Start();

    }

    public void TogglePanel()
    {
        Toggle toggle = EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>();
        for (int x = 0; x < toggles.Length; x++)
        {
            if (toggle.name == toggles[x] && !activePanels[x])
            {
                activePanels[x] = true;
            }
            else if (toggle.name == toggles[x] && activePanels[x])
            {
                activePanels[x] = false;
            }
        }
    }

    public void BackToInfo()
    {
        SetCustomerInfo(true);
        for (int x = 0; x < activePanels.Length; x++)
        {
            if (activePanels[x])
            {
                transforms[x].gameObject.SetActive(false);
            }
        }
        infoPanel.parent.gameObject.SetActive(true);
        qtyPanel.gameObject.SetActive(false);
    }

    public void OnEnterInfo()
    {
        SetCustomerInfo(false);
        if (customers[cd.value].fullPallets)
        {
            rowsPerPallet = new int[] { 11, 11, 11, 11, 11, 11, 11, 11, 8, 8, 8, 12, 12, 12, 9, 9, 9, 9, 8, 8, 8, 8, 9, 9, 9, 7, 7, 7, 25, 3, 3, 3, 3 };    //full pallet
        }
        else
        {
            rowsPerPallet = new int[] { 10, 10, 10, 10, 10, 10, 10, 10, 7, 7, 7, 11, 11, 11, 9, 9, 9, 9, 8, 8, 8, 8, 9, 9, 9, 7, 7, 7, 25, 3, 3, 3, 3 };    //full bagel chip/pizzeti
        }
        for (int x = 0; x < activePanels.Length; x++)
        {
            if(activePanels[x])
            {
                transforms[x].gameObject.SetActive(true);
            }
        }
        infoPanel.parent.gameObject.SetActive(false);
        qtyPanel.gameObject.SetActive(true);
    }

    public void OnEnterQty()
    {
        for(int x = 0; x < flavorCases.Length; x++)
        {
#if !TESTING
            switch(x)
            {
                case 0:
                    flavorCases[x] = int.Parse(transforms[0].GetChild(0).GetChild(0).GetComponent<InputField>().text);
                    break;
                case 1:
                    flavorCases[x] = int.Parse(transforms[0].GetChild(1).GetChild(0).GetComponent<InputField>().text);
                    break;
                case 2:
                    flavorCases[x] = int.Parse(transforms[0].GetChild(2).GetChild(0).GetComponent<InputField>().text);
                    break;
                case 3:
                    flavorCases[x] = int.Parse(transforms[0].GetChild(3).GetChild(0).GetComponent<InputField>().text);
                    break;
                case 4:
                    flavorCases[x] = int.Parse(transforms[0].GetChild(4).GetChild(0).GetComponent<InputField>().text);
                    break;
                case 5:
                    flavorCases[x] = int.Parse(transforms[0].GetChild(5).GetChild(0).GetComponent<InputField>().text);
                    break;
                case 6:
                    flavorCases[x] = int.Parse(transforms[1].GetChild(0).GetChild(0).GetComponent<InputField>().text);
                    break;
                case 7:
                    flavorCases[x] = int.Parse(transforms[1].GetChild(1).GetChild(0).GetComponent<InputField>().text);
                    break;
                case 8:
                    flavorCases[x] = int.Parse(transforms[2].GetChild(0).GetChild(0).GetComponent<InputField>().text);
                    break;
                case 9:
                    flavorCases[x] = int.Parse(transforms[2].GetChild(1).GetChild(0).GetComponent<InputField>().text);
                    break;
                case 10:
                    flavorCases[x] = int.Parse(transforms[2].GetChild(2).GetChild(0).GetComponent<InputField>().text);
                    break;
                case 11:
                    flavorCases[x] = int.Parse(transforms[3].GetChild(0).GetChild(0).GetComponent<InputField>().text);
                    break;
                case 12:
                    flavorCases[x] = int.Parse(transforms[3].GetChild(1).GetChild(0).GetComponent<InputField>().text);
                    break;
                case 13:
                    flavorCases[x] = int.Parse(transforms[3].GetChild(2).GetChild(0).GetComponent<InputField>().text);
                    break;
                case 14:
                    flavorCases[x] = int.Parse(transforms[4].GetChild(0).GetChild(0).GetComponent<InputField>().text);
                    break;
                case 15:
                    flavorCases[x] = int.Parse(transforms[4].GetChild(1).GetChild(0).GetComponent<InputField>().text);
                    break;
                case 16:
                    flavorCases[x] = int.Parse(transforms[4].GetChild(2).GetChild(0).GetComponent<InputField>().text);
                    break;
                case 17:
                    flavorCases[x] = int.Parse(transforms[4].GetChild(3).GetChild(0).GetComponent<InputField>().text);
                    break;
                case 18:
                    flavorCases[x] = int.Parse(transforms[5].GetChild(0).GetChild(0).GetComponent<InputField>().text);
                    break;
                case 19:
                    flavorCases[x] = int.Parse(transforms[5].GetChild(1).GetChild(0).GetComponent<InputField>().text);
                    break;
                case 20:
                    flavorCases[x] = int.Parse(transforms[5].GetChild(2).GetChild(0).GetComponent<InputField>().text);
                    break;
                case 21:
                    flavorCases[x] = int.Parse(transforms[5].GetChild(3).GetChild(0).GetComponent<InputField>().text);
                    break;
                case 22:
                    flavorCases[x] = int.Parse(transforms[6].GetChild(0).GetChild(0).GetComponent<InputField>().text);
                    break;
                case 23:
                    flavorCases[x] = int.Parse(transforms[6].GetChild(1).GetChild(0).GetComponent<InputField>().text);
                    break;
                case 24:
                    flavorCases[x] = int.Parse(transforms[6].GetChild(2).GetChild(0).GetComponent<InputField>().text);
                    break;
                case 25:
                    flavorCases[x] = int.Parse(transforms[7].GetChild(0).GetChild(0).GetComponent<InputField>().text);
                    break;
                case 26:
                    flavorCases[x] = int.Parse(transforms[7].GetChild(1).GetChild(0).GetComponent<InputField>().text);
                    break;
                case 27:
                    flavorCases[x] = int.Parse(transforms[7].GetChild(2).GetChild(0).GetComponent<InputField>().text);
                    break;
                case 28:
                    flavorCases[x] = int.Parse(transforms[8].GetChild(0).GetChild(0).GetComponent<InputField>().text);
                    break;
            }
#endif
            //print($"{flavorCases[x]} of {flavors[x]}");
        }
        for(int x = 0; x < flavorCases.Length; x++)
        {
            backup[x] = flavorCases[x];
        }
        sort = true;   
    }

    void SetCustomerInfo(bool back)
    {
        if (!back)
        {
            ci.c_name = customers[cd.value].company;
            ci.c_shipdate = infoPanel.GetChild(1).GetChild(0).GetComponent<InputField>().text;
            ci.c_freight = infoPanel.GetChild(2).GetChild(0).GetComponent<InputField>().text;
        }
        else
        {
            ci.c_name = "";
            ci.c_shipdate = "";
            ci.c_freight = "";
        }
    }

    public int[] GetFlavorCases()
    {
        return flavorCases;
    }

    public int[] GetBackupCases()
    {
        return backup;
    }

    public string[] GetFlavors()
    {
        return flavors;
    }

    public int[] GetRowsPerPallet()
    {
        return rowsPerPallet;
    }

    public CustomerInfo GetCustomerInfo()
    {
        return ci;
    }

    public int GetMaxHeight()
    {
        if (customers[cd.value].fullPallets) return 88;
        else return 81;
    }

    public bool GetSorting()
    {
        return sort;
    }

    public void SaveNewCustomer()
    {
        Transform panel = newCustomerPanel.GetChild(0).GetChild(0);
        Customer customer = new Customer()
        {
            company = panel.GetChild(0).GetChild(0).GetComponent<InputField>().text,
            street = panel.GetChild(1).GetChild(0).GetComponent<InputField>().text,
            city = panel.GetChild(2).GetChild(0).GetComponent<InputField>().text,
            state = panel.GetChild(3).GetChild(0).GetComponent<InputField>().text,
            zipcode = panel.GetChild(4).GetChild(0).GetComponent<InputField>().text,
            fullPallets = panel.GetChild(5).GetComponent<Toggle>().isOn
        };
        customers.Add(customer);
        //UpdateDropdown();
        newCustomerPanel.gameObject.SetActive(false);
        string file = "Assets/Loading Plan/Customer List.txt";
        using (StreamWriter stream = new StreamWriter(file,true))
        {
            stream.WriteLine(customer.company + "," + customer.street + "," + customer.city + "," + customer.state + "," + customer.zipcode + "," + customer.fullPallets);
        }
    }

    public void CloseNewCustomerPrompt()
    {
        newCustomerPanel.gameObject.SetActive(false);
    }

    public void OpenNewCustomerPrompt()
    {
        newCustomerPanel.gameObject.SetActive(true);
    }

    List<Customer> RetriveCustomerList()
    {
        List<Customer> customerList = new List<Customer>();
        string[] read;
        string file = "Assets/Loading Plan/Customer List.txt";
        read = File.ReadAllLines(file);

        for (int x = 0; x < read.Length; x++)
        {
            customerList.Add(new Customer() { company = read[x].Split(',')[0], street = read[x].Split(',')[1], city = read[x].Split(',')[2], state = read[x].Split(',')[3], zipcode = read[x].Split(',')[4], fullPallets = bool.Parse(read[x].Split(',')[5]) });
        }

        return customerList;
    }

    public void OnSubmit()
    {
        customerSelect.gameObject.SetActive(false);
        //cd.options.RemoveAt(0);
        /*cd.ClearOptions();
        dataList = new List<Dropdown.OptionData>();
        foreach (Customer cust in customers)
        {
            dataList.Add(new Dropdown.OptionData(cust.company));
        }
        cd.AddOptions(dataList);*/
        //cd.RefreshShownValue();
    }

    void UpdateScrollViewContent()
    {

        customerSelect.GetChild(0).GetChild(0);
    }

    /*void UpdateDropdown()
    {
        cd.ClearOptions();
        dataList = new List<Dropdown.OptionData>()
        {
            new Dropdown.OptionData("Choose Customer")
        };
        foreach(Customer cust in customers)
        {
            dataList.Add(new Dropdown.OptionData(cust.company));
        }
        cd.AddOptions(dataList);
    }*/

    public void OpenCustomerSelection()
    {
        customerSelect.gameObject.SetActive(true);
    }
}

public class CustomerInfo
{
    public string c_name;
    public string c_shipdate;
    public string c_freight;
}

public class Customer
{
    public string company, street, city, state, zipcode;
    public bool fullPallets;
}