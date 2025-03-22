using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toy_Protein_Ligand_Docking_Scorer
{
    public class Protein : Molecule
    {
        private Dictionary<(char chainId, int resNum), Residue> residueMap { get; set; }
        public Protein(string name, Dictionary<(char chainId, int resNum), Residue> residueMap, List<Atom> atoms, List<Bond> bonds) : base(name, atoms, bonds) 
        {
            this.residueMap = residueMap;
        }


    }
}
