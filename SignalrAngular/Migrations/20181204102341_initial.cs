using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SignalrAngular.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          
            migrationBuilder.CreateTable(
                name: "ChatMessage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(nullable: true),
                    NickName = table.Column<string>(nullable: true),
                    ConnectionID = table.Column<string>(nullable: true),
                    IsLoggedIn = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessage", x => x.Id);
                });


            migrationBuilder.CreateTable(
               name: "ChatHistory",
               columns: table => new
               {
                   Id = table.Column<int>(nullable: false)
                       .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                   ChatData = table.Column<string>(nullable: true),
                   GrpName = table.Column<string>(nullable: true),
                   UserId = table.Column<int>(nullable: false)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_ChatHistory", x => x.Id);
                   table.ForeignKey("FK_ChatMessageHistory", x => x.UserId, "ChatMessage", "Id");
               });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatHistory");

            migrationBuilder.DropTable(
                name: "ChatMessage");
        }
    }
}
