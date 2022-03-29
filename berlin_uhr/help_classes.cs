// =====================================================================
// Author:        Michael Bröde
// Created:       02.04.2019
// =====================================================================
using System;
using rpi_rgb_led_matrix_sharp;

namespace help_classes
{
    // =================================================================
    // CColor
    // =================================================================
    static class CColor
    {
        // "Color" kommt nicht aus dem .Net-Namespace System.Drawing
        // sondern aus der rpi_rgb_led_matrix_sharp-Bibliothek
        static public readonly Color White = new Color(255, 255, 255);
        static public readonly Color Yellow = new Color(255, 255, 0);
        static public readonly Color Red = new Color(255, 0, 0);
        static public readonly Color Green = new Color(0, 255, 0);
        static public readonly Color Blue = new Color(0, 0, 255);
        static public readonly Color Black = new Color(0, 0, 0);
        static public readonly Color DarkSlateGrey = new Color(47, 79, 79);
        static public readonly Color DarkDarkGrey = new Color(25, 25, 25);
        static public readonly Color DarkDarkGrey2 = new Color(10, 10, 10);

        static public readonly Color CornflowerBlue = new Color(100, 149, 237);
        static public readonly Color LightSkyBlue = new Color(135, 206, 250);
        static public readonly Color MediumBlue = new Color(0, 0, 205);
        static public readonly Color DarkBlue = new Color(0, 0, 139);
    }

    // =================================================================
    // CPoint
    // =================================================================
    class CPoint
    {
        private int _x;
        private int _y;

        public int x
        {
            get { return _x; }
            set { _x = value; }
        }

        public int y
        {
            get { return _y; }
            set { _y = value; }
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", _x, _y);
        }

        // ----------------------
        // ctor
        // ----------------------
        public CPoint(int x, int y)
        {
            _x = x;
            _y = y;
        }
    }

    // =================================================================
    // CBaseClock
    // =================================================================
    abstract class CBaseClock
    {
        internal DateTime dtnow { get; set; }
        internal int msec { get; set; }
        internal int sec { get; set; }
        internal int min { get; set; }
        internal int hour { get; set; }
        internal int day { get; set; }
        internal int month { get; set; }
        internal int year { get; set; }

        /// <summary>
        /// Setzt die Datums- und Zeit-Properties dieser Klasse auf aktuelle Werte
        /// </summary>
        internal void SetTimeParts()
        {
            dtnow = DateTime.Now;
            msec = dtnow.Millisecond;
            sec = dtnow.Second;
            min = dtnow.Minute;
            hour = dtnow.Hour;
            day = dtnow.Day;
            month = dtnow.Month;
            year = dtnow.Year;
        }
    }
}
