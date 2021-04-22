#region Copyright © ThotLab Games 2011. Licensed under the terms of the Microsoft Reciprocal Licence (Ms-RL).

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

#endregion Copyright © ThotLab Games 2011. Licensed under the terms of the Microsoft Reciprocal Licence (Ms-RL).

using UnityEngine;

namespace Entities.Memory.SensoryMemory {
    /// <summary>
    ///     Class to implement short term memory of sensory info.
    /// </summary>
    public class SensoryMemoryRecord {
        /// <summary>
        ///     Initializes a new instance of the SensoryMemoryRecord class.
        /// </summary>
        public SensoryMemoryRecord(){
            TimeLastSensed = -999;
            TimeBecameVisible = -999;
            TimeLastVisible = 0;
            IsWithinFieldOfView = false;
            IsShootable = false;
        }

        /// <summary>
        ///     Gets or sets the time the opponent was last sensed (seen or heard). This is used to
        ///     determine if a bot can 'remember' this record or not.  (if Time.TimeNow -
        ///     _timeLastSensed is greater than the bot's memory span, the data in this record is made
        ///     unavailable to clients).
        /// </summary>
        public float TimeLastSensed { get; set; }

        /// <summary>
        ///     Gets or sets the current time when an opponent first becomes visible. It can be useful
        ///     to know how long an opponent has been visible. It's then a simple matter to calculate
        ///     how long the opponent has been in view: Time.TimeNow - TimeBecameVisible.
        /// </summary>
        public float TimeBecameVisible { get; set; }

        /// <summary>
        ///     Gets or sets the last time an opponent was seen.
        /// </summary>
        public float TimeLastVisible { get; set; }

        /// <summary>
        ///     Gets or sets a vector marking the position where the opponent was last sensed. This can
        ///     be used to help hunt down an opponent if it goes out of view.
        /// </summary>
        public Vector2 LastSensedPosition { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether an opponent is within the field of view of the owner.
        /// </summary>
        public bool IsWithinFieldOfView { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether there is no obstruction between the opponent and the
        ///     owner, permitting a shot.
        /// </summary>
        public bool IsShootable { get; set; }
    }
}