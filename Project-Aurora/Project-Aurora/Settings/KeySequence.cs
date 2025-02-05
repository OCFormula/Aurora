﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora.Settings
{
    /// <summary>
    /// The type of the KeySequence
    /// </summary>
    public enum KeySequenceType
    {
        /// <summary>
        /// Sequence uses an array of DeviceKeys keys
        /// </summary>
        Sequence,
        /// <summary>
        /// Sequence uses a freeform region
        /// </summary>
        FreeForm
    }

    /// <summary>
    /// A class representing a series of DeviceKeys keys or a freeform region
    /// </summary>
    public class KeySequence : ICloneable
    {
        /// <summary>
        /// An array of DeviceKeys keys to be used with KeySequenceType.Sequence type.
        /// </summary>
        public List<Devices.DeviceKeys> keys;

        /// <summary>
        /// The type of this KeySequence instance.
        /// </summary>
        public KeySequenceType type;

        /// <summary>
        /// The Freeform object to be used with KeySequenceType.FreeForm type
        /// </summary>
        public FreeFormObject freeform;

        public KeySequence()
        {
            keys = new List<Devices.DeviceKeys>();
            type = KeySequenceType.Sequence;
            freeform = new FreeFormObject();
        }

        public KeySequence(KeySequence otherKeysequence)
        {
            keys = new List<Devices.DeviceKeys>(otherKeysequence.keys);
            type = otherKeysequence.type;
            freeform = otherKeysequence.freeform;
        }

        public KeySequence(FreeFormObject freeform)
        {
            this.keys = new List<Devices.DeviceKeys>();
            type = KeySequenceType.FreeForm;
            this.freeform = freeform;
        }

        public KeySequence(Devices.DeviceKeys[] keys)
        {
            this.keys = new List<Devices.DeviceKeys>(keys);
            type = KeySequenceType.Sequence;
            freeform = new FreeFormObject();
        }

        public RectangleF GetAffectedRegion()
        {
            switch (type)
            {
                case KeySequenceType.FreeForm:
                    return new RectangleF((freeform.X + Effects.grid_baseline_x) * Effects.EditorToCanvasWidth, (freeform.Y + Effects.grid_baseline_y) * Effects.EditorToCanvasHeight, freeform.Width * Effects.EditorToCanvasWidth, freeform.Height * Effects.EditorToCanvasHeight);
                default:

                    var left = 0.0f;
                    var top = left;
                    var right = top;
                    var bottom = right;

                    foreach(var key in keys)
                    {
                        BitmapRectangle keyMapping = Effects.GetBitmappingFromDeviceKey(key);

                        if(left == top && top == right && right == bottom && bottom == 0.0f)
                        {
                            left = keyMapping.Left;
                            top = keyMapping.Top;
                            right = keyMapping.Right;
                            bottom = keyMapping.Bottom;
                        }
                        else
                        {
                            if (keyMapping.Left < left)
                                left = keyMapping.Left;
                            if (keyMapping.Top < top)
                                top = keyMapping.Top;
                            if (keyMapping.Right > right)
                                right = keyMapping.Right;
                            if (keyMapping.Bottom > bottom)
                                bottom = keyMapping.Bottom;
                        }
                    }

                    return new RectangleF(left, top, right - left, bottom - top);
            }

        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((KeySequence)obj);
        }

        public bool Equals(KeySequence p)
        {
            if (ReferenceEquals(null, p)) return false;
            if (ReferenceEquals(this, p)) return true;

            return keys.Equals(p.keys) &&
                   type == p.type &&
                   freeform.Equals(p.freeform);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + keys.GetHashCode();
                hash = hash * 23 + type.GetHashCode();
                hash = hash * 23 + freeform.GetHashCode();
                return hash;
            }
        }

        public object Clone()
        {
            return new KeySequence(this);
        }
    }
}
