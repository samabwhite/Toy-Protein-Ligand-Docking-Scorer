using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Net;
using System.Text.RegularExpressions;
using System.Data;

namespace Toy_Protein_Ligand_Docking_Scorer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ResidueDictionary residueDictionary = new ResidueDictionary("C:/Users/samwh/source/repos/Toy Protein-Ligand Docking Scorer/components.cif");
            // residueDictionary.StoreResidueDictionary("components"); // BROKEN TOO LARGE

            //Ligand newLigand = MoleculeFactory.CreateFromSDFFile("C:/Users/samwh/source/repos/Toy Protein-Ligand Docking Scorer/AMO_ideal.sdf");
            MoleculeFactory.CreateFromPDB("C:/Users/samwh/source/repos/Toy Protein-Ligand Docking Scorer/1il2.pdb", residueDictionary);
            


        }
    }

}
