using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.Dtos
{
    public class UpdateCategoryDto
    {
        public int Id { get; set; }
        
        [Display(Name = "اسم التصنيف")]
        [Required]
        public string Name { get; set; }
    }
}
