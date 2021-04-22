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

namespace GameBrains.AI {
    /// <summary>
    ///     Class for right shoulder type fuzzy set.
    /// </summary>
    public class FuzzySetRightShoulder : FuzzySet {
        /// <summary>
        ///     Initializes a new instance of the FuzzySetRightShoulder class.
        /// </summary>
        /// <param name="peak">The peak point.</param>
        /// <param name="left">The left offset.</param>
        /// <param name="right">The right offset.</param>
        public FuzzySetRightShoulder(float peak, float left, float right)
            : base((peak + right + peak) / 2){
            PeakPoint = peak;
            LeftOffset = left;
            RightOffset = right;
        }

        /// <summary>
        ///     Gets the peak point.
        /// </summary>
        public float PeakPoint { get; }

        /// <summary>
        ///     Gets the left offset.
        /// </summary>
        public float LeftOffset { get; }

        /// <summary>
        ///     Gets the right offset.
        /// </summary>
        public float RightOffset { get; }

        /// <summary>
        ///     Returns the degree of membership in this set of the given value. This does not set
        ///     <see cref="FuzzySet._dom" /> to the degree of membership of the value passed as the
        ///     parameter. This is because the centroid defuzzification method also uses this method to
        ///     determine the DOMs of the values it uses as its sample points.
        /// </summary>
        /// <param name="givenValue">The given value.</param>
        /// <returns>
        ///     The degree of membership in this set of the given value.
        /// </returns>
        public override float CalculateDom(float givenValue){
            // test for the case where the left or right offsets are zero
            // (to prevent divide by zero errors below)
            if (Epsilon.IsEqual(RightOffset, 0.0f) && Epsilon.IsEqual(PeakPoint, givenValue) ||
                Epsilon.IsEqual(LeftOffset, 0.0f) && Epsilon.IsEqual(PeakPoint, givenValue)){
                return 1.0f;
            }

            // find DOM if left of center
            if (givenValue <= PeakPoint && givenValue > PeakPoint - LeftOffset){
                var grad = 1.0f / LeftOffset;
                return grad * (givenValue - (PeakPoint - LeftOffset));
            }

            // find DOM if right of center and less than center + right offset
            if (givenValue > PeakPoint && givenValue <= PeakPoint + RightOffset){
                return 1.0f;
            }

            // out of range of this FLV, return zero
            return 0f;
        }
    }
}