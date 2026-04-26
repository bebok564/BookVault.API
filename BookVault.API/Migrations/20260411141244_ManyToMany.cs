using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookVault.API.Migrations
{
    /// <inheritdoc />
    public partial class ManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "BookCategory",
                columns: new[] { "BooksId", "CategoriesId" },
                values: new object[,]
                {
                    { 2, 2 },
                    { 3, 3 }
                });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Isbn", "PageCount", "PublishedDate" },
                values: new object[] { "9780316029186", 288, new DateOnly(1993, 1, 1) });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Isbn", "PageCount", "PublishedDate" },
                values: new object[] { "9780441013593", 412, new DateOnly(1965, 8, 1) });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Isbn", "PageCount", "PublishedDate" },
                values: new object[] { "9780451524935", 328, new DateOnly(1949, 6, 8) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BookCategory",
                keyColumns: new[] { "BooksId", "CategoriesId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "BookCategory",
                keyColumns: new[] { "BooksId", "CategoriesId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Isbn", "PageCount", "PublishedDate" },
                values: new object[] { "", 0, new DateOnly(1, 1, 1) });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Isbn", "PageCount", "PublishedDate" },
                values: new object[] { "", 0, new DateOnly(1, 1, 1) });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Isbn", "PageCount", "PublishedDate" },
                values: new object[] { "", 0, new DateOnly(1, 1, 1) });
        }
    }
}
