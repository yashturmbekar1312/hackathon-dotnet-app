using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PersonalFinanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddOccupationCurrencyAndAnnualIncomeToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "bank_aggregators",
                keyColumn: "id",
                keyValue: new Guid("62306dba-9063-433d-81fd-e91867630d49"));

            migrationBuilder.DeleteData(
                table: "bank_aggregators",
                keyColumn: "id",
                keyValue: new Guid("6a8b7ff9-99b5-4fd7-a7c8-cf4551100dc5"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("0400c609-e7be-4a82-ab41-8be9b1e91af6"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("250e2952-639e-48e3-a234-5491ce25bbe7"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("28ccd85e-4ffe-403b-a54c-803c2e17dec0"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("3a567b23-0dba-4dd7-a114-037a764cb497"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("a0f3ea59-e4c6-4806-8143-d96ebc4b583f"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("a5323a77-5fa3-4f5b-be0c-f98524443e77"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("add7ad65-2c2a-46ee-83eb-0416f44632bf"));

            migrationBuilder.DeleteData(
                table: "merchants",
                keyColumn: "id",
                keyValue: new Guid("0510e63d-05f6-471f-a982-2182d8925f1b"));

            migrationBuilder.DeleteData(
                table: "merchants",
                keyColumn: "id",
                keyValue: new Guid("4c3f8ad3-909e-4c76-8ca9-8d0c2eb9044a"));

            migrationBuilder.DeleteData(
                table: "merchants",
                keyColumn: "id",
                keyValue: new Guid("70469cbd-6cb0-4a82-967c-5363fb371ec1"));

            migrationBuilder.DeleteData(
                table: "merchants",
                keyColumn: "id",
                keyValue: new Guid("ae30c3f8-8362-4e30-a8d5-33c5d0d92564"));

            migrationBuilder.DeleteData(
                table: "merchants",
                keyColumn: "id",
                keyValue: new Guid("b13bdae4-53c1-4019-b8bc-0dab1c66ce36"));

            migrationBuilder.DeleteData(
                table: "merchants",
                keyColumn: "id",
                keyValue: new Guid("fdb1d8a8-4287-4592-9d70-4bb2d6425def"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("2ab3b432-71f0-4d0a-9cc7-db52f2b8be3c"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("4c4d8137-c0db-4b12-8697-cac2646d1c2b"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("689dc3ce-bdef-4446-8d41-9b5e061da701"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("a089ab47-fcf9-471f-a937-57017204f471"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("f90d138c-c9c3-47dd-bc17-25a3a9c3e63d"));

            migrationBuilder.AddColumn<decimal>(
                name: "annual_income",
                table: "users",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.InsertData(
                table: "bank_aggregators",
                columns: new[] { "id", "api_endpoint", "created_at", "is_active", "name", "updated_at" },
                values: new object[,]
                {
                    { new Guid("14e2469f-ec3e-4a7e-8867-68f6213209a2"), "https://api.demo-bank.com", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5822), true, "Demo Bank Connector", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5823) },
                    { new Guid("1c6e61ab-3128-4f9e-9b74-bb823b125036"), "https://api.openbanking-sim.com", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5828), true, "Simulated Open Banking", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5828) }
                });

            migrationBuilder.InsertData(
                table: "categories",
                columns: new[] { "id", "color", "created_at", "icon", "is_system_defined", "name", "parent_id", "type", "updated_at" },
                values: new object[,]
                {
                    { new Guid("0324ff7c-4f30-46cb-bc89-7b29390876f4"), "#F7DC6F", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5257), "plane", true, "Travel", null, "EXPENSE", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5258) },
                    { new Guid("0cf44ffb-f363-43a0-9c19-d3cf1fb893c1"), "#2ECC71", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5263), "dollar-sign", true, "Salary", null, "INCOME", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5264) },
                    { new Guid("361fc2e0-7378-4efa-93b2-6aca7956ff1e"), "#98D8C8", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5246), "graduation-cap", true, "Education", null, "EXPENSE", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5247) },
                    { new Guid("47fa3398-77cb-46e0-a0a2-3d47fdb8168f"), "#DDA0DD", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5240), "medical", true, "Healthcare", null, "EXPENSE", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5241) },
                    { new Guid("48ad4adb-a928-4c9d-b01f-d42267754753"), "#FFEAA7", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5235), "receipt", true, "Bills & Utilities", null, "EXPENSE", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5236) },
                    { new Guid("4d4d09fc-52e7-402b-8c81-5a8276e2a808"), "#FF6B6B", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5204), "restaurant", true, "Food & Dining", null, "EXPENSE", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5205) },
                    { new Guid("7e3d462d-e98d-4e7a-9830-1b5d4c0feb8d"), "#27AE60", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5271), "briefcase", true, "Freelance", null, "INCOME", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5272) },
                    { new Guid("87c77c4f-0743-431e-907c-fed54b1e39f2"), "#4ECDC4", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5216), "car", true, "Transportation", null, "EXPENSE", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5217) },
                    { new Guid("9abe57a3-8294-4d25-877f-1142a510969f"), "#45B7D1", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5223), "shopping-cart", true, "Shopping", null, "EXPENSE", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5223) },
                    { new Guid("a4a9ac33-0e20-488c-b40b-f9c77088b4cd"), "#96CEB4", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5228), "movie", true, "Entertainment", null, "EXPENSE", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5229) },
                    { new Guid("a602f834-052b-4d3e-9679-188457194d48"), "#16A085", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5277), "trending-up", true, "Investment Returns", null, "INCOME", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5278) },
                    { new Guid("ff5f2d6b-feee-4523-acfd-75531b21c8fb"), "#95A5A6", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5282), "exchange", true, "Transfer", null, "TRANSFER", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5283) }
                });

            migrationBuilder.InsertData(
                table: "merchants",
                columns: new[] { "id", "category_id", "created_at", "is_verified", "mcc_code", "name", "updated_at" },
                values: new object[,]
                {
                    { new Guid("0f55cd75-e995-4da2-b6fb-a1a7e093b48e"), new Guid("9abe57a3-8294-4d25-877f-1142a510969f"), new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5928), true, "5399", "Amazon", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5929) },
                    { new Guid("9ed972fb-e35d-4f23-ba24-ddfa1d699d0e"), new Guid("4d4d09fc-52e7-402b-8c81-5a8276e2a808"), new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5910), true, "5812", "Swiggy", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5911) },
                    { new Guid("d1c167e0-2777-4473-8417-b1c6cb772efb"), new Guid("a4a9ac33-0e20-488c-b40b-f9c77088b4cd"), new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5937), true, "4899", "Netflix", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5937) },
                    { new Guid("d40b3c6e-0bdb-454a-8202-1fa2c905696a"), new Guid("87c77c4f-0743-431e-907c-fed54b1e39f2"), new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5922), true, "4121", "Uber", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5923) },
                    { new Guid("f7d47b2c-1bb2-470d-8c43-292a798f1f32"), new Guid("48ad4adb-a928-4c9d-b01f-d42267754753"), new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5942), true, "4900", "BSES Delhi", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5943) },
                    { new Guid("f970f25d-e975-4e5b-a4ff-8ac94f3cd53b"), new Guid("4d4d09fc-52e7-402b-8c81-5a8276e2a808"), new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5917), true, "5812", "Zomato", new DateTime(2025, 8, 12, 20, 19, 14, 597, DateTimeKind.Utc).AddTicks(5917) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "bank_aggregators",
                keyColumn: "id",
                keyValue: new Guid("14e2469f-ec3e-4a7e-8867-68f6213209a2"));

            migrationBuilder.DeleteData(
                table: "bank_aggregators",
                keyColumn: "id",
                keyValue: new Guid("1c6e61ab-3128-4f9e-9b74-bb823b125036"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("0324ff7c-4f30-46cb-bc89-7b29390876f4"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("0cf44ffb-f363-43a0-9c19-d3cf1fb893c1"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("361fc2e0-7378-4efa-93b2-6aca7956ff1e"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("47fa3398-77cb-46e0-a0a2-3d47fdb8168f"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("7e3d462d-e98d-4e7a-9830-1b5d4c0feb8d"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("a602f834-052b-4d3e-9679-188457194d48"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("ff5f2d6b-feee-4523-acfd-75531b21c8fb"));

            migrationBuilder.DeleteData(
                table: "merchants",
                keyColumn: "id",
                keyValue: new Guid("0f55cd75-e995-4da2-b6fb-a1a7e093b48e"));

            migrationBuilder.DeleteData(
                table: "merchants",
                keyColumn: "id",
                keyValue: new Guid("9ed972fb-e35d-4f23-ba24-ddfa1d699d0e"));

            migrationBuilder.DeleteData(
                table: "merchants",
                keyColumn: "id",
                keyValue: new Guid("d1c167e0-2777-4473-8417-b1c6cb772efb"));

            migrationBuilder.DeleteData(
                table: "merchants",
                keyColumn: "id",
                keyValue: new Guid("d40b3c6e-0bdb-454a-8202-1fa2c905696a"));

            migrationBuilder.DeleteData(
                table: "merchants",
                keyColumn: "id",
                keyValue: new Guid("f7d47b2c-1bb2-470d-8c43-292a798f1f32"));

            migrationBuilder.DeleteData(
                table: "merchants",
                keyColumn: "id",
                keyValue: new Guid("f970f25d-e975-4e5b-a4ff-8ac94f3cd53b"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("48ad4adb-a928-4c9d-b01f-d42267754753"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("4d4d09fc-52e7-402b-8c81-5a8276e2a808"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("87c77c4f-0743-431e-907c-fed54b1e39f2"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("9abe57a3-8294-4d25-877f-1142a510969f"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("a4a9ac33-0e20-488c-b40b-f9c77088b4cd"));

            migrationBuilder.DropColumn(
                name: "annual_income",
                table: "users");

            migrationBuilder.InsertData(
                table: "bank_aggregators",
                columns: new[] { "id", "api_endpoint", "created_at", "is_active", "name", "updated_at" },
                values: new object[,]
                {
                    { new Guid("62306dba-9063-433d-81fd-e91867630d49"), "https://api.openbanking-sim.com", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(1112), true, "Simulated Open Banking", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(1113) },
                    { new Guid("6a8b7ff9-99b5-4fd7-a7c8-cf4551100dc5"), "https://api.demo-bank.com", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(1104), true, "Demo Bank Connector", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(1105) }
                });

            migrationBuilder.InsertData(
                table: "categories",
                columns: new[] { "id", "color", "created_at", "icon", "is_system_defined", "name", "parent_id", "type", "updated_at" },
                values: new object[,]
                {
                    { new Guid("0400c609-e7be-4a82-ab41-8be9b1e91af6"), "#2ECC71", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(337), "dollar-sign", true, "Salary", null, "INCOME", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(338) },
                    { new Guid("250e2952-639e-48e3-a234-5491ce25bbe7"), "#F7DC6F", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(330), "plane", true, "Travel", null, "EXPENSE", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(331) },
                    { new Guid("28ccd85e-4ffe-403b-a54c-803c2e17dec0"), "#95A5A6", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(364), "exchange", true, "Transfer", null, "TRANSFER", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(364) },
                    { new Guid("2ab3b432-71f0-4d0a-9cc7-db52f2b8be3c"), "#4ECDC4", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(125), "car", true, "Transportation", null, "EXPENSE", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(126) },
                    { new Guid("3a567b23-0dba-4dd7-a114-037a764cb497"), "#16A085", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(352), "trending-up", true, "Investment Returns", null, "INCOME", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(353) },
                    { new Guid("4c4d8137-c0db-4b12-8697-cac2646d1c2b"), "#FF6B6B", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(115), "restaurant", true, "Food & Dining", null, "EXPENSE", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(116) },
                    { new Guid("689dc3ce-bdef-4446-8d41-9b5e061da701"), "#96CEB4", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(298), "movie", true, "Entertainment", null, "EXPENSE", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(299) },
                    { new Guid("a089ab47-fcf9-471f-a937-57017204f471"), "#FFEAA7", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(306), "receipt", true, "Bills & Utilities", null, "EXPENSE", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(307) },
                    { new Guid("a0f3ea59-e4c6-4806-8143-d96ebc4b583f"), "#98D8C8", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(322), "graduation-cap", true, "Education", null, "EXPENSE", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(323) },
                    { new Guid("a5323a77-5fa3-4f5b-be0c-f98524443e77"), "#27AE60", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(345), "briefcase", true, "Freelance", null, "INCOME", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(346) },
                    { new Guid("add7ad65-2c2a-46ee-83eb-0416f44632bf"), "#DDA0DD", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(315), "medical", true, "Healthcare", null, "EXPENSE", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(316) },
                    { new Guid("f90d138c-c9c3-47dd-bc17-25a3a9c3e63d"), "#45B7D1", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(134), "shopping-cart", true, "Shopping", null, "EXPENSE", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(135) }
                });

            migrationBuilder.InsertData(
                table: "merchants",
                columns: new[] { "id", "category_id", "created_at", "is_verified", "mcc_code", "name", "updated_at" },
                values: new object[,]
                {
                    { new Guid("0510e63d-05f6-471f-a982-2182d8925f1b"), new Guid("4c4d8137-c0db-4b12-8697-cac2646d1c2b"), new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(1234), true, "5812", "Zomato", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(1235) },
                    { new Guid("4c3f8ad3-909e-4c76-8ca9-8d0c2eb9044a"), new Guid("4c4d8137-c0db-4b12-8697-cac2646d1c2b"), new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(1216), true, "5812", "Swiggy", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(1218) },
                    { new Guid("70469cbd-6cb0-4a82-967c-5363fb371ec1"), new Guid("f90d138c-c9c3-47dd-bc17-25a3a9c3e63d"), new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(1251), true, "5399", "Amazon", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(1253) },
                    { new Guid("ae30c3f8-8362-4e30-a8d5-33c5d0d92564"), new Guid("689dc3ce-bdef-4446-8d41-9b5e061da701"), new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(1260), true, "4899", "Netflix", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(1261) },
                    { new Guid("b13bdae4-53c1-4019-b8bc-0dab1c66ce36"), new Guid("2ab3b432-71f0-4d0a-9cc7-db52f2b8be3c"), new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(1243), true, "4121", "Uber", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(1244) },
                    { new Guid("fdb1d8a8-4287-4592-9d70-4bb2d6425def"), new Guid("a089ab47-fcf9-471f-a937-57017204f471"), new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(1273), true, "4900", "BSES Delhi", new DateTime(2025, 8, 12, 19, 31, 59, 645, DateTimeKind.Utc).AddTicks(1274) }
                });
        }
    }
}
