using UnityEngine;
using SkiaSharp;
using System.IO;

public class SkiaSharpTest : MonoBehaviour
{
    void Start()
    {
        // Create a new SkiaSharp surface with a width, height, and color type
        var info = new SKImageInfo(256, 256);
        using (var surface = SKSurface.Create(info))
        {
            var canvas = surface.Canvas;

            // Clear the canvas with a color
            canvas.Clear(SKColors.White);

            // Draw a red circle
            var paint = new SKPaint
            {
                Color = SKColors.Red,
                IsAntialias = true
            };
            canvas.DrawCircle(128, 128, 64, paint);

            // Define the path for the SkiaSharpTest folder
            var folderPath = Path.Combine(Application.dataPath, "SkiaSharpTest");

            // Create the folder if it doesn't exist
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Save the surface to a PNG file in the SkiaSharpTest folder
            using (var image = surface.Snapshot())
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            {
                var path = Path.Combine(folderPath, "SkiaSharpTest.png");
                File.WriteAllBytes(path, data.ToArray());
                Debug.Log("SkiaSharpTest.png saved to " + path);
            }
        }
    }
}
