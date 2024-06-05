using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSPhoto.Common.Migrations
{
    /// <inheritdoc />
    public partial class FixAlbumPaths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // one-time data fix to align album paths with photo paths; making both absolute
            var mediaPath = Environment.GetEnvironmentVariable("MEDIA_PATH");

            if (!mediaPath.EndsWith(Path.DirectorySeparatorChar))
                mediaPath += Path.DirectorySeparatorChar;

            migrationBuilder.Sql($"UPDATE albums SET Path = '{mediaPath}' || Path WHERE Path NOT LIKE '{mediaPath}%'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
