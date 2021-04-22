#region Copyright � ThotLab Games 2011. Licensed under the terms of the Microsoft Reciprocal Licence (Ms-RL).

// Microsoft Reciprocal License (Ms-RL)
//
// This license governs use of the accompanying software. If you use the software, you accept this
// license. If you do not accept the license, do not use the software.
//
// 1. Definitions
// The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same
// meaning here as under U.S. copyright law.
// A "contribution" is the original software, or any additions or changes to the software.
// A "contributor" is any person that distributes its contribution under this license.
// "Licensed patents" are a contributor's patent claims that read directly on its contribution.
//
// 2. Grant of Rights
// (A) Copyright Grant- Subject to the terms of this license, including the license conditions and
// limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free
// copyright license to reproduce its contribution, prepare derivative works of its contribution,
// and distribute its contribution or any derivative works that you create.
// (B) Patent Grant- Subject to the terms of this license, including the license conditions and
// limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free
// license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or
// otherwise dispose of its contribution in the software or derivative works of the contribution in
// the software.
//
// 3. Conditions and Limitations
// (A) Reciprocal Grants- For any file you distribute that contains code from the software (in
// source code or binary format), you must provide recipients the source code to that file along
// with a copy of this license, which license will govern that file. You may license other files
// that are entirely your own work and do not contain code from the software under any terms you
// choose.
// (B) No Trademark License- This license does not grant you rights to use any contributors' name,
// logo, or trademarks.
// (C) If you bring a patent claim against any contributor over patents that you claim are
// infringed by the software, your patent license from such contributor to the software ends
// automatically.
// (D) If you distribute any portion of the software, you must retain all copyright, patent,
// trademark, and attribution notices that are present in the software.
// (E) If you distribute any portion of the software in source code form, you may do so only under
// this license by including a complete copy of this license with your distribution. If you
// distribute any portion of the software in compiled or object code form, you may only do so under
// a license that complies with this license.
// (F) The software is licensed "as-is." You bear the risk of using it. The contributors give no
// express warranties, guarantees or conditions. You may have additional consumer rights under your
// local laws which this license cannot change. To the extent permitted under your local laws, the
// contributors exclude the implied warranties of merchantability, fitness for a particular purpose
// and non-infringement.

#endregion Copyright � ThotLab Games 2011. Licensed under the terms of the Microsoft Reciprocal Licence (Ms-RL).

using System;
using System.Collections.Generic;

namespace Utility.Fuzzy {
    /// <summary>
    ///     Class for fuzzy OR.
    /// </summary>
    public class FzOr : FuzzyTerm {
        /// <summary>
        ///     Initializes a new instance of the FzOr class from the given FzOr term.
        /// </summary>
        /// <param name="fzOr">The given FzOr term.</param>
        public FzOr(FzOr fzOr){
            Terms = new List<FuzzyTerm>();
            foreach (var term in fzOr.Terms){
                Terms.Add(term.Clone());
            }
        }

        /// <summary>
        ///     Initializes a new instance of the FzOr class using two terms.
        /// </summary>
        /// <param name="op1">The first fuzzy term.</param>
        /// <param name="op2">The second fuzzy term.</param>
        public FzOr(FuzzyTerm op1, FuzzyTerm op2){
            Terms = new List<FuzzyTerm>{op1.Clone(), op2.Clone()};
        }

        /// <summary>
        ///     Initializes a new instance of the FzOr class using three terms.
        /// </summary>
        /// <param name="op1">The first fuzzy term.</param>
        /// <param name="op2">The second fuzzy term.</param>
        /// <param name="op3">The third fuzzy term.</param>
        public FzOr(FuzzyTerm op1, FuzzyTerm op2, FuzzyTerm op3){
            Terms = new List<FuzzyTerm>{op1.Clone(), op2.Clone(), op3.Clone()};
        }

        /// <summary>
        ///     Initializes a new instance of the FzOr class using four terms.
        /// </summary>
        /// <param name="op1">The first fuzzy term.</param>
        /// <param name="op2">The second fuzzy term.</param>
        /// <param name="op3">The third fuzzy term.</param>
        /// <param name="op4">The fourth fuzzy term.</param>
        public FzOr(FuzzyTerm op1, FuzzyTerm op2, FuzzyTerm op3, FuzzyTerm op4){
            Terms = new List<FuzzyTerm>{op1.Clone(), op2.Clone(), op3.Clone(), op4.Clone()};
        }

        /// <summary>
        ///     Gets the list of terms. An instance of this class may AND together up to 4 terms.
        /// </summary>
        public List<FuzzyTerm> Terms { get; }

        /// <summary>
        ///     Clone this FzOr term.
        /// </summary>
        /// <returns>A cop of this FzOr term.</returns>
        public override FuzzyTerm Clone(){
            return new FzOr(this);
        }

        /// <summary>
        ///     The OR operator returns the maximum DOM of the sets it is operating on.
        /// </summary>
        /// <returns>The maximum DOM of the ORed sets.</returns>
        public override float GetDom(){
            var largest = float.MinValue;

            foreach (var term in Terms){
                if (term.GetDom() > largest) largest = term.GetDom();
            }

            return largest;
        }

        /// <summary>
        ///     Clears the degree of membership of the term.
        /// </summary>
        /// <remarks>
        ///     Unused.
        /// </remarks>
        public override void ClearDom(){
            throw new Exception("FzOR.ClearDOM: invalid context.");
        }

        /// <summary>
        ///     Method for updating the DOM of a consequent when a rule fires.
        /// </summary>
        /// <remarks>
        ///     Unused.
        /// </remarks>
        /// <param name="givenValue">The given value.</param>
        public override void OrWithDom(float givenValue){
            throw new Exception("FzOR.OrWithDom: invalid context.");
        }
    }
}