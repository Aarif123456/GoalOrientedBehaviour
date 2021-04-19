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

namespace GameBrains.AI
{
    using System.Collections.Generic;
    
    using UnityEngine;

    /// <summary>
    /// Class to implement a fuzzy variable.
    /// </summary>
    public class FuzzyVariable
    {
        /// <summary>
        /// Initializes a new instance of the FuzzyVariable class.
        /// </summary>
        public FuzzyVariable()
        {
            MemberSets = new Dictionary<string, FuzzySet>();
            MinRange = 0.0f;
            MaxRange = 0.0f;
        }

        /// <summary>
        /// Gets the member sets of the variable.
        /// </summary>
        public Dictionary<string, FuzzySet> MemberSets { get; private set; }

        /// <summary>
        /// Gets or sets the minimum value of the range of this variable.
        /// </summary>
        public float MinRange { get; set; }

        /// <summary>
        /// Gets or sets the maximum value of the range of this variable.
        /// </summary>
        public float MaxRange { get; set; }

        /// <summary>
        /// Takes a crisp value and calculates its degree of membership for each set in the
        /// variable.
        /// </summary>
        /// <param name="crispValue">The crisp value.</param>
        public void Fuzzify(float crispValue)
        {
            // make sure the value is within the bounds of this variable
            if (crispValue < MinRange || crispValue > MaxRange)
            {
                System.Diagnostics.Debug.WriteLine("FuzzyVariable.Fuzzify>: value out of range.");
                return;
            }

            // for each set in the flv calculate the DOM for the given value
            foreach (KeyValuePair<string, FuzzySet> kvp in MemberSets)
            {
                kvp.Value.Dom = kvp.Value.CalculateDom(crispValue);
            }
        }

        /// <summary>
        /// Defuzzifies the value by averaging the maxima of the sets that have fired.
        /// </summary>
        /// <returns>Sum (maxima * DOM) / sum (DOMs).</returns>
        public float DeFuzzifyMaxAv()
        {
            float bottom = 0.0f;
            float top = 0.0f;

            foreach (KeyValuePair<string, FuzzySet> kvp in MemberSets)
            {
                bottom += kvp.Value.Dom;

                top += kvp.Value.RepresentativeValue * kvp.Value.Dom;
            }

            // make sure bottom is not equal to zero
            if (Epsilon.IsEqual(0, bottom))
            {
                return 0.0f;
            }

            return top / bottom;
        }

        /// <summary>
        /// Defuzzify the variable using the centroid method.
        /// </summary>
        /// <param name="sampleCount">The number of samples to use.</param>
        /// <returns>A crisp value.</returns>
        public float DeFuzzifyCentroid(int sampleCount)
        {
            // calculate the step size
            float stepSize = (MaxRange - MinRange) / sampleCount;

            float totalArea = 0.0f;
            float sumOfMoments = 0.0f;

            // step through the range of this variable in increments equal to
            // stepSize adding up the contribution (lower of CalculateDOM or
            // the actual DOM of this variable's fuzzified value) for each
            // subset. This gives an approximation of the total area of the
            // fuzzy manifold. (This is similar to how the area under a curve
            // is calculated using calculus... the heights of lots of 'slices'
            // are summed to give the total area.)
            //
            // In addition the moment of each slice is calculated and summed.
            // Dividing the total area by the sum of the moments gives the
            // centroid. (Just like calculating the center of mass of an object)
            for (int sample = 1; sample <= sampleCount; ++sample)
            {
                // for each set get the contribution to the area. This is the
                // lower of the value returned from CalculateDOM or the actual
                // DOM of the fuzzified value itself   
                foreach (KeyValuePair<string, FuzzySet> kvp in MemberSets)
                {
                    float contribution =
                        Mathf.Min(
                            kvp.Value.CalculateDom(MinRange + sample * stepSize),
                            kvp.Value.Dom);

                    totalArea += contribution;

                    sumOfMoments += (MinRange + sample * stepSize) * contribution;
                }
            }

            // make sure total area is not equal to zero
            if (Epsilon.IsEqual(0, totalArea))
            {
                return 0.0f;
            }

            return sumOfMoments / totalArea;
        }

