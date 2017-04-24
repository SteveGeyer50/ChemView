/// @file    LoadPDBData.cs
/// @author	 Michael Kuhn (kuhnmic5@msu.edu)
/// @date    Fri Apr 21 12:51 EST 2017
/// @brief   Implimenting LoadPDBData class
///
/// This class should be able to load any PDB formatted file in .txt file into
/// Unity, and generate a 3D moluecule GameObject.
/// Code modified from LoadCmlData.cs, Thomas Bolden (boldenth@msu.edu)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Xml;
using System.IO;
using System;

public class LoadPdbData : MonoBehaviour
{

    //public GameObject HUD; // to display info about hovered over molecule

    static string ElementText; // eventually to be used for popup dialog

    public string path;

    // atomArray is a list of atoms in the molecule mapping the abbreviated
    // ptable name to its position
    // the order of atomArray is same as order in PDB file, so index in the
    // list will reference the atom (eg. atom a2 is second in list)
    // { "Ag" : (1.0, 2.0, 1.0) }
    List<Dictionary<string, Vector3>> atomArray
        = new List<Dictionary<string, Vector3>>(); // or <Element, Vector3> ?

    // bondArray is a list of all the bonds where each entry looks like
    // { "bond order" : "atom1 atom2" }


    // temporary dictionaries for atoms and bonds, respectively
    Dictionary<string, Vector3> tempDictA;
    Dictionary<List<string>, string> tempDictB;
    private List<string> row = new List< string> ();
    private string[] atom_array = new string[16];
    private string[] bond_array = new string[6];
    private List<string> line_ls = new List<string>();
    private string[][] row_array;
    
    private Vector3 atom_pos;
    private int a_count = 0;
    private int b_count = 0;
    private string id;
    List<string> tempListB;

    public TextAsset txtFile;

    public Dictionary<string, Vector3> atomPosDict 
        = new Dictionary<string, Vector3>();
    public Dictionary<string, string> atomTypeDict 
        = new Dictionary<string, string>();
    public List<Dictionary<List<string>, string>> bondArray
        = new List<Dictionary<List<string>, string>>();
    public string moleculeName; 

    void Start()
    {
        
        print("Start: " + path);
        GetComponent<PeriodicTable>().CreateTable();
        Read(path);
        GetComponent<GenerateMolecule>().
            Generate(atomPosDict, atomTypeDict, bondArray, moleculeName);

    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

    }

    public void Read(string path)
    {
        using (StreamReader sr = File.OpenText(path))
        {
            //Reads the txt file
            string line = "";

            while ((line = sr.ReadLine()) != null)
            {
                if ((line.Contains("CONECT")))
                {
                    int v = 0;
                    //Record name "CONECT"
                    line_ls.Add(line.Substring(v, 6).Trim());
                    v += 6;
                    //Atom 1
                    line_ls.Add(line.Substring(v, 5).Trim());
                    v += 5;
                    //Atom 2
                    line_ls.Add(line.Substring(v, 5).Trim());
                    v += 5;
                    //Atom 3
                    line_ls.Add(line.Substring(v, 5).Trim());
                    v += 5;
                    //Atom 4
                    line_ls.Add(line.Substring(v, 5).Trim());
                    v += 5;
                    //Atom 5
                    line_ls.Add(line.Substring(v, 5).Trim());
                    v += 5;
                    bond_array = line_ls.ToArray();



                    foreach (string bond in bond_array)
                    {
                        tempListB = new List<string>();
                        tempDictB = new Dictionary<List<string>, string>();
                        int bond_int = 0;
                        if (bond_int > 0 && bond != "0") {
                            tempListB.Add(bond_array[0]);
                            tempListB.Add(bond_array[bond_int]);
                        }
                        tempDictB.Add(tempListB, "Order");
                        bondArray.Add(tempDictB);
                        bond_int++;
                    }


                    b_count++;
                    /*for (int item =0;item < bond_array.Length; item++)
                    {
                        Debug.Log(bond_array[item]);
                    }*/

                    line_ls.Clear();
                }
                else if (line.Contains("ATOM"))
                {
                    int v = 0;
                    //0: Record Name "ATOM"
                    line_ls.Add(line.Substring(v, 6).Trim());
                    v += 6;
                    //1: Serial Number
                    line_ls.Add(line.Substring(v, 5).Trim());
                    v += 5;
                    //2: Atom Name
                    line_ls.Add(line.Substring(v, 5).Trim());
                    v += 5;
                    //3: Alternate Location Indicator
                    line_ls.Add(line.Substring(v, 1).Trim());
                    v += 1;
                    //4: Residue name
                    line_ls.Add(line.Substring(v, 3).Trim());
                    v += 3;
                    //5: Chain Identifier
                    line_ls.Add(line.Substring(v, 1).Trim());
                    v += 1;
                    //6: Residue Sequence Number
                    line_ls.Add(line.Substring(v, 4).Trim());
                    v += 4;
                    //7: Code for insertion of residues
                    line_ls.Add(line.Substring(v, 1).Trim());
                    v += 1;
                    //8: X coord
                    line_ls.Add(line.Substring(v, 12).Trim());
                    v += 12;
                    //9: Y coord
                    line_ls.Add(line.Substring(v, 8).Trim());
                    v += 8;
                    //10: Z coord
                    line_ls.Add(line.Substring(v, 8).Trim());
                    v += 8;
                    //11: Occupancy
                    line_ls.Add(line.Substring(v, 7).Trim());
                    v += 7;
                    //12: Tempature factor
                    line_ls.Add(line.Substring(v, 6).Trim());
                    v += 6;
                    //13: Element Symbol
                    line_ls.Add(line.Substring(v, 12).Trim());
                    v += 12;
                    //14: Charge
                    line_ls.Add(line.Substring(v, 1).Trim());

                    //Make array
                    atom_array = line_ls.ToArray();
                    atom_pos = new Vector3(float.Parse(atom_array[8]), 
                                           float.Parse(atom_array[9]), 
                                           float.Parse(atom_array[10]));
                    id = "a" + a_count.ToString();
                    atomTypeDict.Add(id, atom_array[2]);
                    atomPosDict.Add(id, atom_pos);
                    // DEBUG
                    Debug.Log(id);
                    Debug.Log(atom_pos);
                    Debug.Log(atom_array[2]);
                    line_ls.Clear();
                    a_count++;
                }
                else if (line.Contains("HEADER"))
                {
                    moleculeName= line.Substring(10, 50).Trim();
                }
            }

            /* Debug for above outputs
            Debug.Log("Molecule Name: " + moleculeName);
            foreach (var atom in atomPosDict)
                Debug.Log("Key: " + atom.Key + "\nValue: "+ atom.Value);
            bondArray.ForEach(x => 
            {
                foreach (var bond in x.Keys)
                {
                    Debug.Log("foreach(var bond in x)");
                    bond.ForEach(y => 
                    {
                        Debug.Log("bond.Key.ForEach(y");
                        Debug.Log(y);
                    });
                }
            });*/
            
            //row_array = row.ToArray();
        }
    }
}
