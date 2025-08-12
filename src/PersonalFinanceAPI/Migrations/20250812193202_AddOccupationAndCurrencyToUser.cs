using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PersonalFinanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddOccupationAndCurrencyToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "bank_aggregators",
                keyColumn: "id",
                keyValue: new Guid("8e8935cc-e39e-4963-ac93-55e32348950f"));

            migrationBuilder.DeleteData(
                table: "bank_aggregators",
                keyColumn: "id",
                keyValue: new Guid("b417ef8f-3b32-490b-b5b7-dde631281fdd"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("0b1aefd3-12bb-4448-80d2-6d028c9acb96"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("0bebcbea-4604-4043-94c7-9c3409ba0fae"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("132558ca-524b-4651-a713-d7a66eed7bc0"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("4f303149-3f38-4e06-bc4e-2aafa06ecc82"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("87830e53-fc44-48c4-83e0-8f4a6cbec191"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("9d815638-7255-4cd0-a69a-fc8b20dc9122"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("bc60bbc9-4bfd-4b97-ac47-9a0d80c13435"));

            migrationBuilder.DeleteData(
                table: "merchants",
                keyColumn: "id",
                keyValue: new Guid("59cca9e3-c9f5-4ca6-86ba-8daa7fc73947"));

            migrationBuilder.DeleteData(
                table: "merchants",
                keyColumn: "id",
                keyValue: new Guid("76a5ad7c-8f0a-45c7-ba37-8fdb6088235e"));

            migrationBuilder.DeleteData(
                table: "merchants",
                keyColumn: "id",
                keyValue: new Guid("83058c45-b348-4147-afb9-846e6d62e985"));

            migrationBuilder.DeleteData(
                table: "merchants",
                keyColumn: "id",
                keyValue: new Guid("97f9f306-146d-4cb0-b641-83266853e87e"));

            migrationBuilder.DeleteData(
                table: "merchants",
                keyColumn: "id",
                keyValue: new Guid("ee5923b4-5011-476d-baed-82a60902fb0b"));

            migrationBuilder.DeleteData(
                table: "merchants",
                keyColumn: "id",
                keyValue: new Guid("fe89ab0a-aede-49ae-add4-9b50c03119f5"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("19981257-c439-412b-8ed0-e143ede8ecdf"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("75a0cd3d-940c-47b6-96b2-ae0e2b4f76c5"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("c6806108-5660-4389-821a-6599e7393d7b"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("d2c37002-7af0-4d8c-8f2c-c962ed2bbd07"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("eb90285e-9e4b-4658-8e48-ad8112f076d1"));

            migrationBuilder.AddColumn<string>(
                name: "currency",
                table: "users",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "occupation",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "currency",
                table: "users");

            migrationBuilder.DropColumn(
                name: "occupation",
                table: "users");

            migrationBuilder.InsertData(
                table: "bank_aggregators",
                columns: new[] { "id", "api_endpoint", "created_at", "is_active", "name", "updated_at" },
                values: new object[,]
                {
                    { new Guid("8e8935cc-e39e-4963-ac93-55e32348950f"), "https://api.demo-bank.com", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1414), true, "Demo Bank Connector", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1415) },
                    { new Guid("b417ef8f-3b32-490b-b5b7-dde631281fdd"), "https://api.openbanking-sim.com", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1419), true, "Simulated Open Banking", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1420) }
                });

            migrationBuilder.InsertData(
                table: "categories",
                columns: new[] { "id", "color", "created_at", "icon", "is_system_defined", "name", "parent_id", "type", "updated_at" },
                values: new object[,]
                {
                    { new Guid("0b1aefd3-12bb-4448-80d2-6d028c9acb96"), "#27AE60", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1053), "briefcase", true, "Freelance", null, "INCOME", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1054) },
                    { new Guid("0bebcbea-4604-4043-94c7-9c3409ba0fae"), "#F7DC6F", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1043), "plane", true, "Travel", null, "EXPENSE", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1044) },
                    { new Guid("132558ca-524b-4651-a713-d7a66eed7bc0"), "#2ECC71", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1048), "dollar-sign", true, "Salary", null, "INCOME", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1049) },
                    { new Guid("19981257-c439-412b-8ed0-e143ede8ecdf"), "#4ECDC4", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(894), "car", true, "Transportation", null, "EXPENSE", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(895) },
                    { new Guid("4f303149-3f38-4e06-bc4e-2aafa06ecc82"), "#95A5A6", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1068), "exchange", true, "Transfer", null, "TRANSFER", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1069) },
                    { new Guid("75a0cd3d-940c-47b6-96b2-ae0e2b4f76c5"), "#FF6B6B", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(888), "restaurant", true, "Food & Dining", null, "EXPENSE", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(889) },
                    { new Guid("87830e53-fc44-48c4-83e0-8f4a6cbec191"), "#DDA0DD", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1024), "medical", true, "Healthcare", null, "EXPENSE", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1024) },
                    { new Guid("9d815638-7255-4cd0-a69a-fc8b20dc9122"), "#98D8C8", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1029), "graduation-cap", true, "Education", null, "EXPENSE", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1030) },
                    { new Guid("bc60bbc9-4bfd-4b97-ac47-9a0d80c13435"), "#16A085", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1063), "trending-up", true, "Investment Returns", null, "INCOME", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1064) },
                    { new Guid("c6806108-5660-4389-821a-6599e7393d7b"), "#96CEB4", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(911), "movie", true, "Entertainment", null, "EXPENSE", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(912) },
                    { new Guid("d2c37002-7af0-4d8c-8f2c-c962ed2bbd07"), "#FFEAA7", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(917), "receipt", true, "Bills & Utilities", null, "EXPENSE", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(917) },
                    { new Guid("eb90285e-9e4b-4658-8e48-ad8112f076d1"), "#45B7D1", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(906), "shopping-cart", true, "Shopping", null, "EXPENSE", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(907) }
                });

            migrationBuilder.InsertData(
                table: "merchants",
                columns: new[] { "id", "category_id", "created_at", "is_verified", "mcc_code", "name", "updated_at" },
                values: new object[,]
                {
                    { new Guid("59cca9e3-c9f5-4ca6-86ba-8daa7fc73947"), new Guid("75a0cd3d-940c-47b6-96b2-ae0e2b4f76c5"), new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1504), true, "5812", "Zomato", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1505) },
                    { new Guid("76a5ad7c-8f0a-45c7-ba37-8fdb6088235e"), new Guid("19981257-c439-412b-8ed0-e143ede8ecdf"), new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1510), true, "4121", "Uber", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1511) },
                    { new Guid("83058c45-b348-4147-afb9-846e6d62e985"), new Guid("c6806108-5660-4389-821a-6599e7393d7b"), new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1521), true, "4899", "Netflix", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1522) },
                    { new Guid("97f9f306-146d-4cb0-b641-83266853e87e"), new Guid("75a0cd3d-940c-47b6-96b2-ae0e2b4f76c5"), new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1492), true, "5812", "Swiggy", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1492) },
                    { new Guid("ee5923b4-5011-476d-baed-82a60902fb0b"), new Guid("eb90285e-9e4b-4658-8e48-ad8112f076d1"), new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1516), true, "5399", "Amazon", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1516) },
                    { new Guid("fe89ab0a-aede-49ae-add4-9b50c03119f5"), new Guid("d2c37002-7af0-4d8c-8f2c-c962ed2bbd07"), new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1530), true, "4900", "BSES Delhi", new DateTime(2025, 8, 12, 18, 6, 36, 702, DateTimeKind.Utc).AddTicks(1531) }
                });
        }
    }
}
