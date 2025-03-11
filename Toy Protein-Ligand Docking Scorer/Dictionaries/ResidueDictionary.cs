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
using System.Xml.Serialization;


namespace Toy_Protein_Ligand_Docking_Scorer
{
    public class ResidueDictionary
    {
        // { ResidueName: { AtomType1 : [pairedToAtomType2, pairedToAtomType3, pairedToAtomType4] } }
        // pairedToAtomyType = (AtomyTypeX, BondType, AromaticFlag, StereoFlag)
        public Dictionary<string, Dictionary<string, List<(string, string, bool, bool)>>> mapping;

        public ResidueDictionary() {}

        public ResidueDictionary(string fileDirectory)
        {
            mapping = new Dictionary<string, Dictionary<string, List<(string, string, bool, bool)>>>();

            StreamReader sr = new StreamReader(fileDirectory);

            string row;
            while ((row = sr.ReadLine()) != null)
            {
                if (row.Length > 5 && row.Substring(0, 5).CompareTo("data_") == 0)
                {
                    string residueName = row.Substring(5);

                    if (!this.mapping.ContainsKey(residueName)) // add AtomType dictionary to new residue names
                    {
                        this.mapping[residueName] = new Dictionary<string, List<(string, string, bool, bool)>>();
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
                            this.mapping[residueName][atomType1] = new List<(string, string, bool, bool)>();
                        }

                        if (!this.mapping[residueName].ContainsKey(atomType2)) // add reflective pair
                        {
                            this.mapping[residueName][atomType2] = new List<(string, string, bool, bool)>();
                        }

                        this.mapping[residueName][atomType1].Add((atomType2, bondType, aromaticFlag, stereoFlag));
                        this.mapping[residueName][atomType2].Add((atomType1, bondType, aromaticFlag, stereoFlag));
                    }
                }
            }
        }

        public List<(string, string, bool, bool)> getBonds(string residueName, string atomType)
        {
            return mapping[residueName][atomType];
        }

        public void StoreResidueDictionary(string newFileName) 
        {
            Stream stream = File.Open(newFileName.TrimEnd() + ".dat", FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();

            bf.Serialize(stream, this.mapping);
            stream.Close();

            this.mapping = null;
        }

        public static ResidueDictionary LoadResidueDictionary(string fileDirectory) 
        {
            if (!File.Exists(fileDirectory)) 
            {
                throw new FileNotFoundException("Residue Dictionary file not found");
            }

            Stream stream = File.Open(fileDirectory, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();

            ResidueDictionary residueDictionary = new ResidueDictionary();
            residueDictionary.mapping = (Dictionary<string, Dictionary<string, List<(string, string, bool, bool)>>>)bf.Deserialize(stream);

            stream.Close();
            return residueDictionary;
        }
    }
}
