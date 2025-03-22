using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toy_Protein_Ligand_Docking_Scorer
{
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
}
