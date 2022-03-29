// =====================================================================
// File:          program.cs
// Author:        Michael Bröde
// Created:       02.04.2019
// =====================================================================
using berlin_uhr_classes;
using rpi_rgb_led_matrix_sharp;

namespace berlin_uhr
{
    class Programm
    {
        static void Main(string[] args)
        {
            var matrix = new RGBLedMatrix(new RGBLedMatrixOptions
            {
                Rows = 64,
                Cols = 64,
                GpioSlowdown = 2,
                Brightness = 25,
                DisableHardwarePulsing = false
            });
            var canvas = matrix.CreateOffscreenCanvas();
            CBerlinUhr berlinuhr = new CBerlinUhr(matrix, canvas);
            berlinuhr.Run();
        }
    }
}
