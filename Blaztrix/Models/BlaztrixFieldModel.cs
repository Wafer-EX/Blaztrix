using System.ComponentModel.DataAnnotations;

namespace Blaztrix.Models
{
    public class BlaztrixFieldModel
    {
        [Required]
        [Range(8, 20)]
        public int? Width { get; set; }

        [Required]
        [Range(8, 20)]
        public int? Height { get; set; }
    }
}