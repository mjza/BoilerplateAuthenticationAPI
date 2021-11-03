﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace WebApi.Entities.Auth
{
    public partial class Profession
    {
        public Profession()
        {
            Accounts = new HashSet<Account>();
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        // It is needed to ignore this navigation for security reasons, as all users can access this premitive type. 
        [IgnoreDataMember]
        public virtual ICollection<Account> Accounts { get; set; }
    }
}