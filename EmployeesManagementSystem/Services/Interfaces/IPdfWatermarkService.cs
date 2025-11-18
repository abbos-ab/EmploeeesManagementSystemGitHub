namespace EmployeesManagementSystem.Services.Interfaces;

public interface IPdfWatermarkService
{
    byte[] AddStampToLastPage(byte[] pdfBytes, string watermarkText);
}