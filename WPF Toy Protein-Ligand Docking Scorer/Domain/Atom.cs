using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toy_Protein_Ligand_Docking_Scorer
{
    public class Atom
    {
        public string element { get; set; }
        public double[] coordinates { get; set; }
        public int atomNumber { get; set; }
        public string atomType { get; set; }
        public Residue residue { get; set; }
        public int residueNumber { get; set; }
        public char chain { get; set; }
        public double occupancy { get; set; }
        public double betaFactor { get; set; }
        public bool heteroatom { get; set; }
        public List<Atom> adjacentAtoms { get; } = new List<Atom>();


        public Atom(double x, double y, double z, string element)
        {
            this.coordinates = new double[3] { x, y, z };
            this.element = element;
        }
        
        public Atom(double x, double y, double z, string element, int atomNumber, string atomType, Residue residue, int residueNumber, char chain, double occupancy, double betaFactor, bool heteroatom)
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
}
