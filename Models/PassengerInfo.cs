namespace LegendaryCruises.Models
{
    public class PassengerInfo
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

      
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

     
        public string PassportNumber { get; set; } = string.Empty;

  
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
