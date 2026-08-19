using LegendaryCruises.Data;
using LegendaryCruises.Interfaces;
using LegendaryCruises.Models;
using Microsoft.EntityFrameworkCore;
using QRCoder;

namespace LegendaryCruises.Services
{
    public class QRCodeService : IQRCodeService
    {
        private readonly IDbContextFactory<DataContext> _factory;

        public QRCodeService(IDbContextFactory<DataContext> factory)
        {
            _factory = factory;
        }

        // ============================================================
        // GENERATE QR CODE AND SAVE TO DATABASE
        // ============================================================
        public string GenerateQRCode(int userProfileId, string data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            using var context = _factory.CreateDbContext();

            // Generate unique GUID for QR code
            string uniqueCode = Guid.NewGuid().ToString();

            var (base64, _) = GenerateQRCodeWithBytes(uniqueCode);

            var qrCode = new QRCodeModel
            {
                UserProfileId = userProfileId,
                QRCodeBase64 = base64,
                Text = data,
                UniqueCode = uniqueCode,
                QRCodeScanned = false,
                DateScan = null
            };

            context.QRCodeModels.Add(qrCode);
            context.SaveChanges();

            return base64;
        }

        // ============================================================
        // GENERATE QR CODE (RETURN BASE64 + BYTES)
        // ============================================================
        public (string Base64, byte[] Bytes) GenerateQRCodeWithBytes(string data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new BitmapByteQRCode(qrCodeData);

            var qrBytes = qrCode.GetGraphic(5);
            var base64 = $"data:image/png;base64,{Convert.ToBase64String(qrBytes)}";

            return (base64, qrBytes);
        }

        // ============================================================
        // VALIDATE QR CODE
        // ============================================================
        public async Task<QRCodeValidationResult> ValidateQRCodeAsync(string qrCode)
        {
            if (string.IsNullOrEmpty(qrCode))
                return QRCodeValidationResult.NotFound;

            await using var context = _factory.CreateDbContext();

            var qrCodeEntity = await context.QRCodeModels
                .FirstOrDefaultAsync(q => q.UniqueCode == qrCode);

            if (qrCodeEntity == null)
                return QRCodeValidationResult.NotFound;

            if (qrCodeEntity.QRCodeScanned)
                return QRCodeValidationResult.AlreadyScanned;

            qrCodeEntity.QRCodeScanned = true;
            qrCodeEntity.DateScan = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return QRCodeValidationResult.Valid;
        }
    }
}
