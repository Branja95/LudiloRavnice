using System.ComponentModel.DataAnnotations.Schema;

namespace AccountManaging.Models.Entities
{
    public class AccountForApproval
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string UserId { get; set; }
    }
}
