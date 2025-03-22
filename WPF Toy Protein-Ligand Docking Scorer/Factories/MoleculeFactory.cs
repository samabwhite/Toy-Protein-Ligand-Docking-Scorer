using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Toy_Protein_Ligand_Docking_Scorer.Bond;

namespace Toy_Protein_Ligand_Docking_Scorer
{
    public static class MoleculeFactory
    {
        public static Ligand CreateFromSDFFile(string fileDirectory)
        {
            // TODO : Add file type verification
            // Add checks if file exists

            StreamReader sr = new StreamReader(fileDirectory);

            string moleculeName = sr.ReadLine();

            for (int i = 0; i < 2; i++) sr.ReadLine(); // Skip 2 lines

            string[] items = Regex.Split(sr.ReadLine().Trim(), @"\s+");
            int atomCount = int.Parse(items[0]);
            int bondCount = int.Parse(items[1]);

            List<Atom> atoms = new List<Atom>();
            for (int i = 0; i < atomCount; i++)
            {
                string[] dims = Regex.Split(sr.ReadLine().Trim(), @"\s+");
                double x = double.Parse(dims[0]), y = double.Parse(dims[1]), z = double.Parse(dims[2]);
                string element = dims[3];
                atoms.Add(new Atom(x, y, z, element));
            }

            List<Bond> bonds = new List<Bond>();
            for (int i = 0; i < bondCount; i++)
            {
                string[] dims = Regex.Split(sr.ReadLine().Trim(), @"\s+");
                int atomIndex1 = int.Parse(dims[0]), atomIndex2 = int.Parse(dims[1]), bondType = int.Parse(dims[2]);
                // TODO : FIX
                bonds.Add(new Bond(atoms[atomIndex1 - 1], atoms[atomIndex2 - 1], bondType));
            }

            return new Ligand(moleculeName, atoms, bonds);
        }

        // This overloading version is used for instances where you want to use a cif file, but don't plan on manipulating it before
        // passing it to the CreateFromPDB file. For example, they don't plan on saving the dictionary so they just need to pass the directory.
        public static Protein CreateFromPDB(string pdbFileDirectory, string residueDictFileDirectory)
        {
            ResidueDictionary residueDictionary = ResidueDictionary.LoadResidueDictionary(residueDictFileDirectory);
            return CreateFromPDB(pdbFileDirectory, residueDictionary);
        }

