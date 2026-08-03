using System.ComponentModel.DataAnnotations;
using LegendaryCruises.Models;

namespace LegendaryCruises.Models.DTOs
{
    public class CabinDto
    {
        [Required]
        public CabinType CabinType { get; set; }

        [Range(0, int.MaxValue)]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }
    }
}