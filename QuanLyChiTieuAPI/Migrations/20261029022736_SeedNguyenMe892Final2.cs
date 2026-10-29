using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuanLyChiTieuAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedNguyenMe892Final2 : Migration
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
                    { 5, "#FF5722", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chi tiêu ăn uống", "🍔", true, false, true, "Ăn uống", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 6, "#FF9800", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chi tiêu di chuyển", "🚌", true, false, true, "Di chuyển", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 7, "#E91E63", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chi tiêu mua sắm", "🛍️", true, false, true, "Mua sắm", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 8, "#9C27B0", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chi tiêu giải trí", "🎮", true, false, true, "Giải trí", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 9, "#F44336", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chi tiêu y tế", "🏥", true, false, true, "Y tế", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 10, "#3F51B5", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chi tiêu giáo dục", "📚", true, false, true, "Giáo dục", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 12, "#607D8B", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chi tiêu khác", "💸", true, false, true, "Khác", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 13, "#2196F3", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Thu nhập từ tiền gửi", "🏦", true, true, true, "Tiền gửi", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsActive", "PasswordHash", "PhoneNumber", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "demo@example.com", "Demo User", true, "$2a$11$C0YI5AAcccRXVViOOQx8AOppmYbHjegluMhV35vK3NOiiSF0p.n.i", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "nguyenme892@gmail.com", "Nguyen Van A", true, "$2a$11$v4t8R0yAZBJiGVjGB.DJ8eq/9b.s/F2eARe1nZelJn8.0y7onq//e", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Wallets",
                columns: new[] { "Id", "Balance", "CreatedAt", "Currency", "Description", "IsActive", "IsDefault", "Name", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { 1, 5000000m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "VND", null, true, true, "Tiền mặt", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 2, 20000000m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "VND", null, true, false, "Thẻ Ngân hàng", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 3, 15000000m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "VND", null, true, true, "Tiền mặt", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 4, 85000000m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "VND", null, true, false, "Tài khoản ngân hàng", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 5, -5000000m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "VND", null, true, false, "Thẻ tín dụng", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2 }
                });

            migrationBuilder.InsertData(
                table: "Budgets",
                columns: new[] { "Id", "Amount", "CategoryId", "CreatedAt", "EndDate", "IsActive", "Name", "StartDate", "UpdatedAt", "UserId", "WalletId" },
                values: new object[,]
                {
                    { 1, 10000000m, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Dự phòng", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1 },
                    { 2, 5000000m, 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Ăn uống", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1 },
                    { 3, 8000000m, 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Mua sắm", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 2 },
                    { 4, 3000000m, 10, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Giáo dục", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 2 },
                    { 5, 50000000m, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Dự phòng", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 3 },
                    { 6, 3000000m, 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Ăn uống", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 3 },
                    { 7, 5000000m, 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Mua sắm", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 4 }
                });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "Amount", "CategoryId", "CreatedAt", "IsIncome", "Note", "TransactionDate", "UpdatedAt", "UserId", "WalletId" },
                values: new object[,]
                {
                    { 1, 10000000m, 1, new DateTime(2026, 10, 29, 0, 0, 0, 0, DateTimeKind.Local), true, "Lương tháng 10", new DateTime(2026, 10, 24, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2026, 10, 29, 0, 0, 0, 0, DateTimeKind.Local), 1, 1 },
                    { 2, 3000000m, 13, new DateTime(2026, 10, 29, 0, 0, 0, 0, DateTimeKind.Local), true, "Thưởng", new DateTime(2026, 10, 27, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2026, 10, 29, 0, 0, 0, 0, DateTimeKind.Local), 1, 1 },
                    { 3, 850000m, 5, new DateTime(2026, 10, 29, 0, 0, 0, 0, DateTimeKind.Local), false, "Ăn uống", new DateTime(2026, 10, 25, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2026, 10, 29, 0, 0, 0, 0, DateTimeKind.Local), 1, 1 },
                    { 4, 620000m, 5, new DateTime(2026, 10, 29, 0, 0, 0, 0, DateTimeKind.Local), false, "Ăn uống", new DateTime(2026, 10, 26, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2026, 10, 29, 0, 0, 0, 0, DateTimeKind.Local), 1, 1 },
                    { 5, 2500000m, 7, new DateTime(2026, 10, 29, 0, 0, 0, 0, DateTimeKind.Local), false, "Mua sắm", new DateTime(2026, 10, 23, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2026, 10, 29, 0, 0, 0, 0, DateTimeKind.Local), 1, 2 },
                    { 6, 1800000m, 7, new DateTime(2026, 10, 29, 0, 0, 0, 0, DateTimeKind.Local), false, "Mua sắm", new DateTime(2026, 10, 28, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2026, 10, 29, 0, 0, 0, 0, DateTimeKind.Local), 1, 2 },
                    { 7, 450000m, 6, new DateTime(2026, 10, 29, 0, 0, 0, 0, DateTimeKind.Local), false, "Di chuyển", new DateTime(2026, 10, 27, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2026, 10, 29, 0, 0, 0, 0, DateTimeKind.Local), 1, 1 },
                    { 8, 1200000m, 12, new DateTime(2026, 10, 29, 0, 0, 0, 0, DateTimeKind.Local), false, "Tiện ích", new DateTime(2026, 10, 28, 0, 0, 0, 0, DateTimeKind.Local), new DateTime(2026, 10, 29, 0, 0, 0, 0, DateTimeKind.Local), 1, 2 },
                    { 2000, 25704536m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(3772), true, "Lương tháng 1", new DateTime(2024, 1, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(3774), 2, 3 },
                    { 2001, 27613821m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(4611), true, "Lương tháng 2", new DateTime(2024, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(4612), 2, 3 },
                    { 2002, 26312963m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(4701), true, "Lương tháng 3", new DateTime(2024, 3, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(4702), 2, 3 },
                    { 2003, 27564614m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(4748), true, "Lương tháng 4", new DateTime(2024, 4, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(4749), 2, 3 },
                    { 2004, 28806253m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(4797), true, "Lương tháng 5", new DateTime(2024, 5, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(4797), 2, 3 },
                    { 2005, 26286607m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(4840), true, "Lương tháng 6", new DateTime(2024, 6, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(4841), 2, 3 },
                    { 2006, 26601166m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(4881), true, "Lương tháng 7", new DateTime(2024, 7, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(4881), 2, 3 },
                    { 2007, 26301223m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(4933), true, "Lương tháng 8", new DateTime(2024, 8, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(4933), 2, 3 },
                    { 2008, 25176594m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(4982), true, "Lương tháng 9", new DateTime(2024, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(4983), 2, 3 },
                    { 2009, 27885984m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5031), true, "Lương tháng 10", new DateTime(2024, 10, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5031), 2, 3 },
                    { 2010, 25760965m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5078), true, "Lương tháng 11", new DateTime(2024, 11, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5079), 2, 3 },
                    { 2011, 28525761m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5120), true, "Lương tháng 12", new DateTime(2024, 12, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5120), 2, 3 },
                    { 2012, 27699853m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5158), true, "Lương tháng 1", new DateTime(2025, 1, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5159), 2, 3 },
                    { 2013, 28555773m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5197), true, "Lương tháng 2", new DateTime(2025, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5198), 2, 3 },
                    { 2014, 29531064m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5244), true, "Lương tháng 3", new DateTime(2025, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5245), 2, 3 },
                    { 2015, 27582069m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5286), true, "Lương tháng 4", new DateTime(2025, 4, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5287), 2, 3 },
                    { 2016, 25207549m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5641), true, "Lương tháng 5", new DateTime(2025, 5, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5641), 2, 3 },
                    { 2017, 27915802m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5693), true, "Lương tháng 6", new DateTime(2025, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5693), 2, 3 },
                    { 2018, 25019774m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5737), true, "Lương tháng 7", new DateTime(2025, 7, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5737), 2, 3 },
                    { 2019, 29025923m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5779), true, "Lương tháng 8", new DateTime(2025, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5780), 2, 3 },
                    { 2020, 26922868m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5817), true, "Lương tháng 9", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5817), 2, 3 },
                    { 2021, 25131697m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5876), true, "Lương tháng 10", new DateTime(2025, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5877), 2, 3 },
                    { 2022, 28908215m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5920), true, "Lương tháng 11", new DateTime(2025, 11, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5920), 2, 3 },
                    { 2023, 25575881m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5960), true, "Lương tháng 12", new DateTime(2025, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5961), 2, 3 },
                    { 2024, 28862142m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(5999), true, "Lương tháng 1", new DateTime(2026, 1, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(6000), 2, 3 },
                    { 2025, 28240730m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(6042), true, "Lương tháng 2", new DateTime(2026, 2, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(6043), 2, 3 },
                    { 2026, 25222491m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(6083), true, "Lương tháng 3", new DateTime(2026, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(6083), 2, 3 },
                    { 2027, 27579567m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(6139), true, "Lương tháng 4", new DateTime(2026, 4, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(6140), 2, 3 },
                    { 2028, 27101831m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(6182), true, "Lương tháng 5", new DateTime(2026, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(6183), 2, 3 },
                    { 2029, 28142318m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(6252), true, "Lương tháng 6", new DateTime(2026, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(6253), 2, 3 },
                    { 2030, 25066268m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(6294), true, "Lương tháng 7", new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(6295), 2, 3 },
                    { 2031, 26337192m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(6333), true, "Lương tháng 8", new DateTime(2026, 8, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(6334), 2, 3 },
                    { 2032, 28590874m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(6375), true, "Lương tháng 9", new DateTime(2026, 9, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(6375), 2, 3 },
                    { 2033, 27320468m, 1, new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(6417), true, "Lương tháng 10", new DateTime(2026, 10, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 653, DateTimeKind.Utc).AddTicks(6418), 2, 3 },
                    { 5034, 11701m, 8, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6006), false, "Chi tiêu tháng 1", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6007), 2, 5 },
                    { 5035, 187028m, 5, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6399), false, "Chi tiêu tháng 6", new DateTime(2024, 1, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6400), 2, 5 },
                    { 5036, 156752m, 7, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6437), false, "Chi tiêu tháng 10", new DateTime(2025, 8, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6437), 2, 4 },
                    { 5037, 590872m, 12, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6469), false, "Chi tiêu tháng 1", new DateTime(2025, 5, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6470), 2, 5 },
                    { 5038, 246126m, 9, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6498), false, "Chi tiêu tháng 6", new DateTime(2026, 12, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6498), 2, 3 },
                    { 5039, 82349m, 5, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6528), false, "Chi tiêu tháng 7", new DateTime(2024, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6528), 2, 4 },
                    { 5040, 157895m, 10, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6555), false, "Chi tiêu tháng 12", new DateTime(2026, 10, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6556), 2, 4 },
                    { 5041, 36871m, 12, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6617), false, "Chi tiêu tháng 4", new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6617), 2, 3 },
                    { 5042, 137493m, 7, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6648), false, "Chi tiêu tháng 2", new DateTime(2024, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6648), 2, 5 },
                    { 5043, 265166m, 10, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6675), false, "Chi tiêu tháng 1", new DateTime(2024, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6676), 2, 3 },
                    { 5044, 846836m, 12, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6702), false, "Chi tiêu tháng 7", new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6702), 2, 3 },
                    { 5045, 173218m, 7, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6730), false, "Chi tiêu tháng 3", new DateTime(2025, 8, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6730), 2, 3 },
                    { 5046, 182510m, 5, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6757), false, "Chi tiêu tháng 4", new DateTime(2026, 12, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6757), 2, 3 },
                    { 5047, 139727m, 5, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6784), false, "Chi tiêu tháng 11", new DateTime(2025, 7, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6784), 2, 4 },
                    { 5048, 113173m, 8, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6813), false, "Chi tiêu tháng 10", new DateTime(2025, 5, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6813), 2, 5 },
                    { 5049, 476121m, 6, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6839), false, "Chi tiêu tháng 5", new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6840), 2, 5 },
                    { 5050, 964263m, 12, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6869), false, "Chi tiêu tháng 3", new DateTime(2025, 5, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6869), 2, 5 },
                    { 5051, 824111m, 12, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6897), false, "Chi tiêu tháng 4", new DateTime(2025, 7, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6898), 2, 5 },
                    { 5052, 455201m, 6, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6924), false, "Chi tiêu tháng 3", new DateTime(2025, 7, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6924), 2, 4 },
                    { 5053, 173655m, 9, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6949), false, "Chi tiêu tháng 10", new DateTime(2024, 3, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6949), 2, 4 },
                    { 5054, 471942m, 12, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6975), false, "Chi tiêu tháng 12", new DateTime(2026, 10, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(6976), 2, 5 },
                    { 5055, 550811m, 9, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7004), false, "Chi tiêu tháng 2", new DateTime(2024, 8, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7004), 2, 5 },
                    { 5056, 140085m, 10, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7030), false, "Chi tiêu tháng 10", new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7030), 2, 4 },
                    { 5057, 58352m, 5, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7073), false, "Chi tiêu tháng 4", new DateTime(2025, 3, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7073), 2, 3 },
                    { 5058, 475333m, 12, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7101), false, "Chi tiêu tháng 6", new DateTime(2024, 4, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7102), 2, 4 },
                    { 5059, 442621m, 7, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7137), false, "Chi tiêu tháng 3", new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7138), 2, 3 },
                    { 5060, 482449m, 7, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7166), false, "Chi tiêu tháng 6", new DateTime(2026, 8, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7166), 2, 3 },
                    { 5061, 891777m, 12, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7195), false, "Chi tiêu tháng 4", new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7195), 2, 3 },
                    { 5062, 370529m, 7, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7222), false, "Chi tiêu tháng 12", new DateTime(2025, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7223), 2, 4 },
                    { 5063, 101120m, 5, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7251), false, "Chi tiêu tháng 1", new DateTime(2024, 12, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7251), 2, 4 },
                    { 5064, 187404m, 7, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7278), false, "Chi tiêu tháng 8", new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7279), 2, 4 },
                    { 5065, 70387m, 5, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7306), false, "Chi tiêu tháng 4", new DateTime(2025, 4, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7307), 2, 5 },
                    { 5066, 415893m, 9, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7334), false, "Chi tiêu tháng 3", new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7334), 2, 3 },
                    { 5067, 362318m, 6, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7359), false, "Chi tiêu tháng 8", new DateTime(2025, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7360), 2, 3 },
                    { 5068, 479158m, 9, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7390), false, "Chi tiêu tháng 3", new DateTime(2025, 11, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7390), 2, 4 },
                    { 5069, 470229m, 9, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7417), false, "Chi tiêu tháng 12", new DateTime(2026, 7, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7417), 2, 4 },
                    { 5070, 164466m, 8, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7443), false, "Chi tiêu tháng 4", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7443), 2, 5 },
                    { 5071, 506789m, 12, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7467), false, "Chi tiêu tháng 1", new DateTime(2025, 10, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7468), 2, 5 },
                    { 5072, 87196m, 8, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7494), false, "Chi tiêu tháng 12", new DateTime(2025, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7494), 2, 3 },
                    { 5073, 182804m, 10, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7522), false, "Chi tiêu tháng 3", new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7522), 2, 3 },
                    { 5074, 505751m, 12, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7548), false, "Chi tiêu tháng 12", new DateTime(2026, 6, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7549), 2, 3 },
                    { 5075, 201010m, 10, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7576), false, "Chi tiêu tháng 1", new DateTime(2026, 8, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7576), 2, 3 },
                    { 5076, 573408m, 12, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7603), false, "Chi tiêu tháng 5", new DateTime(2024, 11, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7604), 2, 3 },
                    { 5077, 147931m, 5, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7629), false, "Chi tiêu tháng 3", new DateTime(2024, 5, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7629), 2, 3 },
                    { 5078, 339303m, 7, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7654), false, "Chi tiêu tháng 10", new DateTime(2026, 10, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7655), 2, 3 },
                    { 5079, 147651m, 5, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7680), false, "Chi tiêu tháng 7", new DateTime(2024, 7, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7681), 2, 3 },
                    { 5080, 680437m, 12, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7716), false, "Chi tiêu tháng 5", new DateTime(2026, 7, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7717), 2, 5 },
                    { 5081, 288309m, 10, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7746), false, "Chi tiêu tháng 8", new DateTime(2025, 11, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7746), 2, 5 },
                    { 5082, 137341m, 10, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7771), false, "Chi tiêu tháng 12", new DateTime(2024, 10, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7772), 2, 5 },
                    { 5083, 203779m, 12, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7800), false, "Chi tiêu tháng 10", new DateTime(2026, 10, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7800), 2, 3 },
                    { 5084, 69270m, 12, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7827), false, "Chi tiêu tháng 5", new DateTime(2025, 7, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7828), 2, 5 },
                    { 5085, 140579m, 12, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7855), false, "Chi tiêu tháng 7", new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7855), 2, 5 },
                    { 5086, 385883m, 9, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7881), false, "Chi tiêu tháng 3", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7882), 2, 3 },
                    { 5087, 82638m, 8, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7908), false, "Chi tiêu tháng 12", new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7909), 2, 4 },
                    { 5088, 127600m, 8, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7935), false, "Chi tiêu tháng 1", new DateTime(2026, 4, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(7935), 2, 4 },
                    { 5089, 964773m, 12, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8103), false, "Chi tiêu tháng 3", new DateTime(2024, 11, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8104), 2, 3 },
                    { 5090, 174295m, 5, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8139), false, "Chi tiêu tháng 11", new DateTime(2025, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8139), 2, 4 },
                    { 5091, 149823m, 12, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8166), false, "Chi tiêu tháng 8", new DateTime(2026, 11, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8167), 2, 4 },
                    { 5092, 354451m, 9, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8193), false, "Chi tiêu tháng 2", new DateTime(2025, 10, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8194), 2, 4 },
                    { 5093, 76796m, 8, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8220), false, "Chi tiêu tháng 9", new DateTime(2024, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8220), 2, 4 },
                    { 5094, 425782m, 7, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8247), false, "Chi tiêu tháng 5", new DateTime(2024, 8, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8248), 2, 5 },
                    { 5095, 30925m, 8, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8274), false, "Chi tiêu tháng 12", new DateTime(2024, 4, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8275), 2, 3 },
                    { 5096, 182944m, 8, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8301), false, "Chi tiêu tháng 10", new DateTime(2025, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8301), 2, 4 },
                    { 5097, 953272m, 9, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8330), false, "Chi tiêu tháng 4", new DateTime(2024, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8331), 2, 3 },
                    { 5098, 197235m, 8, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8359), false, "Chi tiêu tháng 8", new DateTime(2026, 5, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8360), 2, 3 },
                    { 5099, 364539m, 7, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8388), false, "Chi tiêu tháng 3", new DateTime(2024, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8389), 2, 3 },
                    { 5100, 189843m, 6, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8417), false, "Chi tiêu tháng 9", new DateTime(2025, 1, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8417), 2, 3 },
                    { 5101, 17289m, 8, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8450), false, "Chi tiêu tháng 1", new DateTime(2025, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8451), 2, 5 },
                    { 5102, 862555m, 12, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8479), false, "Chi tiêu tháng 7", new DateTime(2024, 7, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8480), 2, 3 },
                    { 5103, 198439m, 5, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8506), false, "Chi tiêu tháng 2", new DateTime(2026, 11, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8507), 2, 3 },
                    { 5104, 161931m, 6, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8533), false, "Chi tiêu tháng 8", new DateTime(2024, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8533), 2, 4 },
                    { 5105, 276996m, 10, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8561), false, "Chi tiêu tháng 5", new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8562), 2, 4 },
                    { 5106, 298677m, 12, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8589), false, "Chi tiêu tháng 2", new DateTime(2025, 7, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8590), 2, 4 },
                    { 5107, 146298m, 10, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8616), false, "Chi tiêu tháng 7", new DateTime(2026, 11, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8616), 2, 4 },
                    { 5108, 466806m, 6, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8642), false, "Chi tiêu tháng 2", new DateTime(2024, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8643), 2, 4 },
                    { 5109, 220311m, 12, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8670), false, "Chi tiêu tháng 9", new DateTime(2026, 11, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8670), 2, 4 },
                    { 5110, 481690m, 12, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8698), false, "Chi tiêu tháng 5", new DateTime(2025, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8698), 2, 3 },
                    { 5111, 738900m, 9, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8726), false, "Chi tiêu tháng 5", new DateTime(2025, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8726), 2, 3 },
                    { 5112, 47235m, 12, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8753), false, "Chi tiêu tháng 4", new DateTime(2024, 9, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8753), 2, 3 },
                    { 5113, 964023m, 12, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8778), false, "Chi tiêu tháng 9", new DateTime(2024, 6, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8779), 2, 3 },
                    { 5114, 75743m, 5, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8806), false, "Chi tiêu tháng 5", new DateTime(2025, 11, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8807), 2, 4 },
                    { 5115, 180814m, 10, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8832), false, "Chi tiêu tháng 10", new DateTime(2026, 4, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8833), 2, 3 },
                    { 5116, 335410m, 6, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8860), false, "Chi tiêu tháng 8", new DateTime(2024, 11, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8860), 2, 5 },
                    { 5117, 247897m, 6, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8885), false, "Chi tiêu tháng 6", new DateTime(2024, 9, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8885), 2, 4 },
                    { 5118, 189838m, 8, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8911), false, "Chi tiêu tháng 9", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8911), 2, 5 },
                    { 5119, 130109m, 8, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8939), false, "Chi tiêu tháng 7", new DateTime(2024, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8939), 2, 5 },
                    { 5120, 891849m, 9, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8966), false, "Chi tiêu tháng 1", new DateTime(2025, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(8967), 2, 4 },
                    { 5121, 312103m, 7, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(9007), false, "Chi tiêu tháng 6", new DateTime(2025, 12, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(9008), 2, 3 },
                    { 5122, 281938m, 10, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(9038), false, "Chi tiêu tháng 12", new DateTime(2024, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(9039), 2, 5 },
                    { 5123, 90773m, 8, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(9065), false, "Chi tiêu tháng 10", new DateTime(2025, 10, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(9065), 2, 3 },
                    { 5124, 780771m, 12, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(9091), false, "Chi tiêu tháng 10", new DateTime(2026, 12, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(9091), 2, 3 },
                    { 5125, 17000m, 8, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(9117), false, "Chi tiêu tháng 2", new DateTime(2024, 10, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(9118), 2, 4 },
                    { 5126, 361119m, 7, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(9144), false, "Chi tiêu tháng 2", new DateTime(2026, 3, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(9144), 2, 3 },
                    { 5127, 299260m, 10, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(9171), false, "Chi tiêu tháng 9", new DateTime(2024, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(9172), 2, 4 },
                    { 5128, 78328m, 8, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(9199), false, "Chi tiêu tháng 11", new DateTime(2026, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(9199), 2, 3 },
                    { 5129, 107026m, 10, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(9226), false, "Chi tiêu tháng 12", new DateTime(2024, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(9226), 2, 4 },
                    { 5130, 528982m, 12, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(9253), false, "Chi tiêu tháng 12", new DateTime(2024, 5, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(9253), 2, 5 },
                    { 5131, 253320m, 10, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(9280), false, "Chi tiêu tháng 9", new DateTime(2025, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(9280), 2, 5 },
                    { 5132, 201619m, 10, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(9307), false, "Chi tiêu tháng 4", new DateTime(2024, 12, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(9308), 2, 3 },
                    { 5133, 292406m, 9, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(9335), false, "Chi tiêu tháng 9", new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(9336), 2, 3 },
                    { 5134, 732314m, 9, new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(9362), false, "Chi tiêu tháng 9", new DateTime(2024, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 29, 2, 27, 34, 654, DateTimeKind.Utc).AddTicks(9363), 2, 3 }
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
