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
        protected string name;
        protected List<Atom> atoms;
        protected List<Bond> bonds;
        protected int atomCount;
        protected int bondCount;

        protected Molecule(string name, List<Atom> atoms = null, List<Bond> bonds = null, int atomCount = 0, int bondCount = 0)
        {
            this.name = name;
            this.atoms = atoms;
            this.bonds = bonds;
            this.atomCount = atomCount;
            this.bondCount = bondCount; 
        }

        public string getName() => this.name;

        public List<Atom> getAtoms() => this.atoms;

        public List<Bond> getBonds() => this.bonds;

        public int getAtomCount() => this.atomCount;

        public int getBondCount() => this.bondCount;

    }


    public class Ligand : Molecule
    {
        public Ligand(string name, List<Atom> atoms, List<Bond> bonds, int atomCount, int bondCount) : base(name, atoms, bonds, atomCount, bondCount) {}
    }

    public class Protein : Molecule
    {
        public Protein(string name, List<Atom> atoms = null) : base(name, atoms) {}
    }

    public class Atom
    {
        private char element;
        private double[] coordinates;
        private int atomNumber;
        private string atomType;
        private string residue;
        private int residueNumber;
        private char chain;
        private double occupancy;
        private double betaFactor;
        private bool heteroatom;

      
        public Atom(double x, double y, double z,  char element)
        {
            this.coordinates = new double[3] {x, y, z};
            this.element = element;
        }

        public Atom(double x, double y, double z, char element, int atomNumber, string atomType, string residue, int residueNumber, char chain, double occupancy, double betaFactor, bool heteroatom)
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

        public char getElement() => this.element;

        public double[] getCoordinates() => this.coordinates;

        public int getAtomNumber() => this.atomNumber;

        public string getAtomType() => this.atomType;

        public string getResidue() => this.residue;

        public int getResidueNumber() => this.residueNumber;

        public char getChain() => this.chain;

        public double getOccupancy() => this.occupancy;

        public double getBetaFactor() => this.betaFactor;

        public bool getHeteroatom() => this.heteroatom;
    }





    public class Bond
    {
        private Atom[] atoms = new Atom[2];
        public enum bonds { SingleBond, DoubleBond, TripleBond, AromaticBond }
        private bonds bondType;

        public Bond(Atom atom1, Atom atom2, int bondType) 
        {
            this.atoms[0] = atom1;
            this.atoms[1] = atom2;
            this.bondType = (bonds)(bondType-1);
        }

        public Atom[] getAtoms() => this.atoms;
        public bonds getBondType() => this.bondType;
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
                char element = char.Parse(dims[3]);
                atoms.Add(new Atom(x, y, z, element));
            }

            List<Bond> bonds = new List<Bond>();
            for (int i = 0; i < bondCount; i++)
            {
                string[] dims = Regex.Split(sr.ReadLine().Trim(), @"\s+");
                int atomIndex1 = int.Parse(dims[0]), atomIndex2 = int.Parse(dims[1]), bondType = int.Parse(dims[2]);
                bonds.Add(new Bond(atoms[atomIndex1-1], atoms[atomIndex2-1], bondType));
            }

            return new Ligand(moleculeName, atoms, bonds, atomCount, bondCount);
        }

        public static void CreateFromPDB(string fileDirectory) 
        {

            // PDB File Format - Atoms
            // record atom# atom type  residue  chain  residue#         XYZcoords         occupancy   beta factor   element
            // ATOM   3320     NH2       ARG      A       27      3.861  39.707  26.866     1.00        62.11          N  

            // atom type: 
            // residue: in a protein chain, each amino acid is called a residue
            // residue number: label given to each residue (amino acid) in the protein chain
            // chain: separate links of amino acids, kind of like a sub protein that folds and can link up with others to make larger proteins
            // beta factor: how much an atom seems to wiggle/vibrate 

            // TODO : Add file type verification
            // Add checks if file exists

            StreamReader sr = new StreamReader(fileDirectory);

            string name = Regex.Split(sr.ReadLine(), @"\s+")[3];

            string row;
            while ((row = sr.ReadLine()) != null) 
            {
                if (!row.StartsWith("ATOM") && !row.StartsWith("HETATM")) continue;

                // TODO : Replace with regex that separates per each data type in string. Columns touch with no white space separation once numbers become larger. i.e. ATOM12345 when it should be ATOM 12345
                string[] values = Regex.Split(row, @"\s+"); 
                Console.WriteLine(values[0]);
            }
            



        }
    }



}
