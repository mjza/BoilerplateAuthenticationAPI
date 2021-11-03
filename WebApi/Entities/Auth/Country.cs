using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace WebApi.Entities.Auth
{
    public partial class Country
    {
        public Country()
        {
            Cities = new HashSet<City>();            
        }

        [Key]
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public long TellCode { get; set; }
        public virtual ICollection<City> Cities { get; set; }        
    }
}
