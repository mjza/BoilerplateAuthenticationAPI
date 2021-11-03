using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities.Auth
{
    public partial class Locale
    {
        public Locale()
        {
            Accounts = new HashSet<Account>();
        }

        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        // It is needed to ignore this navigation for security reasons, as all users can access this premitive type. 
        [IgnoreDataMember]
        public virtual ICollection<Account> Accounts { get; set; }
    }
}
