using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace webBookingSolution.Data.Migrations
{
    public partial class updateDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                table: "HallImages",
                nullable: false,
                defaultValue: new DateTime(2020, 11, 23, 11, 30, 24, 368, DateTimeKind.Local).AddTicks(6357),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2020, 11, 11, 16, 51, 10, 210, DateTimeKind.Local).AddTicks(1130));

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Books",
                nullable: false,
                defaultValue: "Chưa thanh toán",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "0");

            migrationBuilder.AlterColumn<string>(
                name: "Season",
                table: "Books",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "0");

            migrationBuilder.AlterColumn<DateTime>(
                name: "BookDate",
                table: "Books",
                nullable: false,
                defaultValue: new DateTime(2020, 11, 23, 11, 30, 24, 350, DateTimeKind.Local).AddTicks(3460),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2020, 11, 11, 16, 51, 10, 190, DateTimeKind.Local).AddTicks(7864));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                table: "HallImages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2020, 11, 11, 16, 51, 10, 210, DateTimeKind.Local).AddTicks(1130),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 11, 23, 11, 30, 24, 368, DateTimeKind.Local).AddTicks(6357));

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "0",
                oldClrType: typeof(string),
                oldDefaultValue: "Chưa thanh toán");

            migrationBuilder.AlterColumn<string>(
                name: "Season",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "0",
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<DateTime>(
                name: "BookDate",
                table: "Books",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2020, 11, 11, 16, 51, 10, 190, DateTimeKind.Local).AddTicks(7864),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 11, 23, 11, 30, 24, 350, DateTimeKind.Local).AddTicks(3460));
        }
    }
}
