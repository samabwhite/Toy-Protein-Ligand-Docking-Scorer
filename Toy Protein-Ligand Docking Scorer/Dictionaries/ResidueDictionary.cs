using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Windows.Markup;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;



namespace Toy_Protein_Ligand_Docking_Scorer
{
    public class ResidueDictionary
    {
        public class BondInfo 
        {
            public string OtherAtomType { get; set; }
            public string BondType { get; set; }
            public bool AromaticFlag { get; set; }
            public bool StereoFlag { get; set; }
            public BondInfo(string otherAtomType, string bondType, bool aromaticFlag, bool stereoFlag)
            {
                this.OtherAtomType = otherAtomType;
                this.BondType = bondType;
                this.AromaticFlag = aromaticFlag;
                this.StereoFlag = stereoFlag;
            }
        }

        // { ResidueName: { AtomType1 : bondInfo1, ...] } }
        // bondInfoX = (AtomyTypeX, BondType, AromaticFlag, StereoFlag)
        public Dictionary<string, Dictionary<string, List<BondInfo>>> mapping;

        public ResidueDictionary() {}

        public ResidueDictionary(string fileDirectory)
        {
            mapping = new Dictionary<string, Dictionary<string, List<BondInfo>>>();

            StreamReader sr = new StreamReader(fileDirectory);

            string row;
            while ((row = sr.ReadLine()) != null)
            {
                if (row.Length > 5 && row.Substring(0, 5).CompareTo("data_") == 0)
                {
                    string residueName = row.Substring(5);

                    if (!this.mapping.ContainsKey(residueName)) // add AtomType dictionary to new residue names
                    {
                        this.mapping[residueName] = new Dictionary<string, List<BondInfo>>();
                    }

                    while (sr.ReadLine().TrimEnd().CompareTo("_chem_comp_bond.pdbx_ordinal") != 0) {}; // Skip until the bond table

                    while (!(row = sr.ReadLine()).TrimEnd().StartsWith("#"))
                    {
                        string[] values = Regex.Split(row.TrimEnd(), @"\s+");

                        string atomType1 = values[1].Trim('\"');
                        string atomType2 = values[2].Trim('\"');
                        string bondType = values[3];
                        bool aromaticFlag = char.Parse(values[4]) == 'Y';
                        bool stereoFlag = char.Parse(values[5]) == 'Y';

                        if (!this.mapping[residueName].ContainsKey(atomType1)) // add List of paired atoms for new Atom Types
                        {
                            this.mapping[residueName][atomType1] = new List<BondInfo>();
                        }

                        if (!this.mapping[residueName].ContainsKey(atomType2)) // add reflective pair
                        {
                            this.mapping[residueName][atomType2] = new List<BondInfo>();
                        }

                        this.mapping[residueName][atomType1].Add(new BondInfo(atomType2, bondType, aromaticFlag, stereoFlag));
                        this.mapping[residueName][atomType2].Add(new BondInfo(atomType1, bondType, aromaticFlag, stereoFlag));
                    }
                }
            }
        }

        public List<BondInfo> getBonds(string residueName, string atomType)
        {
            return mapping[residueName][atomType]; 
        }

        public void StoreResidueDictionary(string newFileName) 
        {
            if (!newFileName.EndsWith(".json")) newFileName += ".json";

            string jsonString = JsonSerializer.Serialize(this.mapping);

            File.WriteAllText(newFileName.Trim(), jsonString);
        }

        public static ResidueDictionary LoadResidueDictionary(string fileDirectory) 
        {
            if (!File.Exists(fileDirectory)) 
            {
                throw new FileNotFoundException("Residue Dictionary file not found");
            }

            if (!fileDirectory.EndsWith(".json")) fileDirectory += ".json";


            ResidueDictionary residueDictionary = new ResidueDictionary();
            string loadedJson = File.ReadAllText(fileDirectory);

            residueDictionary.mapping = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<BondInfo>>>>(loadedJson);

            return residueDictionary;
        }
    }
}
