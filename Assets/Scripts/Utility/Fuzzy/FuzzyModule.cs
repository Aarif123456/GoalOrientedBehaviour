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
    ///     This class describes a fuzzy module: a collection of fuzzy variables and the rules that
    ///     operate on them.
    /// </summary>
    public class FuzzyModule {
        /// <summary>
        ///     You must pass one of these values to the <see cref="DeFuzzify" />method. This module only
        ///     supports the MaxAv and Centroid methods.
        /// </summary>
        public enum DefuzzifyMethod {
            MaxAv,
            Centroid
        }

        /// <summary>
        ///     When calculating the centroid of the fuzzy manifold this value is used to determine how
        ///     many cross-sections should be sampled.
        /// </summary>
        public const int CROSS_SECTION_SAMPLE_COUNT = 15;

        /// <summary>
        ///     Initializes a new instance of the FuzzyModule class.
        /// </summary>
        public FuzzyModule(){
            Rules = new List<FuzzyRule>();
            Variables = new Dictionary<string, FuzzyVariable>();
        }

        /// <summary>
        ///     Gets a map of all the fuzzy variables this module uses.
        /// </summary>
        public Dictionary<string, FuzzyVariable> Variables { get; }

        /// <summary>
        ///     Gets a list containing all the fuzzy rules.
        /// </summary>
        public List<FuzzyRule> Rules { get; }

        /// <summary>
        ///     This method calls the Fuzzify method of the variable with the same name as the key.
        /// </summary>
        /// <param name="nameOfFlv">Name of the fuzzy linguistic variable.</param>
        /// <param name="val">The value of the fuzzy linguistic variable.</param>
        public void Fuzzify(string nameOfFlv, float val){
            // first make sure the key exists
            if (!Variables.ContainsKey(nameOfFlv)){
                throw new Exception("FuzzyModule.Fuzzify>: key not found.");
            }

            Variables[nameOfFlv].Fuzzify(val);
        }

        /// <summary>
        ///     Given a fuzzy variable and a defuzzification method this returns a crisp value.
        /// </summary>
        /// <param name="nameOfFlv">Name of the fuzzy linguistic variable.</param>
        /// <param name="method">The defuzzification method.</param>
        /// <returns>A crisp value.</returns>
        public float DeFuzzify(string nameOfFlv, DefuzzifyMethod method){
            // first make sure the key exists
            if (!Variables.ContainsKey(nameOfFlv)){
                throw new Exception("FuzzyModule.DeFuzzify: key not found.");
            }

            // clear the DOMs of all the consequents of all the rules
            SetConfidencesOfConsequentsToZero();

            // process the rules
            foreach (var rule in Rules){
                rule.Calculate();
            }

            // now defuzzify the resultant conclusion using the specified method
            switch (method){
                case DefuzzifyMethod.Centroid:
                    return Variables[nameOfFlv].DeFuzzifyCentroid(CROSS_SECTION_SAMPLE_COUNT);

                case DefuzzifyMethod.MaxAv:
                    return Variables[nameOfFlv].DeFuzzifyMaxAv();
            }

            return 0;
        }

        /// <summary>
        ///     Add a fuzzy rule.
        /// </summary>
        /// <param name="antecedent">The antecedent of the rule.</param>
        /// <param name="consequent">The consequent of the rule.</param>
        public void AddRule(FuzzyTerm antecedent, FuzzyTerm consequent){
            Rules.Add(new FuzzyRule(antecedent, consequent));
        }

        /// <summary>
        ///     Creates a new fuzzy variable and returns it.
        /// </summary>
        /// <param name="fuzzyVariableName">The fuzzy variable name.</param>
        /// <returns>The new fuzzy variable.</returns>
        public FuzzyVariable CreateFlv(string fuzzyVariableName){
            Variables[fuzzyVariableName] = new FuzzyVariable();

            return Variables[fuzzyVariableName];
        }

        /// <summary>
        ///     Zeros the DOMs of the consequents of each rule.
        /// </summary>
        private void SetConfidencesOfConsequentsToZero(){
            foreach (var rule in Rules){
                rule.SetConfidenceOfConsequentToZero();
            }
        }
    }
}