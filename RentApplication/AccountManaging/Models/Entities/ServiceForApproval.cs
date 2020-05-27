using System.ComponentModel.DataAnnotations.Schema;

namespace AccountManaging.Models.Entities
{
    public class ServiceForApproval
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
    }
}
