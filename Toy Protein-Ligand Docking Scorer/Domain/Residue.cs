using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toy_Protein_Ligand_Docking_Scorer
{
    public class Residue
    {
        private int residueNumber { set; get; }
        private string abbrev { set; get; }
        private char chainId { set; get; }
        private Dictionary<string, List<Atom>> atomTypes = new Dictionary<string, List<Atom>>();

        public Residue(string abbrev, int residueNumber, char chainId) 
        {
            this.abbrev = abbrev;
            this.residueNumber = residueNumber;
            this.chainId = chainId;

        }

        public void addAtom(string atomType, Atom atom)
        {
            if (atomTypes.ContainsKey(atomType))
            {
                atomTypes[atomType].Add(atom);
            }
            else 
            {
                atomTypes.Add(atomType, new List<Atom>() { atom });
            }
        }

        // TODO : Add residue implementation and residue number, not sure if it should inherit molecule
        // It will be stored in an array inside a protein
        // it should contain an abbreviation
        // it should contain a reference to the mapping
    }
}
