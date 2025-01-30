using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toy_Protein_Ligand_Docking_Scorer
{
    public class Atom
    {
        private string element { get; set; }
        private double[] coordinates { get; set; }
        private int atomNumber { get; set; }
        private string atomType { get; set; } // TODO : Make atomType an object type
        private string residue { get; set; }
        private int residueNumber { get; set; }
        private char chain { get; set; }
        private double occupancy { get; set; }
        private double betaFactor { get; set; }
        private bool heteroatom { get; set; }
        private List<Atom> adjacentAtoms { get; } = new List<Atom>();


        public Atom(double x, double y, double z, string element)
        {
            this.coordinates = new double[3] { x, y, z };
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
}
