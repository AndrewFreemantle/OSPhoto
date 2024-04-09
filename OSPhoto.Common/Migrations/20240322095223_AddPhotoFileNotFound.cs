using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSPhoto.Common.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoFileNotFound : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "photos_file_not_found",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CameraMake = table.Column<string>(type: "TEXT", nullable: true),
                    CameraModel = table.Column<string>(type: "TEXT", nullable: true),
                    TimeTaken = table.Column<string>(type: "TEXT", nullable: true),
                    ShareId = table.Column<int>(type: "INTEGER", nullable: true),
                    ImportFilename = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_photos_file_not_found", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "photos_file_not_found");
        }
    }
}
