using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SignalRWithAngular.SignalrAngular
{
    public class ChatMessage
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string ConnectionID { get; set; }
        public int IsLoggedIn { get; set; }
    }
}