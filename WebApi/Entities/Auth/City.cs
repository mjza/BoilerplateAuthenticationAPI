using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities.Auth
{
    public partial class City
    {
        public City()
        {
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string CountryId { get; set; }
        public virtual Country Country { get; set; }
    }
}
