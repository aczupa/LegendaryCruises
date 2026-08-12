using System.ComponentModel.DataAnnotations;

namespace LegendaryCruises.Models
{
    public class PassengerInfo
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le prénom est requis.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom est requis.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La date de naissance est requise.")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Le numéro de passeport est requis.")]
        public string PassportNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est requis.")]
        [EmailAddress(ErrorMessage = "Email invalide.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le téléphone est requis.")]
        public string Phone { get; set; } = string.Empty;
    }

}
