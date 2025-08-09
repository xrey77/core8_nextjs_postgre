using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace core8_nextjs_postgre.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    category = table.Column<string>(type: "varchar", nullable: true),
                    descriptions = table.Column<string>(type: "varchar", nullable: true),
                    qty = table.Column<int>(type: "integer", nullable: false),
                    unit = table.Column<string>(type: "varchar", nullable: true),
                    costPrice = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    sellPrice = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    productPicture = table.Column<string>(type: "varchar", nullable: true),
                    salePrice = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    alertStocks = table.Column<int>(type: "integer", nullable: false),
                    criticalStocks = table.Column<int>(type: "integer", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    lastname = table.Column<string>(type: "varchar", nullable: true),
                    firstname = table.Column<string>(type: "varchar", nullable: true),
                    username = table.Column<string>(type: "varchar", nullable: true),
                    password = table.Column<string>(type: "varchar", nullable: true),
                    email = table.Column<string>(type: "varchar", nullable: true),
                    mobile = table.Column<string>(type: "varchar", nullable: true),
                    roles = table.Column<string>(type: "varchar", nullable: true),
                    isactivated = table.Column<int>(type: "integer", nullable: false),
                    isblocked = table.Column<int>(type: "integer", nullable: false),
                    mailtoken = table.Column<int>(type: "integer", nullable: false),
                    qrcodeurl = table.Column<string>(type: "varchar", nullable: true),
                    profilepic = table.Column<string>(type: "varchar", nullable: true),
                    secretkey = table.Column<string>(type: "text", nullable: true),
                    createdAt = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
