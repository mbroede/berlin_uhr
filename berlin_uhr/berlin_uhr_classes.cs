// =====================================================================
// File:          berlin_uhr_classes.cs
// Author:        Michael Bröde
// Created:       02.04.2019
// Last modified: 10.03.2022
// =====================================================================
using System;
using System.Collections.Generic;
using rpi_rgb_led_matrix_sharp;
using help_classes;
using System.Threading;

namespace berlin_uhr_classes
{
    // =================================================================
    // CBaseBlock
    // =================================================================
    abstract class CBaseBlock
    {
        private int _top;
        private int _left;
        private int _width;
        private int _height;
        private Color _fillcolor_enabled;
        private readonly Color _fillcolor_disabled = CColor.DarkDarkGrey2;
        private readonly Color _bordercolor_enabled = CColor.White;
        private readonly Color _bordercolor_disabled = CColor.DarkSlateGrey;

        public CBaseBlock(int top, int left, int width, int height, Color fillcolor)
        {
            _top = top;
            _left = left;
            _width = width;
            _height = height;
            _fillcolor_enabled = fillcolor;
        }

        public void Enabled(RGBLedCanvas _canvas, bool enabled)
        {
            // Rahmen
            _canvas.DrawLine(
                _left, 
                _top, 
                _left + _width - 1, 
                _top,
                enabled ? _bordercolor_enabled : _bordercolor_disabled);
            _canvas.DrawLine(
                _left + _width - 1, 
                _top, 
                _left + _width - 1, 
                _top + _height - 1,
                enabled ? _bordercolor_enabled : _bordercolor_disabled);
            _canvas.DrawLine(
                _left + _width - 1, 
                _top + _height - 1, 
                _left, 
                _top + _height - 1,
                enabled ? _bordercolor_enabled : _bordercolor_disabled);
            _canvas.DrawLine(
                _left, 
                _top + _height - 1, 
                _left, 
                _top,
                enabled ? _bordercolor_enabled : _bordercolor_disabled);
            // Füllung
            for (int i = 1; i <= _height - 2; i++)
            {
                _canvas.DrawLine(
                    _left + 1, 
                    _top + i, 
                    _left + _width - 2, 
                    _top + i,
                    enabled ? _fillcolor_enabled : _fillcolor_disabled);
            }
        }
    }

    // =================================================================
    // CStandardBlock
    // =================================================================
    abstract class CStandardBlock : CBaseBlock
    {
        public CStandardBlock(int top, int left, Color fillcolor)
            : base(top, left, 12, 9, fillcolor)
        {
        }
    }

    // =================================================================
    // CHour5
    // =================================================================
    class CHour5 : CStandardBlock
    {
        public CHour5(int top, int left, Color fillcolor)
            : base(top, left, fillcolor)
        {
        }
    }

    // =================================================================
    // CHour1
    // =================================================================
    class CHour1 : CStandardBlock
    {
        public CHour1(int top, int left, Color fillcolor)
            : base(top, left, fillcolor)
        {
        }
    }

    // =================================================================
    // CMinute5
    // =================================================================
    class CMinute5 : CBaseBlock
    {
        public CMinute5(int top, int left, Color fillcolor)
            : base(top, left, 4, 9, fillcolor)
        {
        }
    }

    // =================================================================
    // CMinute1
    // =================================================================
    class CMinute1 : CStandardBlock
    {
        public CMinute1(int top, int left, Color fillcolor)
            : base(top, left, fillcolor)
        {
        }
    }

    // =================================================================
    // CBerlinUhr
    // =================================================================
    class CBerlinUhr : CBaseClock
    {
        private RGBLedMatrix _matrix;
        private RGBLedCanvas _canvas;
        private List<CPoint> _secpoints;

        private CHour5[] _hour5 = new CHour5[]
        {
            new CHour5(19, 5, CColor.Red),
            new CHour5(19, 19, CColor.Red),
            new CHour5(19, 33, CColor.Red),
            new CHour5(19, 47, CColor.Red)
        };

        private CHour1[] _hour1 = new CHour1[]
        {
            new CHour1(30, 5, CColor.Green),
            new CHour1(30, 19, CColor.Green),
            new CHour1(30, 33, CColor.Green),
            new CHour1(30, 47, CColor.Green)
        };

        private CMinute5[] _min5 = new CMinute5[]
        {
            new CMinute5(41, 5, CColor.Yellow),
            new CMinute5(41, 10, CColor.Blue),
            new CMinute5(41, 15, CColor.Yellow),
            new CMinute5(41, 20, CColor.Blue),
            new CMinute5(41, 25, CColor.Yellow),
            new CMinute5(41, 30, CColor.Blue),
            new CMinute5(41, 35, CColor.Yellow),
            new CMinute5(41, 40, CColor.Blue),
            new CMinute5(41, 45, CColor.Yellow),
            new CMinute5(41, 50, CColor.Blue),
            new CMinute5(41, 55, CColor.Yellow)
        };

