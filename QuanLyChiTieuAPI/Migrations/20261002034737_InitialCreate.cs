using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuanLyChiTieuAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    IsIncome = table.Column<bool>(type: "bit", nullable: false),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wallets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Budgets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SpentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    WalletId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budgets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Budgets_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Budgets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Budgets_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsIncome = table.Column<bool>(type: "bit", nullable: false),
                    WalletId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactions_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Color", "CreatedAt", "Description", "Icon", "IsActive", "IsIncome", "IsSystem", "Name", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { 1, "#4CAF50", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Thu nhập từ lương", "💰", true, true, true, "Lương", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 2, "#8BC34A", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Thu nhập từ thưởng", "🎁", true, true, true, "Thưởng", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 3, "#CDDC39", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Thu nhập từ đầu tư", "📈", true, true, true, "Đầu tư", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 4, "#FFEB3B", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Thu nhập khác", "💵", true, true, true, "Khác (Thu)", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 5, "#FF5722", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chi tiêu ăn uống", "🍔", true, false, true, "Ăn uống", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 6, "#FF9800", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chi tiêu di chuyển", "🚌", true, false, true, "Di chuyển", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 7, "#E91E63", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chi tiêu mua sắm", "🛍️", true, false, true, "Mua sắm", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 8, "#9C27B0", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chi tiêu giải trí", "🎮", true, false, true, "Giải trí", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 9, "#F44336", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chi tiêu y tế", "🏥", true, false, true, "Y tế", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 10, "#3F51B5", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chi tiêu giáo dục", "📚", true, false, true, "Giáo dục", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 11, "#2196F3", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chi tiêu tiện ích", "💡", true, false, true, "Tiện ích", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 12, "#607D8B", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chi tiêu khác", "💸", true, false, true, "Khác (Chi)", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_CategoryId",
                table: "Budgets",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_StartDate_EndDate",
                table: "Budgets",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_UserId",
                table: "Budgets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_WalletId",
                table: "Budgets",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UserId",
                table: "Categories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CategoryId",
                table: "Transactions",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionDate",
                table: "Transactions",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_WalletId",
                table: "Transactions",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserId_IsDefault",
                table: "Wallets",
                columns: new[] { "UserId", "IsDefault" },
                unique: true,
                filter: "[IsDefault] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Budgets");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Wallets");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
