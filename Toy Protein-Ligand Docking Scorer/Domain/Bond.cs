using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toy_Protein_Ligand_Docking_Scorer
{
    public class Bond
    {
        public enum bonds { SingleBond, DoubleBond, TripleBond, AromaticBond }

        private Atom[] atoms { get; set; } = new Atom[2];
        private bonds bondType { get; set; }

        public Bond(Atom atom1, Atom atom2, int bondType)
        {
            this.atoms[0] = atom1;
            this.atoms[1] = atom2;
            this.bondType = (bonds)(bondType - 1);
            atom1.addAdjacent(atom2);
            atom2.addAdjacent(atom1);
        }
    }
}
