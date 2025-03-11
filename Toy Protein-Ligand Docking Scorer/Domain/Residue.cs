using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toy_Protein_Ligand_Docking_Scorer
{
    public class Residue
    {
        public int residueNumber { set; get; }
        public string residueAbbrev { set; get; }
        public char chainId { set; get; }
        public Dictionary<string, Atom> atomTypes = new Dictionary<string, Atom>();

        public Residue(string residueAbbrev, int residueNumber, char chainId) 
        {
            this.residueAbbrev = residueAbbrev;
            this.residueNumber = residueNumber;
            this.chainId = chainId;

        }

        public void addAtom(string atomType, Atom atom)
        {
            atomTypes[atomType] = atom;
        }

        // TODO : Add residue implementation and residue number, not sure if it should inherit molecule
        // It will be stored in an array inside a protein
        // it should contain an abbreviation
        // it should contain a reference to the mapping
    }
}
