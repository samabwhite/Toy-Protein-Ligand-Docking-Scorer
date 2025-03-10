using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toy_Protein_Ligand_Docking_Scorer
{
    public class Bond
    {
        public enum Bonds { SingleBond, DoubleBond, TripleBond }

        public  Atom[] Atoms { get; set; } = new Atom[2];
        
        public bool AromaticFlag;
        public bool StereoFlag;

        private Bonds BondType { get; set; }

        public Bond(Atom atom1, Atom atom2, int bondType, bool AromaticFlag = false, bool StereoFlag = false)
        {
            this.Atoms[0] = atom1;
            this.Atoms[1] = atom2;
            this.BondType = (Bonds)(bondType - 1);
            this.AromaticFlag = AromaticFlag;
            this.StereoFlag = StereoFlag;
            atom1.addAdjacent(atom2);
            atom2.addAdjacent(atom1);
        }
    }
}
