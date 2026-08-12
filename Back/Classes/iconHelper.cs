using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Back.Classes;
public static class IconHelper
{
    public static string ExtractAndSave(string filePath)
    {
        var icon = Icon.ExtractAssociatedIcon(filePath);
        if (icon == null)
            return string.Empty;

        string AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var AtlasIconsFolder = Path.Combine(AppDataFolder, "AtlasOrganizer", "Icons");   
        Directory.CreateDirectory(AtlasIconsFolder);

        var fileName = Path.GetFileNameWithoutExtension(filePath) + ".png";
        var savePath = Path.Combine(AtlasIconsFolder,fileName);

        using var bmp = icon.ToBitmap();
        bmp.Save(savePath, ImageFormat.Png);
        return savePath;
    }
}