using EmployeesManagementSystem.Services.Interfaces;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using PdfSharpCore.Drawing;

namespace EmployeesManagementSystem.Services;

public class PdfWatermarkService : IPdfWatermarkService
{
    public byte[] AddStampToLastPage(byte[] pdfBytes, string watermarkText)
    {
        using var input = new MemoryStream(pdfBytes);
        using var output = new MemoryStream();

        PdfDocument document;
        try
        {
            document = PdfReader.Open(input, PdfDocumentOpenMode.Modify);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("The provided data is not a valid PDF file.", ex);
        }

        if (document.Pages.Count == 0)
            throw new InvalidDataException("The PDF document contains no pages.");

        var page = document.Pages[document.Pages.Count - 1];
        using var gfx = XGraphics.FromPdfPage(page);

        var centerX = page.Width - 150;
        var centerY = page.Height - 150;
        double radius = 80;

        var pen = new XPen(XColor.FromArgb(180, 200, 0, 0), 4);
        gfx.DrawEllipse(pen, centerX - radius, centerY - radius, radius * 2, radius * 2);

        gfx.DrawEllipse(pen, centerX - radius / 1.5, centerY - radius / 1.5, radius * 1.3, radius * 1.3);

        var font = new XFont("DejaVu Sans", 20, XFontStyle.Bold);
        var brush = new XSolidBrush(XColor.FromArgb(150, 220, 0, 0));
        gfx.DrawString(watermarkText, font, brush, new XPoint(centerX, centerY), XStringFormats.Center);

        DrawCircularText(gfx, "CONFIRMED BY THE RECEIVER.", centerX, centerY, radius - 10, 14);

        gfx.RotateAtTransform(-10, new XPoint(centerX, centerY));

        document.Save(output, false);
        return output.ToArray();
    }

    private void DrawCircularText(XGraphics gfx, string text, double centerX, double centerY, double radius,
        double fontSize)
    {
        var font = new XFont("DejaVu Sans", fontSize, XFontStyle.Bold);
        var chars = text.ToCharArray();
        var angleStep = 360.0 / chars.Length;
        double startAngle = -90;

        for (var i = 0; i < chars.Length; i++)
        {
            var angle = startAngle + i * angleStep;
            var rad = angle * Math.PI / 180.0;
            var x = centerX + radius * Math.Cos(rad);
            var y = centerY + radius * Math.Sin(rad);
            gfx.Save();
            gfx.TranslateTransform(x, y);
            gfx.RotateTransform(angle + 90);
            gfx.DrawString(chars[i].ToString(), font, XBrushes.DarkRed, new XPoint(0, 0), XStringFormats.Center);
            gfx.Restore();
        }
    }
}