using AccountManaging.User;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountManaging.Models.Entities
{
    public class BanedManager
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public ApplicationUser User { get; set; }
    }
}