        private CMinute1[] _min1 = new CMinute1[]
        {
            new CMinute1(52, 5, CColor.Blue),
            new CMinute1(52, 19, CColor.Blue),
            new CMinute1(52, 33, CColor.Blue),
            new CMinute1(52, 47, CColor.Blue)
        };

        // ----------------------
        // ctor
        // ----------------------
        public CBerlinUhr(RGBLedMatrix matrix, RGBLedCanvas canvas)
        {
            _matrix = matrix;
            _canvas = canvas;

            // Die Punkte des Sekundenwurms kommen aus einem Array,
            // das sich einmal ums Karree zieht und aus dem gezielt
            // vier Punkte enabled sind.
            // Das Array beginnt und endet oben am Rhombus
            // Start=32,2 --> RO=60,2
            _secpoints = new List<CPoint>();
            for (int x = 32; x <= 60; x++)
            {
                _secpoints.Add(new CPoint(x, 2));
            }
            // RO=60,2 --> RU=60,62
            for (int y = 2; y <= 62; y++)
            {
                _secpoints.Add(new CPoint(60, y));
            }
            // RU=60,62 --> LU=3,62  
            for (int x = 60; x >= 3; x--)
            {
                _secpoints.Add(new CPoint(x, 62));
            }
            // LU=3,62 --> LO=3,2
            for (int y = 62; y >= 2; y--)
            {
                _secpoints.Add(new CPoint(3, y));
            }
            // LO=3,2 --> Stopp=31,2
            for (int x = 3; x <= 31; x++)
            {
                _secpoints.Add(new CPoint(x, 2));
            }
        }