        /// <summary>
        /// Adds a triangular shaped fuzzy set to the variable.
        /// </summary>
        /// <param name="name">The fuzzy set name.</param>
        /// <param name="minimumBound">The minimum bound.</param>
        /// <param name="peak">The peak point.</param>
        /// <param name="maximumBound">The maximum bound.</param>
        /// <returns>The triangular shaped fuzzy set.</returns>
        public FzSet AddTriangularSet(
            string name,
            float minimumBound,
            float peak,
            float maximumBound)
        {
            MemberSets[name] = 
                new FuzzySetTriangle(peak, peak - minimumBound, maximumBound - peak);

            // adjust range if necessary
            AdjustRangeToFit(minimumBound, maximumBound);

            return new FzSet(MemberSets[name]);
        }

        /// <summary>
        /// Adds a left shoulder type fuzzy set.
        /// </summary>
        /// <param name="name">The fuzzy set name.</param>
        /// <param name="minimumBound">The minimum bound.</param>
        /// <param name="peak">The peak point.</param>
        /// <param name="maximumBound">The maximum bound.</param>
        /// <returns>A left shoulder type fuzzy set.</returns>
        public FzSet AddLeftShoulderSet(
            string name,
            float minimumBound,
            float peak,
            float maximumBound)
        {
            MemberSets[name] = 
                new FuzzySetLeftShoulder(peak, peak - minimumBound, maximumBound - peak);

            // adjust range if necessary
            AdjustRangeToFit(minimumBound, maximumBound);

            return new FzSet(MemberSets[name]);
        }

        /// <summary>
        /// Adds a right shoulder type fuzzy set.
        /// </summary>
        /// <param name="name">The fuzzy set name.</param>
        /// <param name="minimumBound">The minimum bound.</param>
        /// <param name="peak">The peak point.</param>
        /// <param name="maximumBound">The maximum bound.</param>
        /// <returns>A right shoulder type fuzzy set.</returns>
        public FzSet AddRightShoulderSet(
            string name,
            float minimumBound,
            float peak,
            float maximumBound)
        {
            MemberSets[name] =
                new FuzzySetRightShoulder(
                    peak,
                    peak - minimumBound,
                    maximumBound - peak);

            // adjust range if necessary
            AdjustRangeToFit(minimumBound, maximumBound);

            return new FzSet(MemberSets[name]);
        }

        /// <summary>
        /// Adds a singleton fuzzy set to the variable.
        /// </summary>
        /// <param name="name">The fuzzy set name.</param>
        /// <param name="minimumBound">The minimum bound.</param>
        /// <param name="peak">The peak point.</param>
        /// <param name="maximumBound">The maximum bound.</param>
        /// <returns>A singleton fuzzy set.</returns>
        public FzSet AddSingletonSet(
            string name,
            float minimumBound,
            float peak,
            float maximumBound)
        {
            MemberSets[name] =
                new FuzzySetSingleton(
                    peak,
                    peak - minimumBound,
                    maximumBound - peak);

            AdjustRangeToFit(minimumBound, maximumBound);

            return new FzSet(MemberSets[name]);
        }

        /// <summary>
        /// This method is called with the upper and lower bound of a set each time a new set is
        /// added to adjust the upper and lower range values accordingly.
        /// </summary>
        /// <param name="minimumBound">The minimum bound.</param>
        /// <param name="maximumBound">The maximum bound.</param>
        private void AdjustRangeToFit(float minimumBound, float maximumBound)
        {
            if (minimumBound < MinRange)
            {
                MinRange = minimumBound;
            }

            if (maximumBound > MaxRange)
            {
                MaxRange = maximumBound;
            }
        }
    }
}
