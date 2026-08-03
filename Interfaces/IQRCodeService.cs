using LegendaryCruises.Models;

namespace LegendaryCruises.Interfaces
{
    public interface IQRCodeService
    {
        string GenerateQRCode(int userProfileId, string data);
        (string Base64, byte[] Bytes) GenerateQRCodeWithBytes(string data);
        Task<QRCodeValidationResult> ValidateQRCodeAsync(string qrCode);
    }
}
