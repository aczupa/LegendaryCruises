using Microsoft.AspNetCore.Identity;

namespace LegendaryCruises.Models
{
    public class UserProfile
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public IdentityUser User { get; set; }

        
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Street { get; set; } = string.Empty;
        public string StreetNumber { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

      
        public string PassportNumber { get; set; } = string.Empty;


        public string PaymentMethod { get; set; } = string.Empty;
        public List<QRCodeModel> QRCodes { get; set; } = new();
    }
}
