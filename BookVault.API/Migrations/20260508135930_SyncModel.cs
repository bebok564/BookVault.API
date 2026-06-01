using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookVault.API.Migrations
{
    /// <inheritdoc />
    public partial class SyncModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "Bio", "BirthDate", "FirstName", "LastName" },
                values: new object[,]
                {
                    { 1, "Polish fantasy writer", new DateOnly(1948, 6, 21), "Andrzej", "Sapkowski" },
                    { 2, "American science fiction author", null, "Frank", "Herbert" },
                    { 3, "English novelist and essayist", null, "George", "Orwell" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Fantasy" },
                    { 2, "Science Fiction" },
                    { 3, "Classic" },
                    { 4, "Horror" },
                    { 5, "Romance" }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "AuthorId", "AverageRating", "Description", "Isbn", "PageCount", "PublishedDate", "Title" },
                values: new object[,]
                {
                    { 1, 1, 0m, null, "9780316029186", 288, new DateOnly(1993, 1, 1), "The Witcher: The Last Wish" },
                    { 2, 2, 0m, null, "9780441013593", 412, new DateOnly(1965, 8, 1), "Dune" },
                    { 3, 3, 0m, null, "9780451524935", 328, new DateOnly(1949, 6, 8), "1984" }
                });

            migrationBuilder.InsertData(
                table: "BookCategory",
                columns: new[] { "BooksId", "CategoriesId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 3 },
                    { 2, 2 },
                    { 3, 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
              {
            migrationBuilder.DeleteData(
                table: "BookCategory",
                keyColumns: new[] { "BooksId", "CategoriesId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "BookCategory",
                keyColumns: new[] { "BooksId", "CategoriesId" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "BookCategory",
                keyColumns: new[] { "BooksId", "CategoriesId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "BookCategory",
                keyColumns: new[] { "BooksId", "CategoriesId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
