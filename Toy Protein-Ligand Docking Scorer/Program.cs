using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Net;
using System.Text.RegularExpressions;
using System.Data;

namespace Toy_Protein_Ligand_Docking_Scorer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Ligand newLigand = MoleculeFactory.CreateFromSDFFile("C:/Users/samwh/source/repos/Toy Protein-Ligand Docking Scorer/AMO_ideal.sdf");
            MoleculeFactory.CreateFromPDB("C:/Users/samwh/source/repos/Toy Protein-Ligand Docking Scorer/1il2.pdb");
            


        }
    }


    public abstract class Molecule
    {
        protected string name { get; set; }
        protected List<Atom> atoms { get; set; }
        protected List<Bond> bonds { get; set; }

        protected Molecule(string name, List<Atom> atoms = null, List<Bond> bonds = null)
        {
            this.name = name;
            this.atoms = atoms;
            this.bonds = bonds;
        }
    }


    public class Ligand : Molecule
    {
        public Ligand(string name, List<Atom> atoms, List<Bond> bonds) : base(name, atoms, bonds) {}
    }

    public class Protein : Molecule
    {
        public Protein(string name, List<Atom> atoms, List<Bond> bonds) : base(name, atoms, bonds) {}
    }

    public class Atom
    {
        private string element { get; set; }
        private double[] coordinates { get; set; }
        private int atomNumber { get; set; }
        private string atomType { get; set; }
        private string residue { get; set; }
        private int residueNumber { get; set; }
        private char chain { get; set; }
        private double occupancy { get; set; }
        private double betaFactor { get; set; }
        private bool heteroatom { get; set; }
        private List<Atom> adjacentAtoms { get; } = new List<Atom>();


        public Atom(double x, double y, double z,  string element)
        {
            this.coordinates = new double[3] {x, y, z};
            this.element = element;
        }

        public Atom(double x, double y, double z, string element, int atomNumber, string atomType, string residue, int residueNumber, char chain, double occupancy, double betaFactor, bool heteroatom)
        {
            this.element = element;
            this.coordinates = new double[3] { x, y, z };
            this.atomNumber = atomNumber;
            this.atomType = atomType;
            this.residue = residue;
            this.residueNumber = residueNumber;
            this.chain = chain;
            this.occupancy = occupancy;
            this.betaFactor = betaFactor;
            this.heteroatom = heteroatom;
        }

        public void addAdjacent(Atom neighbor) 
        { 
            this.adjacentAtoms.Add(neighbor);
        }
    }





    public class Bond
    {
        public enum bonds { SingleBond, DoubleBond, TripleBond, AromaticBond }

        private Atom[] atoms { get; set; } = new Atom[2];
        private bonds bondType { get; set; }

        // TODO : The bond constructor is incorrect. If makeReflections is off, there will be 1 bond per direction of a bond while reflection on will be 1 bond for both directions.
        public Bond(Atom atom1, Atom atom2, int bondType, bool makeReflections = false) 
        {
            this.atoms[0] = atom1;
            this.atoms[1] = atom2;
            this.bondType = (bonds)(bondType-1);
            atom1.addAdjacent(atom2);
            if (makeReflections) atom2.addAdjacent(atom1);
        }
    }


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
                bonds.Add(new Bond(atoms[atomIndex1-1], atoms[atomIndex2-1], bondType, true));
            }

            return new Ligand(moleculeName, atoms, bonds);
        }

        public static Protein CreateFromPDB(string fileDirectory) 
        {
            // PDB File Format - Atoms
            // record atom# atom type  residue  chain  residue#         XYZcoords         occupancy   beta factor   element
            // ATOM   3320     NH2       ARG      A       27      3.861  39.707  26.866     1.00        62.11          N  


            // TODO : Add file type verification
            // Add checks if file exists

            StreamReader sr = new StreamReader(fileDirectory);

            string name = Regex.Split(sr.ReadLine(), @"\s+")[3];

            List<Atom> atoms = new List<Atom>();
            List<Bond> bonds = new List<Bond>();
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
                    string residue = row.Substring(17, 3);
                    char chain = row[21];
                    int residueNumber = int.Parse(row.Substring(22, 4).TrimEnd());
                    double x = double.Parse(row.Substring(26, 12).Trim());
                    double y = double.Parse(row.Substring(38, 8).Trim());
                    double z = double.Parse(row.Substring(48, 8).Trim());
                    double occupancy = double.Parse(row.Substring(56, 4));
                    double betaFactor = double.Parse(row.Substring(60, 6));
                    string element = row.Substring(76, 2).Trim();

                    atoms.Add(new Atom(x, y, z, element, atomNumber, atomType, residue, residueNumber, chain, occupancy, betaFactor, isHeteroatom));
                }
                else if (row.StartsWith("CONECT")) // These bonds are only the outlier bonds and this block does not cover all bonds of the protein. The other bonds need to be inferred based on other information provided.
                {
                    int neighbors = (row.TrimEnd().Length - 11) / 5;
                    int atomIndex = int.Parse(row.Substring(6, 5)) - 1;
                    for (int i = 0; i < neighbors; i++) 
                    {
                        int neighborIndex = int.Parse(row.Substring((11 + (5*i)), 5)) - 1;
                        // TODO I'm leaving a placeholder 1 for all bonds, but this should be inferred based on the structure of the molecules amino acids
                        // TODO the Bond constructor is not correct, see bond class for details
                        // Solution will likely be a set of atoms that were already the first atom, these will be skipped as neighbors
                        bonds.Add(new Bond(atoms[atomIndex], atoms[neighborIndex], 1, false)); 
                    }
                }
                else continue;

                
            }
            return new Protein(name, atoms, bonds);
        }
    }

    public static class AminoAcidDictionary 
        { 
            // My idea for this dictionary is to contain a hashmap of amino acid objects {string : amino acid}
            // Each amino acid is going to contain a hash map of standard components of the amino acid (this is the atom type in PDB files) and the bond objects with other atom types and atoms. 
            // This structure of the dictionary should allow us to lookup and infer the bonds between neighboring atoms
            
        }



}
