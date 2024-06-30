using System.ComponentModel.DataAnnotations;

namespace Blaztrix.Models.Tetris
{
    public class SettingsModel
    {
        [Required]
        [Range(8, 20)]
        public int? Width { get; set; }

        [Required]
        [Range(8, 20)]
        public int? Height { get; set; }
    }
}