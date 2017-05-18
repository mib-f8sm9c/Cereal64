using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cereal64.Common.Controls
{
    public partial class NumericSliderInput : UserControl
    {
        public NumericSliderInput()
        {
            InitializeComponent();
            ValueUpdated();
        }

        [Browsable(true)]
        public double Value { get { return _value; } set { if(_value != value) { _value = value; ValueUpdated(); } } }
        private double _value = 0.0;

        [Browsable(true)]
        public double Range { get { return _range; } set { if (_range != value) { _range = value; RecenterSlider(); } } }
        private double _range = 20.0;

        [Browsable(true)]
        public int Ticks { get { return _ticks; } set { if (_ticks != value) { _ticks = value; RecenterSlider(); } } }
        private int _ticks = 100;

        [Browsable(true)]
        public int Decimals { get { return _decimals; } set { if (_decimals != value) { _decimals = value; ValueUpdated(); } } }
        private int _decimals = 2;

        private double _sliderRefValue = 0.0;

        //[Browsable(true)]
        public event EventHandler ValueChanged;

        private void ValueUpdated(bool updateSlider = true)
        {
            //Update the text box
            string textFormat = GetTextFormatString();

            textBox.Text = string.Format(textFormat, _value);

            //Recenter the slider
            if(updateSlider)
                RecenterSlider();

            if(ValueChanged != null)
                ValueChanged(this, new EventArgs());
        }

        private string GetTextFormatString()
        {
            string textFormat = "{0:0";
            if (_decimals > 0)
            {
                textFormat += ".";
                for (int i = 0; i < _decimals; i++)
                    textFormat += "0";
            }
            textFormat += "}";

            return textFormat;
        }
        
        public void RecenterSlider()
        {
            trackBar.ValueChanged -= trackBar_ValueChanged;

            trackBar.Value = 0;
            trackBar.Minimum = -_ticks;
            trackBar.Maximum = _ticks;
            _sliderRefValue = _value;

            trackBar.ValueChanged += trackBar_ValueChanged;
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            textBox.TextChanged -= textBox_TextChanged;
            //Scrub the input for proper number formatting
            bool alreadyFoundDecimal = false;
            string text = textBox.Text;
            for (int i = 0; i < text.Length; i++)
            {
                if (!VALID_CHARS.Contains(text[i])) //Remove any non-number character from the string
                {
                    text.Remove(i, 1);
                    i--;
                }
                else
                {
                    switch (text[i])
                    {
                        case '.': //Only 1 decimal per number
                            if (alreadyFoundDecimal)
                            {
                                text.Remove(i, 1);
                                i--;
                            }
                            else
                                alreadyFoundDecimal = true;
                            break;
                        case '-': //Only allow - if it's the first character
                            if (i != 0)
                            {
                                text.Remove(i, 1);
                                i--;
                            }
                            break;
                    }
                }
            }

            if (text.Length != textBox.Text.Length)
                textBox.Text = text;
            
            textBox.TextChanged += textBox_TextChanged;
        }

        private const string VALID_CHARS = "0123456789-.";

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                double newVal;
                if (!double.TryParse(textBox.Text, out newVal))
                {
                    //Add back the old value to the box, for some reason it's currently invalid
                    ValueUpdated();
                }
                else
                {
                    Value = newVal;
                }
            }
        }

        private void trackBar_ValueChanged(object sender, EventArgs e)
        {
            //Update the value with the current value
            double newVal = _sliderRefValue + ((double)trackBar.Value / _ticks) * _range;
            _value = newVal;
            ValueUpdated(false);
        }

        private void trackBar_Scroll(object sender, EventArgs e)
        {

        }
    }
}
