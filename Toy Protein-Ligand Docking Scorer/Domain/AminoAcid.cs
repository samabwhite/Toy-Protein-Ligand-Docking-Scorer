using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toy_Protein_Ligand_Docking_Scorer
{
    public class AminoAcid : Molecule
    {
        public AminoAcid(string name, List<Atom> atoms, List<Bond> bonds) : base(name, atoms, bonds) { }
    }
}
