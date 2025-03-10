using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

        public static Protein CreateFromPDB(string fileDirectory)
        {
            // PDB File Format - Atoms
            // record atom# atom type  residue  chainId  residue#         XYZcoords         occupancy   beta factor   element
            // ATOM   3320     NH2       ARG      A       27      3.861  39.707  26.866     1.00        62.11          N  


            // TODO : Add file type verification
            // Add checks if file exists

            StreamReader sr = new StreamReader(fileDirectory);

            string name = Regex.Split(sr.ReadLine(), @"\s+")[3];

            Dictionary<(char chainId, int resNum), Residue> residueMap = new Dictionary<(char chainId, int resNum), Residue>();
            List<Atom> atoms = new List<Atom>();
            List<Bond> bonds = new List<Bond>();
            HashSet<int> seenAtoms = new HashSet<int>();
            HashSet<string> seenResidues = new HashSet<string>();
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

                    seenResidues.Add(residue);

                    Atom newAtom = new Atom(x, y, z, element, atomNumber, atomType, residue, residueNumber, chainId, occupancy, betaFactor, isHeteroatom);

                    Residue res;
                    if (!residueMap.TryGetValue((chainId, residueNumber), out res))
                    {
                        res = new Residue(residue, residueNumber, chainId);
                        residueMap.Add((chainId, residueNumber), res);
                    }
                    res.addAtom(atomType, newAtom);

                    atoms.Add(newAtom);

                    // Determine Bond
                    // TODO I'm leaving a placeholder 1 for all bonds, but this should be inferred based on the structure of the molecules amino acids


                    // Create Bond

                }
                else if (row.StartsWith("CONECT")) // These bonds are only the outlier bonds and this block does not cover all bonds of the protein. The other bonds need to be inferred based on other information provided.
                {
                    int neighbors = (row.TrimEnd().Length - 11) / 5;
                    int atomIndex = int.Parse(row.Substring(6, 5)) - 1;
                    seenAtoms.Add(atomIndex);
                    for (int i = 0; i < neighbors; i++)
                    {
                        int neighborIndex = int.Parse(row.Substring((11 + (5 * i)), 5)) - 1;
                        if (seenAtoms.Contains(neighborIndex)) continue;
                        bonds.Add(new Bond(atoms[atomIndex], atoms[neighborIndex], 1));
                    }
                }
            }
            

            // Display seen residues
            foreach (string value in seenResidues)
            {
                Console.WriteLine(value);
            }
            Console.WriteLine(seenResidues.Count());

            // Check for copied bonds 
            HashSet<(Atom, Atom)> bondSet = new HashSet<(Atom, Atom)>();
            foreach (Bond bond in bonds) 
            {
                (Atom, Atom) bondPair = (bond.Atoms[0], bond.Atoms[1]);
                (Atom, Atom) reverseBondPair = (bond.Atoms[1], bond.Atoms[0]);

                if (bondSet.Contains(bondPair) || bondSet.Contains(reverseBondPair))
                {
                    Console.WriteLine("DUPLICATE FOUND: " + bondPair);
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
