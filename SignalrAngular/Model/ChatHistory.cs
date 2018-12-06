using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SignalRWithAngular.SignalrAngular
{
    public class ChatHistory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ChatData { get; set; }
        public string GrpName { get; set; }
        [ForeignKey("ChatMessage")]
        public int UserId { get; set; }
    }
}