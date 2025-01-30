using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toy_Protein_Ligand_Docking_Scorer
{
    public static class AminoAcidDictionary
    {
        // My idea for this dictionary is to contain a hashmap of amino acid objects {string : amino acid}
        // Each amino acid is going to contain a hash map of standard components of the amino acid (this is the atom type in PDB files) and the bond objects with other atom types and atoms. 
        // This structure of the dictionary should allow us to lookup and infer the bonds between neighboring atoms

    }
}
