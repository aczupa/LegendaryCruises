using System.ComponentModel.DataAnnotations;

namespace LegendaryCruises.Models
{
    public enum CabinType
    {
        [Display(Name = "Cabine intérieure")]
        Interior,

        [Display(Name = "Cabine avec vue mer")]
        OceanView,

        [Display(Name = "Cabine avec balcon")]
        Balcony,

        [Display(Name = "Suite")]
        Suite
    }
}