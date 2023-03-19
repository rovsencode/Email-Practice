using JuanProject.Models;
using System.ComponentModel.DataAnnotations;
namespace JuanProject.ViewModels
{
    public class CreateCategoryVM
    {
        [Required,MaxLength(50)]
        public string? Name { get; set; }
        public int? Id { get; set; }
        public List<Product>? Products { get; set; }
    }
}
