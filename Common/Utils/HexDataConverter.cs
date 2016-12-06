using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Cereal64.Common.Utils
{
    //Code copied from http://snipplr.com/view/17695/

    /// <summary>
    /// Type converter to convert hexadecimal unsigned integer to/from string. Used for PropertyGrid.
    /// </summary>
    public class UInt32HexTypeConverter : TypeConverter
    {
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
            if (destinationType == typeof(string) && value.GetType() == typeof(UInt32))
            {
                return string.Format("0x{0:X8}", value);
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

                if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    input = input.Substring(2);
                }

                return UInt32.Parse(input, System.Globalization.NumberStyles.HexNumber, culture);
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }
    }

    /// <summary>
    /// Type converter to convert hexadecimal integer to/from string. Used for PropertyGrid.
    /// </summary>
    public class Int32HexTypeConverter : TypeConverter
    {
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
            if (destinationType == typeof(string))
            {
                if(value == null)
                {
                    return "null";
                }
                else if (value.GetType() == typeof(Int32))
                {
                    return string.Format("0x{0:X8}", value);
                }
                else
                {
                    return base.ConvertTo(context, culture, value, destinationType);
                }
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

                if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    input = input.Substring(2);
                }

                return Int32.Parse(input, System.Globalization.NumberStyles.HexNumber, culture);
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }
    }

    /// <summary>
    /// Type converter to convert hexadecimal unsigned short to/from string. Used for PropertyGrid.
    /// </summary>
    public class UInt16HexTypeConverter : TypeConverter
    {
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
            if (destinationType == typeof(string) && value.GetType() == typeof(UInt16))
            {
                return string.Format("0x{0:X4}", value);
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

                if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    input = input.Substring(2);
                }

                return UInt16.Parse(input, System.Globalization.NumberStyles.HexNumber, culture);
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }
    }

    /// <summary>
    /// Type converter to convert hexadecimal short to/from string. Used for PropertyGrid.
    /// </summary>
    public class Int16HexTypeConverter : TypeConverter
    {
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
            if (destinationType == typeof(string) && value.GetType() == typeof(Int16))
            {
                return string.Format("0x{0:X4}", value);
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

                if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    input = input.Substring(2);
                }

                return Int16.Parse(input, System.Globalization.NumberStyles.HexNumber, culture);
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }
    }

    /// <summary>
    /// Type converter to convert hexadecimal byte to/from string. Used for PropertyGrid.
    /// </summary>
    public class ByteHexTypeConverter : TypeConverter
    {
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
            if (destinationType == typeof(string) && value.GetType() == typeof(Byte))
            {
                return string.Format("0x{0:X2}", value);
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

                if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    input = input.Substring(2);
                }

                return Byte.Parse(input, System.Globalization.NumberStyles.HexNumber, culture);
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }
    }

    /// <summary>
    /// Type converter to convert hexadecimal signed byte to/from string. Used for PropertyGrid.
    /// </summary>
    public class SByteHexTypeConverter : TypeConverter
    {
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
            if (destinationType == typeof(string) && value.GetType() == typeof(SByte))
            {
                return string.Format("0x{0:X2}", value);
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

                if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    input = input.Substring(2);
                }

                return SByte.Parse(input, System.Globalization.NumberStyles.HexNumber, culture);
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }
    }

    /// <summary>
    /// Type converter to convert array of hexadecimal bytes to/from string. Used for PropertyGrid.
    /// </summary>
    public class ByteArrayHexTypeConverter : ArrayConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            //Note: this works, but overwrites it on ALL bytes. Not sure I want that yet.
            //TypeDescriptor.AddAttributes(typeof(Byte), new TypeConverterAttribute(typeof(ByteHexTypeConverter)));

            //Create a new PropertyDescriptorCollection, but using the ByteHexPropertyDescriptorWrapper
            // to force the array values to use ByteHexTypeConverter
            PropertyDescriptorCollection coll = base.GetProperties(context, value, attributes);
            ByteHexPropertyDescriptorWrapper[] props = new ByteHexPropertyDescriptorWrapper[coll.Count];
            for(int i = 0; i < coll.Count; i++)
                props[i] = new ByteHexPropertyDescriptorWrapper(coll[i]);
            PropertyDescriptorCollection coll2 = new PropertyDescriptorCollection(props);
            
            return coll2;
        }

        //public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        //{
        //    return base.CanConvertFrom(context, sourceType);

        //    if (sourceType == typeof(string))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return base.CanConvertFrom(context, sourceType);
        //    }
        //}

        //public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        //{
        //    return base.CanConvertTo(context, destinationType);

        //    if (destinationType == typeof(string))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return base.CanConvertTo(context, destinationType);
        //    }
        //}

        //public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        //{
        //    return base.ConvertTo(context, culture, value, destinationType);

        //    Type sourceType = value.GetType();
        //    if (destinationType == typeof(string) && 
        //        sourceType.HasElementType && sourceType.GetElementType() == typeof(Byte))
        //    {
        //        byte[] input = (byte[])value;
        //        string[] output = new string[input.Length];
        //        for(int i = 0; i < input.Length; i++)
        //            output[i] = string.Format("0x{0:X2}", input[i]);
        //        return output;
        //    }
        //    else
        //    {
        //        return base.ConvertTo(context, culture, value, destinationType);
        //    }
        //}

        //public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        //{
        //    return base.ConvertFrom(context, culture, value);

        //    if (value.GetType() == typeof(Array)
        //        && value.GetType().HasElementType && value.GetType().GetElementType() == typeof(string))
        //    {
        //        string[] inputs = (string[])value;

        //        byte[] outputs = new byte[inputs.Length];
        //        for (int i = 0; i < inputs.Length; i++ )
        //        {
        //            string input = inputs[i];
        //            if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        //            {
        //                input = input.Substring(2);
        //            }
        //            outputs[i] = Byte.Parse(input, System.Globalization.NumberStyles.HexNumber, culture);
        //        }

        //        return outputs;
        //    }
        //    else
        //    {
        //        return base.ConvertFrom(context, culture, value);
        //    }
        //}
    }

    /// <summary>
    /// Use this to use the ByteHexTypeConverter with all members in a ByteArrayHexTypeConverter
    ///  HUGE pain in the butt, by the way
    /// </summary>
    public class ByteHexPropertyDescriptorWrapper : PropertyDescriptor
    {
        PropertyDescriptor _baseDesc;

        public ByteHexPropertyDescriptorWrapper(PropertyDescriptor desc)
            : base (desc)
        {
            _baseDesc = desc;
        }

        //The whole point of this class, force it to use the
        // ByteHexTypeConverter
        public override TypeConverter Converter
        {
            get
            {
                return new ByteHexTypeConverter();
            }
        }

        public override bool CanResetValue(object component)
        {
            return _baseDesc.CanResetValue(component);
        }

        public override void ResetValue(object component)
        {
            _baseDesc.ResetValue(component);
        }

        public override object GetValue(object component)
        {
            return _baseDesc.GetValue(component);
        }

        public override void SetValue(object component, object value)
        {
            _baseDesc.SetValue(component, value);
        }

        public override Type ComponentType
        {
            get { return _baseDesc.ComponentType; }
        }

        public override bool IsReadOnly
        {
            get { return _baseDesc.IsReadOnly; }
        }

        public override Type PropertyType
        {
            get { return _baseDesc.PropertyType; }
        }

        public override bool ShouldSerializeValue(object component)
        {
            return _baseDesc.ShouldSerializeValue(component);
        }
    }

}
