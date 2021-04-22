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

using UnityEngine;

namespace GameBrains.AI {
    /// <summary>
    ///     Class for handling small things.
    /// </summary>
    public class Epsilon {
        /// <summary>
        ///     The default Epsilon value.
        /// </summary>
        private const float DEFAULT_EPSILON = 0.000001f;

        /// <summary>
        ///     The Epsilon value used in comparisons.
        /// </summary>
        private static EpsilonTester _epsilonTester =
            new EpsilonTester(DEFAULT_EPSILON);

        /// <summary>
        ///     Gets or sets the Epsilon value used in comparisons.
        /// </summary>
        public float Value {
            get => _epsilonTester.Value;
            set => _epsilonTester.Value = value;
        }

        /// <summary>
        ///     Tests if <paramref name="a" /> and <paramref name="b" />are nearly equal.
        /// </summary>
        /// <param name="a">Float value a.</param>
        /// <param name="b">Float value b.</param>
        /// <returns>
        ///     True if <paramref name="a" /> and <paramref name="b" />are nearly equal.
        /// </returns>
        public static bool IsEqual(float a, float b){
            return IsZero(a - b);
        }

        /// <summary>
        ///     Tests if vectors <paramref name="v1" /> and <paramref name="v2" />are nearly equal.
        /// </summary>
        /// <param name="v1">Vector v1.</param>
        /// <param name="v2">Vector v2.</param>
        /// <returns>
        ///     True if vectors <paramref name="v1" /> and <paramref name="v2" />are nearly equal.
        /// </returns>
        public static bool IsEqual(Vector2 v1, Vector2 v2){
            return IsZero(v1 - v2);
        }

        /// <summary>
        ///     Tests if <paramref name="a" /> and <paramref name="b" />are not nearly equal.
        /// </summary>
        /// <param name="a">Float value a.</param>
        /// <param name="b">Float value b.</param>
        /// <returns>
        ///     True if <paramref name="a" /> and <paramref name="b" />are not nearly equal.
        /// </returns>
        public static bool IsNotEqual(float a, float b){
            return IsNotZero(a - b);
        }

        /// <summary>
        ///     Tests if <paramref name="f" /> is not nearly zero.
        /// </summary>
        /// <param name="f">Float to test.</param>
        /// <returns>True if <paramref name="f" /> is not nearly zero.</returns>
        public static bool IsNotZero(float f){
            return _epsilonTester.FloatIsNotZero(f);
        }

        /// <summary>
        ///     Tests if <paramref name="f" /> is nearly zero.
        /// </summary>
        /// <param name="f">Float to test.</param>
        /// <returns>True if <paramref name="f" /> is nearly zero.</returns>
        public static bool IsZero(float f){
            return _epsilonTester.FloatIsZero(f);
        }

        /// <summary>
        ///     Tests if <paramref name="v" /> is nearly zero.
        /// </summary>
        /// <param name="v">Vector to test.</param>
        /// <returns>True if <paramref name="v" /> is nearly zero.</returns>
        public static bool IsZero(Vector2 v){
            return _epsilonTester.VectorIsZero(v);
        }

        /// <summary>
        ///     Tests if <paramref name="v" /> is nearly zero.
        /// </summary>
        /// <param name="v">Vector to test.</param>
        /// <returns>True <paramref name="v" /> is nearly zero.</returns>
        public static bool IsZero(Vector3 v){
            return _epsilonTester.VectorIsZero(v);
        }

        /// <summary>
        ///     Tests if <paramref name="v" /> is nearly zero.
        /// </summary>
        /// <param name="v">Vector to test.</param>
        /// <returns>True if <paramref name="v" /> is nearly zero.</returns>
        public static bool IsZero(Vector4 v){
            return _epsilonTester.VectorIsZero(v);
        }
    }
}