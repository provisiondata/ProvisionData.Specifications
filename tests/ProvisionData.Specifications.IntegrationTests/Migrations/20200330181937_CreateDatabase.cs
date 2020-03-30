namespace ProvisionData.Specifications.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;
    using System;

    public partial class CreateDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<String>(nullable: true),
                    DateOfBirth = table.Column<DateTime>(nullable: false),
                    Gender = table.Column<Int32>(nullable: false)
                },
                constraints: table => table.PrimaryKey("Id", x => x.Id));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "DateOfBirth", "Gender", "Name" },
                values: new Object[,]
                {
                    { new Guid("b7200d4a-ffb2-43b3-b996-ae8bc4a86553"), new DateTime(2007, 10, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Shyloh" },
                    { new Guid("3acb3238-196b-487a-91f0-0cf0481fb272"), new DateTime(1975, 11, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Charmaine" },
                    { new Guid("ef636071-8288-497e-a890-3cb2fe14dfb2"), new DateTime(1974, 10, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Doug" },
                    { new Guid("d08413b2-b072-458f-9e38-29ea89fc37b2"), new DateTime(2011, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Geordi" },
                    { new Guid("140e7335-0299-468d-a821-89709a99511f"), new DateTime(2008, 5, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Piper" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
