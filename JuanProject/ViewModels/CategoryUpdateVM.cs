using System.ComponentModel.DataAnnotations;

namespace JuanProject.ViewModels
{
    public class CategoryUpdateVM
    {
        [Required, MaxLength(50)]
        public string? Name { get; set; }
        public int? Id { get; set; }
    }
}
