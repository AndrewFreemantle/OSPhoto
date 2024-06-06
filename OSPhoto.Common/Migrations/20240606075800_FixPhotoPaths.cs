using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSPhoto.Common.Migrations
{
    /// <inheritdoc />
    public partial class FixPhotoPaths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // one-time data fix to photo paths; changing any imported root to OSPhoto's configured media path root
            var synoSharePathPrefix = Environment.GetEnvironmentVariable("SYNO_SHARE_PATH_PREFIX") ?? "/volume1/photo/";
            var mediaPath = Environment.GetEnvironmentVariable("MEDIA_PATH");

            if (!mediaPath.EndsWith(Path.DirectorySeparatorChar))
                mediaPath += Path.DirectorySeparatorChar;

            migrationBuilder.Sql($"UPDATE photos SET Path = '{mediaPath}' || SUBSTRING(Path, {synoSharePathPrefix.Length + 1}) WHERE Path LIKE '{synoSharePathPrefix}%'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
