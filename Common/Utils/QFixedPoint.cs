using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Cereal64.Common.Utils
{
    /// <summary>
    /// Represents a 2 byte Q fixed-point number with a sliding fixed-point for variable Qtypes and the capacity
    ///  to have unused bits. Stored in a base ushort as follows:
    ///  
    ///  Format: 4.8 (means 4 integer bits, 8 fractional bits, and 4 unused bits)
    ///  uuuu iiii  ffff ffff
    ///  u - unused bit
    ///  i - integer bit
    ///  f - fractional bit
    ///  
    /// </summary>
    [TypeConverter(typeof(QUShortTypeConverter))]
    public struct qushort
    {
        private ushort _value;
        private int _fixedPointIndex;
        private int _emptyBitCount;

        /// <summary>
        /// Number of bits allocated to the integer half
        /// </summary>
        public int IntegerBitCount
        {
            get { return _fixedPointIndex; }
            set { _fixedPointIndex = value; }
        }

        /// <summary>
        /// Number of bits allocated to the fractional half
        /// </summary>
        public int FractionalBitCount
        {
            get { return (16 - _emptyBitCount) - _fixedPointIndex; }
            set { _fixedPointIndex = (16 - _emptyBitCount) - value; }
        }

        /// <summary>
        /// This bitmask is used to zero out any unused bits.
        /// </summary>
        private ushort bitMask
        {
            get
            {
                ushort mask = 1;
                for (int i = _emptyBitCount; i < 15; i++)
                {
                    mask = (ushort)(mask << 1);
                    mask += 1;
                }

                return mask;
            }
        }

        /// <summary>
        /// Accesses the base ushort storing method. Good for loading from/saving to binary data
        /// </summary>
        public ushort RawValue
        {
            get { return _value; }
            set { _value = (ushort)(value & bitMask); }
        }

        /// <summary>
        /// Formatted value of the qushort
        /// </summary>
        public double Value
        {
            get
            {
                double val = (double)_value;
                val /= Math.Pow(2, FractionalBitCount);
                return val;
            }
            set
            {
                _value = (ushort)Math.Round(value * Math.Pow(2, FractionalBitCount));
                _value = (ushort)(_value & bitMask); //Apply the bitmask, to keep empty bits empty
            }
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public qushort(string fixedPointType) : this()
        {
            _value = 0;

            //Format of fixedPointType should be like "8.4". Any two numbers as long as they're
            // not negative and combined must be less than or equal to 16
            string[] splits = fixedPointType.Split('.');
            int top, bottom;
            if (int.TryParse(splits[0], out top) && int.TryParse(splits[1], out bottom))
            {
                _emptyBitCount = 16 - (top + bottom);
                _fixedPointIndex = top;
            }
        }

        public qushort(string fixedPointType, ushort val)
            : this(fixedPointType)
        {
            RawValue = val;
        }

        public qushort(string fixedPointType, double val)
            : this(fixedPointType)
        {
            Value = val;
        }

    }


    /// <summary>
    /// Represents a 2 byte Q fixed-point number with a sliding fixed-point for variable Qtypes and the capacity
    ///  to have unused bits. Stored in a base ushort as follows:
    ///  
    ///  Format: 4.8 (means 4 integer bits, 8 fractional bits, and 3 unused bits)
    ///  uuus iiii  ffff ffff
    ///  u - unused bit
    ///  s - sign bit
    ///  i - integer bit
    ///  f - fractional bit
    ///  
    /// </summary>
    [TypeConverter(typeof(QShortTypeConverter))]
    public struct qshort
    {
        private ushort _value;
        private int _fixedPointIndex;
        private int _emptyBitCount;
        private ushort _signBitMask;

        private bool isNegate { get { return (_value & _signBitMask) != 0; } }

        /// <summary>
        /// Number of bits allocated to the integer half
        /// </summary>
        public int IntegerBitCount
        {
            get { return _fixedPointIndex; }
            set { _fixedPointIndex = value; }
        }

        /// <summary>
        /// Number of bits allocated to the fractional half
        /// </summary>
        public int FractionalBitCount
        {
            get { return (15 - _emptyBitCount) - _fixedPointIndex; }
            set { _fixedPointIndex = (15 - _emptyBitCount) - value; }
        }

        /// <summary>
        /// This bitmask is used to zero out any unused bits.
        /// </summary>
        private ushort bitMask
        {
            get
            {
                ushort mask = 1;
                for (int i = _emptyBitCount; i < 15; i++)
                {
                    mask = (ushort)(mask << 1);
                    mask += 1;
                }

                return mask;
            }
        }

        /// <summary>
        /// Accesses the base ushort storing method. Good for loading from/saving to binary data
        /// </summary>
        public ushort RawValue
        {
            get { return _value; }
            set { _value = (ushort)(value & bitMask); }
        }

        /// <summary>
        /// Returns the value with the sign bit applied
        /// </summary>
        private double signedValue
        {
            get
            {
                if (isNegate)
                    return (double)(_value - _signBitMask * 2);
                else
                    return (double)_value;
            }
        }

        /// <summary>
        /// Formatted value of the qushort
        /// </summary>
        public double Value
        {
            get
            {
                double val = (double)signedValue;
                val /= Math.Pow(2, FractionalBitCount);
                return val;
            }
            set
            {
                double fullIntegerValue = Math.Round(value * Math.Pow(2, FractionalBitCount));
                _value = (ushort)fullIntegerValue;
                _value = (ushort)(_value & bitMask); //Apply the bitmask, to keep empty bits empty

                //For safe measure, double check that the negative value is geting applied, can be missing in truncation cases
                if (fullIntegerValue < 0 && (_value & _signBitMask) == 0)
                    _value = (ushort)(_value | _signBitMask);
            }
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public qshort(string fixedPointType)
            : this()
        {
            _value = 0;

            //Format of fixedPointType should be like "8.4". Any two numbers as long as they're
            // not negative and combined must be less than or equal to 16
            string[] splits = fixedPointType.Split('.');
            int top, bottom;
            if (int.TryParse(splits[0], out top) && int.TryParse(splits[1], out bottom))
            {
                _emptyBitCount = 15 - (top + bottom);
                _fixedPointIndex = top;

                _signBitMask = (ushort)(1 << (top + bottom));
            }
        }

        public qshort(string fixedPointType, ushort val)
            : this(fixedPointType)
        {
            RawValue = val;
        }

        public qshort(string fixedPointType, double val)
            : this(fixedPointType)
        {
            Value = val;
        }

    }

    /// <summary>
    /// Type converter to convert fixed point ushort to/from string. Used for PropertyGrid.
    /// </summary>
    public class QUShortTypeConverter : TypeConverter
    {
        private qushort QUShort;

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            else
            {
                return base.CanConvertFrom(context, sourceType);
            }
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value.GetType() == typeof(qushort))
            {
                QUShort = (qushort)value;
                return QUShort.Value.ToString();
            }
            else
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                string input = (string)value;

                QUShort.Value = double.Parse(input);

                return QUShort;
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }
    }

    /// <summary>
    /// Type converter to convert fixed point short to/from string. Used for PropertyGrid.
    /// </summary>
    public class QShortTypeConverter : TypeConverter
    {
        private qshort QShort;

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            else
            {
                return base.CanConvertFrom(context, sourceType);
            }
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value.GetType() == typeof(qshort))
            {
                QShort = (qshort)value;
                return QShort.Value.ToString();
            }
            else
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                string input = (string)value;

                QShort.Value = double.Parse(input);

                return QShort;
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }
    }

}
