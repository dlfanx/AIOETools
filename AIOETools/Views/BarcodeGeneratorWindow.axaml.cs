using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;
using ZXing.SkiaSharp;

namespace AIOETools.Views
{
    public partial class BarcodeGeneratorWindow : Window
    {
        private TextBox inputTextBox;
        private Avalonia.Controls.Image barcodeImage;

        public BarcodeGeneratorWindow()
        {
            InitializeComponent();

            inputTextBox = this.FindControl<TextBox>("InputTextBox");
            barcodeImage = this.FindControl<Avalonia.Controls.Image>("BarcodeImage");

            var generateButton = this.FindControl<Button>("GenerateButton");
            if (generateButton != null)
                generateButton.Click += OnGenerateBarcodeClicked;
        }

        private void OnGenerateBarcodeClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            string inputText = inputTextBox.Text;
            if (!string.IsNullOrEmpty(inputText))
            {
                var barcodeBitmap = GenerateBarcode(inputText);
                if (barcodeBitmap != null)
                {
                    barcodeImage.Source = barcodeBitmap;
                }
            }
        }

        private Bitmap GenerateBarcode(string data)
        {
            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Width = 200,
                    Height = 100,
                    Margin = 10
                }
            };

            var pixelData = writer.Write(data);
            return ConvertPixelDataToAvaloniaBitmap(pixelData);
        }

        private Bitmap ConvertPixelDataToAvaloniaBitmap(PixelData pixelData)
        {
            using var stream = new MemoryStream();
            using var bitmap = new System.Drawing.Bitmap(
                pixelData.Width, pixelData.Height,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
            );

            // Copy pixel data into Bitmap
            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
            );

            System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
            bitmap.UnlockBits(bitmapData);

            // Save as PNG to MemoryStream
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            stream.Position = 0;

            return new Bitmap(stream);
        }
    }
}
