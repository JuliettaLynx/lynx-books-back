using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace lynxbooksbackv2.Migrations
{
    /// <inheritdoc />
    public partial class RequiredName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Заполним null значения частью email до @
            // Для SQLite используем SUBSTR и INSTR
            migrationBuilder.Sql(@"
                UPDATE Users 
                SET DisplayName = SUBSTR(Email, 1, INSTR(Email, '@') - 1)
                WHERE DisplayName IS NULL OR DisplayName = ''
            ");

            // Затем изменим тип столбца на NOT NULL
            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "Users",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "Users",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100);
        }
    }
}