        public static Protein CreateFromPDB(string pdbFileDirectory, ResidueDictionary residueDictionary)
        {
            // PDB File Format - Atoms
            // record atom# atom type  residue  chainId  residue#         XYZcoords         occupancy   beta factor   element
            // ATOM   3320     NH2       ARG      A       27      3.861  39.707  26.866     1.00        62.11          N  

            // TODO : Add file type verification
            // Add checks if file exists

            StreamReader sr = new StreamReader(pdbFileDirectory);

            string name = Regex.Split(sr.ReadLine(), @"\s+")[3];

            Dictionary<(char chainId, int resNum), Residue> residueMap = new Dictionary<(char chainId, int resNum), Residue>();
            List<Atom> atoms = new List<Atom>();
            List<Bond> bonds = new List<Bond>();
            HashSet<int> seenAtoms = new HashSet<int>();
            HashSet<string> seenResidues = new HashSet<string>(); // TODO: Remove since it is only used for debugging
            string row;
            while ((row = sr.ReadLine()) != null)
            {
                bool isHeteroatom;
                if ((isHeteroatom = row.StartsWith("HETATM")) || row.StartsWith("ATOM"))
                {
                    // Parsing with a regex split will cause issues due to some rows not containing white spaces when values get large.
                    // Instead of using regex to parse lines, use the PDBs standard index structure to skim substrings. This will parse faster than regex due to the lack of searching and instead indexing.
                    int atomNumber = int.Parse(row.Substring(6, 5));
                    string atomType = row.Substring(13, 3).TrimEnd();
                    string residue = row.Substring(17, 3).Trim();
                    char chainId = row[21];
                    int residueNumber = int.Parse(row.Substring(22, 4).TrimEnd());
                    double x = double.Parse(row.Substring(26, 12).Trim());
                    double y = double.Parse(row.Substring(38, 8).Trim());
                    double z = double.Parse(row.Substring(48, 8).Trim());
                    double occupancy = double.Parse(row.Substring(56, 4));
                    double betaFactor = double.Parse(row.Substring(60, 6));
                    string element = row.Substring(76, 2).Trim();

                    seenResidues.Add(residue); // TODO: Remove since it is only used for debugging

                    if (!residueMap.TryGetValue((chainId, residueNumber), out Residue res))
                    {
                        res = new Residue(residue, residueNumber, chainId);
                        residueMap.Add((chainId, residueNumber), res);
                    }

                    Atom newAtom = new Atom(x, y, z, element, atomNumber, atomType, res, residueNumber, chainId, occupancy, betaFactor, isHeteroatom);

                    res.addAtom(atomType, newAtom);

                    atoms.Add(newAtom);
                }
                else if (row.StartsWith("CONECT"))
                {
                    int neighborCount = (row.TrimEnd().Length - 11) / 5;
                    int atomIndex = int.Parse(row.Substring(6, 5)) - 1;
                    seenAtoms.Add(atomIndex);
                    for (int i = 0; i < neighborCount; i++)
                    {
                        int neighborIndex = int.Parse(row.Substring((11 + (5 * i)), 5)) - 1;
                        if (seenAtoms.Contains(neighborIndex)) continue;
                        int bondCount = 1; // TODO: This is temporarily set to 1, but this bondCount should be inferred based on other context clues
                        bonds.Add(new Bond(atoms[atomIndex], atoms[neighborIndex], bondCount));
                    }
                }
            }

            // Second pass for inferring bonds
            Dictionary<string, BondOrder> BondOrderMapping = new Dictionary<string, BondOrder>
            {
                { "SING", BondOrder.SingleBond },
                { "DOUB", BondOrder.DoubleBond },
                { "TRIP", BondOrder.TripleBond }
            };

            foreach (Atom atom in atoms)
            {

                List<ResidueDictionary.BondInfo> foundBonds = residueDictionary.getBonds(atom.residue.residueAbbrev, atom.atomType);
                foreach (ResidueDictionary.BondInfo bondInfo in foundBonds) {
                    if (atom.residue.atomTypes.ContainsKey(bondInfo.OtherAtomType)) // If the residue does not perfectly match and is missing atoms, skip those bonds
                    {
                        bonds.Add(new Bond(atom, atom.residue.atomTypes[bondInfo.OtherAtomType], (int)BondOrderMapping[bondInfo.BondType], bondInfo.AromaticFlag, bondInfo.StereoFlag));
                    }
                }
            }

            // DEBUGGING
            // Display seen residues
            foreach (string value in seenResidues)
            {
                Console.WriteLine(value);
            }
            Console.WriteLine(seenResidues.Count());

            // Display created bonds:
            foreach (Bond bond in bonds) 
            {
                Console.WriteLine(bond.Atoms[0].atomType + " " + bond.Atoms[1].atomType);
            }

            // Check for copied bonds 
            HashSet<(Atom, Atom)> bondSet = new HashSet<(Atom, Atom)>();
            foreach (Bond bond in bonds) 
            {
                (Atom, Atom) bondPair = (bond.Atoms[0], bond.Atoms[1]);
                (Atom, Atom) reverseBondPair = (bond.Atoms[1], bond.Atoms[0]);

                if (bondSet.Contains(bondPair) && bondSet.Contains(reverseBondPair))
                {
                    Console.WriteLine("DUPLICATE FOUND: " + bondPair.Item1.atomType + " " + bondPair.Item2.atomType);
                }
                else 
                {
                    bondSet.Add(bondPair);
                }
            }

            return new Protein(name, residueMap, atoms, bonds);
        }
    }
}
