using System.ComponentModel.DataAnnotations;

namespace LegendaryCruises.Models
{
    public class CheckoutInfo
    {
        [Required(ErrorMessage = "Le prénom est requis.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Le nom est requis.")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "La rue est requise.")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Le numéro est requis.")]
        public string StreetNumber { get; set; }

        [Required(ErrorMessage = "Le code postal est requis.")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "La ville est requise.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Le pays est requis.")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Veuillez choisir une méthode de paiement.")]
        public string PaymentMethod { get; set; }
        [Required(ErrorMessage = "Le numéro de téléphone est requis.")]
        public string Phone { get; set; } = string.Empty;
        [Required(ErrorMessage = "Le numéro de passport est requis.")]
        public string PassportNumber { get; set; } = string.Empty;

    }

}
