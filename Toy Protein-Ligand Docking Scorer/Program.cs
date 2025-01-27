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

namespace Toy_Protein_Ligand_Docking_Scorer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Ligand newLigand = LigandFactory.CreateFromSDFFile("C:/Users/samwh/source/repos/Toy Protein-Ligand Docking Scorer/AMO_ideal.sdf");
            



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

    class Protein : Molecule
    {
        public Protein(string name, List<Atom> atoms = null) : base(name, atoms) {}
    }

    public class Atom
    {
        private char element;
        private double[] coordinates;
      
        public Atom(double x, double y, double z,  char element)
        {
            this.coordinates = new double[3] {x, y, z};
            this.element = element;
        }

        public double[] getCoordinates() => this.coordinates;

        public char getElement() => this.element;
    }

    public class Bond
    {
        private Atom[] atoms = new Atom[2];
        private enum bonds { SingleBond, DoubleBond, TripleBond, AromaticBond }
        private bonds bondType;

        public Bond(Atom atom1, Atom atom2, int bondType) 
        {
            this.atoms[0] = atom1;
            this.atoms[1] = atom2;
            this.bondType = (bonds)(bondType-1);
        }
    }


    public static class LigandFactory
    {
        public static Ligand CreateFromSDFFile(string fileDirectory)
        {
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
    }








}
