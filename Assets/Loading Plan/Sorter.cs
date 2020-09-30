using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class Sorter : UIManager
{

    /*const int CR = 0,
                        MN = 1,
                        MC = 2,
                        BCS = 3,
                        BCL = 4,
                        PZS = 5,
                        PZL = 6,
                        RAC = 7;

    const int CRCO = 0, CRV = 1, CRS = 2, CRC = 3, CRPJ = 4, CRPC = 5,
                        MNCO = 6, MNV = 7,
                        MCCO = 8, MCV = 9, MCCH = 10,
                        BCSG = 11, BCSS = 12, BCSE = 13, BCSC = 14,
                        BCLG = 15, BCLS = 16, BCLE = 17, BCLC = 18,
                        PZSO = 19, PZSG = 20, PZST = 21,
                        PZLO = 22, PZLG = 23, PZLT = 24,
                        RACK = 25;

    int[] startIndex = new int[] { 0, 6, 8, 11, 15, 19, 22, 25 };
    int[] stopIndex = new int[] { 6, 8, 11, 15, 19, 22, 25, 26 };*/
    readonly float[] AREA = new float[]
    {
        100 /  9, 100 /  9, 100 /  9, 100 /  9, 100 /  9, 100 /  9,
        100 / 10, 100 / 10,
        100 /  9, 100 /  9, 100 /  9,
        100 / 15, 100 / 15, 100 / 15,
        100 /  8, 100 /  8,
        100 / 16, 100 / 16, 100 / 16, 100 / 16,
        100 / 11, 100 / 11, 100 / 11, 100 / 11,
        100 / 16, 100 / 16, 100 / 16,
        100 / 12, 100 / 12, 100 / 12,
        100 /  4,
        100 /  10, 100 /  10, 100 /  5, 100 /  5
    };
    readonly float[] HEIGHT = new float[] { 7.5f, 7.5f, 7.5f, 7.5f, 7.5f, 7.5f, 7.5f, 7.5f, 11, 11, 11, 7.25f, 7.25f, 7.25f, 10.5f, 10.5f, 9, 9, 9, 9, 10, 10, 10, 10, 9, 9, 9, 10, 10, 10, 2, 21, 22.5f, 24.5f, 24.5f };
    readonly int[] rows = new int[35], PERROW = new int[] { 9, 9, 9, 9, 9, 9, 10, 10, 9, 9, 9, 15, 15, 15, 8, 8, 16, 16, 16, 16, 11, 11, 11, 11, 16, 16, 16, 12, 12, 12, 4, 10, 10, 5, 5 };
    //readonly int[] cases = new int[8];


    List<Pallet> pallets = new List<Pallet>();
    List<int[]> mixedRows = new List<int[]>();

    int tracker = 0, track = 1;

    /*void Start()
    {
        cases[CR] = flavorCases[CRCO] + flavorCases[CRV] + flavorCases[CRS] + flavorCases[CRC] + flavorCases[CRPJ] + flavorCases[CRPC];
        cases[MN] = flavorCases[MNCO] + flavorCases[MNV];
        cases[MC] = flavorCases[MCCO] + flavorCases[MCV] + flavorCases[MCCH];
        cases[BCS] = flavorCases[BCSG] + flavorCases[BCSS] + flavorCases[BCSE] + flavorCases[BCSC];
        cases[BCL] = flavorCases[BCLG] + flavorCases[BCLS] + flavorCases[BCLE] + flavorCases[BCLC];
        cases[PZS] = flavorCases[PZSO] + flavorCases[PZSG] + flavorCases[PZST];
        cases[PZL] = flavorCases[PZLO] + flavorCases[PZLG] + flavorCases[PZLT];
        cases[RAC] = flavorCases[RACK];

    }*/

    private void Update()
    {
        if (GetSorting())
        {
            switch (track)
            {
                case 1:
                    CreateFullPallets();
                    break;
                case 2:
                    ListFullPallets();
                    BuildMixedRows();
                    break;
                /*case 3:
                    CreateMixedPallets();
                    break;
                case 4:
                    TryAddCase();
                    break;
                case 5:
                    OutputHTML();
                    break;*/
            }
        }
    }

    private void OutputHTML()
    {
        string file = "Assets/Loading Plan/Loading Plan.html";
        int num = 1;
        using (StreamWriter filestream = new StreamWriter(file))
        {
            filestream.Write("<!DOCTYPE html><html><head><title>Loading Plan</title></head><body>");
            filestream.Write("<h1>Customer: " + GetCustomerInfo().c_name + "<br>Ship Date: " + GetCustomerInfo().c_shipdate + "<br>FreightType: " + GetCustomerInfo().c_freight + "</h1>");
            filestream.Write("<h2>Total Pallet Count: " + pallets.Count + "</h2>");

            for(int x = 0; x < ListFullPallets().Length; x++)
            {
                if(ListFullPallets()[x] > 0)
                {

                }
            }

            foreach (Pallet pallet in pallets)
            {
                if (pallet.full == "Mixed")
                {
                    filestream.Write("<p><b>Pallet " + pallet.num + ": </b><br><br>");
                    for (int j = 0; j < pallet.flavorQty.Length; j++)
                    {
                        if (pallet.flavorQty[j] > 0)
                        {
                            if (pallet.flavorQty[j] == 1) filestream.Write(pallet.flavorQty[j] + " - case of " + GetFlavors()[j] + "<br>");
                            else filestream.Write(pallet.flavorQty[j] + " - cases of " + GetFlavors()[j] + "<br>");
                        }
                    }
                    filestream.Write("<br>");
                    filestream.Write("</p>");
                    /*if(pallet.full != "Mixed")
                    {
                        print(pallet.full);
                    }*/
                }
            }
            /*for(int x = 0; x < GetBackupCases().Length; x++)
            {
                if(GetBackupCases()[x] != 0) filestream.WriteLine(GetBackupCases()[x] + " cases of " +  GetFlavors()[x] + "<br>");
            }
            filestream.Write("</p>");*/
            filestream.Write("</body></html>");
        }
        //CreateEmail.SendAnEmail("Assets/Loading Plan/Loading Plan.html", GetCustomerInfo().c_name);
        track = 6;
    }

    private void CreateFullPallets()
    {
        for (int i = 0; i < GetFlavorCases().Length; i++)   //Loop through all available products
        {
            int fullPallets;
            rows[i] = GetFlavorCases()[i] / PERROW[i];  //Current Row = Qty of Current Flavor / Cases per Row
            GetFlavorCases()[i] -= rows[i] * PERROW[i]; //Subtract Current Row x Cases per Row from Qty of Current Flavor
            fullPallets = rows[i] / GetRowsPerPallet()[i];  //Current Row / Rows per Pallet for Current Flavor - Full Pallets
            rows[i] -= fullPallets * GetRowsPerPallet()[i]; //Subtract Full Pallets for Current Flavor x Rows per Pallet for Current Flavor
            for (int j = 0; j < fullPallets; j++)   //Loop through each Full Pallet for Current Flavor
            {
                Pallet pallet = new Pallet() { num = ++tracker, complete = true, full = GetFlavors()[i] };  //Create new Pallet(num is pallet number, complete means nothing else will fit, full is the flavor)
                pallet.flavorQty[i] = GetRowsPerPallet()[i] * PERROW[i];    //Set Pallets Current Flavor Qty to Current Rows per Pallet x Current Cases per Row
                pallets.Add(pallet);    //Add Pallet to Pallet Array
            }
        }
        track = 2;
    }

    private int[] ListFullPallets()
    {
        int[] palletList = new int[35];
        foreach (Pallet pallet in pallets)
        {
            if (pallet.complete && pallet.full != "Mixed")
            {
                for (int i = 0; i < pallet.flavorQty.Length; i++)
                {
                    if(pallet.flavorQty[i] > 0)
                    {
                        palletList[i] += 1;
                        break;
                    }
                }
            }
        }
        return palletList;
    }

    private void CreateMixedPallets()
    {
        int i = 0;
        int[] index = new int[] { 0, 0, 0 };
        float step;
        Pallet pallet = new Pallet() { num = ++tracker };

        i = GetNonZeroRow(i);
        while (rows[i] > 0)
        {
            step = HEIGHT[i] / GetMaxHeight();

            if (CanAddRow(step, pallet))
            {
                rows[i]--;
                pallet.fill += step;
                pallet.flavorQty[i] += PERROW[i];
            }
            if (pallet.complete = IsPalletComplete(pallet))
            {
                pallets.Add(pallet);
                pallet = new Pallet() { num = ++tracker };
                i = GetNonZeroRow(0);
                continue;
            }

            if (rows[i] == 0)
            {
                i = GetNonZeroRow(i);
            }

            if (i == index[0] && pallet.num == index[1] && rows[i] == index[2])
            {
                pallets.Add(pallet);
                pallet = new Pallet() { num = ++tracker };
                i = GetNonZeroRow(0);
                continue;
            }
            index[0] = i;
            index[1] = pallet.num;
            index[2] = rows[i];
        }
        if (!pallet.complete && pallet.fill > 0)
        {
            pallets.Add(pallet);
        }
        track = 4;
    }

    private void TryAddCase()
    {
        foreach(Pallet skid in pallets)
        {
            if(!skid.complete)
            {
                Pallet pallet = skid;
                foreach (int[] row in mixedRows)
                {
                    float step = GetHeight(row) / GetMaxHeight();
                    if (pallet != null)
                    {
                        if (CanAddRow(step, pallet))
                        {
                            for (int i = 0; i < row.Length; i++)
                            {
                                pallet.flavorQty[i] += row[i];
                                row[i] = 0;
                            }
                            pallet.fill += step;
                            if (pallet.complete = IsPalletComplete(pallet))
                            {
                                pallet = GetIncompletePallet();
                            }
                        }
                    }
                    else
                    {
                        pallet = new Pallet() { num = ++tracker };

                        for (int i = 0; i < row.Length; i++)
                        {
                            pallet.flavorQty[i] += row[i];
                            row[i] = 0;
                        }
                        pallet.fill += step;

                        if (pallet.complete = IsPalletComplete(pallet) || pallet.fill > 0)
                        {
                            pallets.Add(pallet);
                        }
                    }
                }
            }
        }
        track = 5;
    }

    private void BuildMixedRows()
    {
        int[] newRow = new int[]
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };
        float fill = 0.0f;

        for (int i = 0; i < GetFlavorCases().Length; i++)
        {
            if (GetFlavorCases()[i] > 0)
            {
                if (fill < 95)
                {
                    while (GetFlavorCases()[i] > 0 && fill < 95)
                    {
                        newRow[i] += 1;
                        fill += AREA[i];
                        GetFlavorCases()[i] -= 1;
                    }
                    if (fill >= 95)
                    {
                        mixedRows.Add(newRow);
                        newRow = new int[]
                        {
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
                        };
                        fill = 0;
                        i--;
                    }
                }
            }
        }
        if (fill != 0)
        {
            mixedRows.Add(newRow);
        }
        track = 3;
    }

    private float GetHeight(int[] row)
    {
        if (row[31] > 0 || row[32] > 0)
        {
            return 24.5f;
        }
        else if (row[30] > 0)
        {
            return 22.5f;
        }
        else if (row[29] > 0)
        {
            return 21;
        }
        else if (row[8] > 0 || row[9] > 0 || row[10] > 0)   //3 Micro Croissants
        {
            return 11;
        }
        else if (row[25] > 0 || row[26] > 0 || row[27] > 0) //3 Large Pizzeti
        {
            return 10.5f;
        }
        else if (row[18] > 0 || row[19] > 0 || row[20] > 0 || row[21] > 0)  //4 Large Bagel Chips
        {
            return 9.5f;
        }
        else if (row[14] > 0 || row[15] > 0 || row[16] > 0 || row[17] > 0 || row[22] > 0 || row[23] > 0 || row[24] > 0) //7 Small Bagel Chips/Pizzeti
        {
            return 8.5f;
        }
        else if (row[0] > 0 || row[1] > 0 || row[2] > 0 || row[3] > 0 || row[4] > 0 || row[5] > 0 || row[6] > 0 || row[7] > 0)    //8 Croissant/Mini Croissant
        {
            return 7.5f;
        }
        else if (row[28] > 0)   //1 Rack
        {
            return 3;
        }
        return 0;
    }

    private bool CanAddRow(float step, Pallet pallet)
    {
        if (step + pallet.fill > 1)
        {
            return false;
        }
        return true;
    }

    private Pallet GetIncompletePallet()
    {
        foreach (Pallet pallet in pallets)
        {
            if (!pallet.complete)
            {
                return pallet;
            }
        }
        return null;
    }

    private bool IsPalletComplete(Pallet pallet)
    {
        for (int i = 0; i < rows.Length; i++)
        {
            if (rows[i] != 0 && HEIGHT[i] / GetMaxHeight() + pallet.fill < 1) //If there are still rows left and the row height will not exceed maximum height
            {
                return false;
            }
        }
        foreach (int[] row in mixedRows)
        {
            float step = GetHeight(row) / GetMaxHeight();
            for (int i = 0; i < row.Length; i++)
            {
                if (row[i] != 0 && step + pallet.fill < 1)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private int GetNonZeroRow(int type) //Gets the next non-zero row
    {
        while (rows[type] == 0 && type < rows.Length - 1)
        {
            type++;
        }
        return type;
    }

    private int GetNonZeroCase(int type) //Gets the next non-zero row
    {
        while (GetFlavorCases()[type] == 0 && type < GetFlavorCases().Length - 1)
        {
            type++;
        }
        return type;
    }
}

public class Pallet
{
    public int[] flavorQty = new int[33];
    public bool complete = false;
    public string full = "Mixed";
    public float fill;
    public int num;
}