        // ----------------------
        // Run
        // ----------------------
        public int Run()
        {
            int rTop = 3;
            int rLeft = 31;
            int rLen = 7;
            int[] dx = new int[] { 0, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 0 };
            int dy1 = 0;
            int dy2 = 1;
            int rfillscale;
            int wormidx;

            // Eine Endlosschleife
            for ( ; ; )
            {
                // LED-Panel initialisieren
                _canvas.Fill(CColor.Black);

                // Zeit-Properties bestimmen (Sekunden, Minuten, Stunden usw.)
                base.SetTimeParts();

                // Sekundenrhombus zeichnen
                if (base.sec == 59)
                {
                    dy1 = 0;
                    dy2 = 1;
                }
                // tröpfelnde Sekunden
                dy1 = dy1 == 0 ? 1 : 0;
                dy2 = dy2 == 1 ? 0 : 1;
                for (int i = 0; i <= 12; i += 2)
                {
                    _canvas.SetPixel(31, 4 + i + dy1, CColor.Yellow);
                    _canvas.SetPixel(32, 4 + i + dy2, CColor.Yellow);
                }
                // Rhombus-Rahmen
                _canvas.DrawLine(
                    rLeft + 1,
                    rTop,
                    rLeft + 1 + rLen, 
                    rTop + rLen, 
                    CColor.White);
                _canvas.DrawLine(
                    rLeft + 1 + rLen, 
                    rTop + rLen, 
                    rLeft + 1, 
                    rTop + rLen + rLen, 
                    CColor.White);
                _canvas.DrawLine(
                    rLeft, 
                    rTop + rLen + rLen, 
                    rLeft - rLen, 
                    rTop + rLen, 
                    CColor.White);
                _canvas.DrawLine(
                    rLeft - rLen, 
                    rTop + rLen, 
                    rLeft, 
                    rTop, 
                    CColor.White);
                // Rhombus-Füllstand
                rfillscale = (int)Math.Round(
                    Convert.ToDecimal(13m / 59m * base.sec), 0);
                for (int i = 0; i < rfillscale; i++)
                {
                    _canvas.DrawLine(
                        rLeft - dx[i], 
                        rTop + 13 - i, 
                        rLeft + dx[i] + 1, 
                        rTop + 13 - i, 
                        CColor.Yellow);
                }

                // Der Sekundenwurm hat ein Führungspixel und drei Pixel die ihm folgen.
                // Der Array-Index des Führungspixels ergibt sich aus dem Verhältnis
                // der verstrichenen Millisekunden zur Gesamtzahl der Millisekunden
                // pro Umlauf (also pro Minute). Liegt das Führungspixel am Array-Anfang
                // (Index < 3) werden Folgepixel vom Array-Ende verwendet. 
                wormidx = Convert.ToInt32(Math.Round(
                    Convert.ToDouble(base.sec * 1000m + base.msec)
                    / Convert.ToDouble(59m * 1000m + 999)
                    * (_secpoints.Count - 1), 0));
                if (wormidx == 0)
                {
                    _canvas.SetPixel(
                        _secpoints[wormidx].x, 
                        _secpoints[wormidx].y, 
                        CColor.LightSkyBlue);
                    _canvas.SetPixel(
                        _secpoints[_secpoints.Count - 1].x, 
                        _secpoints[_secpoints.Count - 1].y,
                        CColor.CornflowerBlue);
                    _canvas.SetPixel(
                        _secpoints[_secpoints.Count - 2].x, 
                        _secpoints[_secpoints.Count - 2].y, 
                        CColor.Blue);
                    _canvas.SetPixel(
                        _secpoints[_secpoints.Count - 3].x, 
                        _secpoints[_secpoints.Count - 3].y,
                        CColor.DarkBlue);
                }
                if (wormidx == 1)
                {
                    _canvas.SetPixel(
                        _secpoints[wormidx].x, 
                        _secpoints[wormidx].y, 
                        CColor.LightSkyBlue);
                    _canvas.SetPixel(
                        _secpoints[wormidx - 1].x, 
                        _secpoints[wormidx - 1].y, 
                        CColor.CornflowerBlue);
                    _canvas.SetPixel(
                        _secpoints[_secpoints.Count - 1].x, 
                        _secpoints[_secpoints.Count - 1].y, 
                        CColor.Blue);
                    _canvas.SetPixel(
                        _secpoints[_secpoints.Count - 2].x, 
                        _secpoints[_secpoints.Count - 2].y, 
                        CColor.DarkBlue);
                }
                if (wormidx == 2)
                {
                    _canvas.SetPixel(
                        _secpoints[wormidx].x, 
                        _secpoints[wormidx].y, 
                        CColor.LightSkyBlue);
                    _canvas.SetPixel(
                        _secpoints[wormidx - 1].x, 
                        _secpoints[wormidx - 1].y, 
                        CColor.CornflowerBlue);
                    _canvas.SetPixel(
                        _secpoints[wormidx - 2].x, 
                        _secpoints[wormidx - 2].y, 
                        CColor.Blue);
                    _canvas.SetPixel(
                        _secpoints[_secpoints.Count - 1].x, 
                        _secpoints[_secpoints.Count - 1].y, 
                        CColor.DarkBlue);
                }
                if (wormidx >= 3)
                {
                    _canvas.SetPixel(
                        _secpoints[wormidx].x, 
                        _secpoints[wormidx].y, 
                        CColor.LightSkyBlue);
                    _canvas.SetPixel(
                        _secpoints[wormidx - 1].x, 
                        _secpoints[wormidx - 1].y, 
                        CColor.CornflowerBlue);
                    _canvas.SetPixel(
                        _secpoints[wormidx - 2].x, 
                        _secpoints[wormidx - 2].y, 
                        CColor.Blue);
                    _canvas.SetPixel(
                        _secpoints[wormidx - 3].x, 
                        _secpoints[wormidx - 3].y, 
                        CColor.DarkBlue);
                }

                // Stunden
                _hour5[0].Enabled(_canvas, base.hour >= 5);
                _hour5[1].Enabled(_canvas, base.hour >= 10);
                _hour5[2].Enabled(_canvas, base.hour >= 15);
                _hour5[3].Enabled(_canvas, base.hour >= 20);
                _hour1[0].Enabled(_canvas, base.hour % 5 >= 1);
                _hour1[1].Enabled(_canvas, base.hour % 5 >= 2);
                _hour1[2].Enabled(_canvas, base.hour % 5 >= 3);
                _hour1[3].Enabled(_canvas, base.hour % 5 >= 4);

                // Minuten
                _min5[0].Enabled(_canvas, base.min >= 5);
                _min5[1].Enabled(_canvas, base.min >= 10);
                _min5[2].Enabled(_canvas, base.min >= 15);
                _min5[3].Enabled(_canvas, base.min >= 20);
                _min5[4].Enabled(_canvas, base.min >= 25);
                _min5[5].Enabled(_canvas, base.min >= 30);
                _min5[6].Enabled(_canvas, base.min >= 35);
                _min5[7].Enabled(_canvas, base.min >= 40);
                _min5[8].Enabled(_canvas, base.min >= 45);
                _min5[9].Enabled(_canvas, base.min >= 50);
                _min5[10].Enabled(_canvas, base.min >= 55);
                _min1[0].Enabled(_canvas, base.min % 5 >= 1);
                _min1[1].Enabled(_canvas, base.min % 5 >= 2);
                _min1[2].Enabled(_canvas, base.min % 5 >= 3);
                _min1[3].Enabled(_canvas, base.min % 5 >= 4);

                // Gesamtgebilde auf dem LED-Panel anzeigen
                _canvas = _matrix.SwapOnVsync(_canvas);

                // Zur Entspannung vor dem nächsten Schleifendurchlauf etwas warten.
                // (abhängig von der Performance des verwendeten Raspberry-Modells)
                Thread.Sleep(250);
            }
            return 0;
        }
    }
}
