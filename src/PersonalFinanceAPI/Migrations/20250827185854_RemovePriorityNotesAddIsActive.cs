using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PersonalFinanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemovePriorityNotesAddIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "notes",
                table: "income_plans");

            migrationBuilder.DropColumn(
                name: "priority",
                table: "income_plans");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "income_plans",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.InsertData(
                table: "bank_aggregators",
                columns: new[] { "id", "api_endpoint", "created_at", "is_active", "name", "updated_at" },
                values: new object[,]
                {
                    { new Guid("7aa95763-a949-4a4d-a424-a08f1fb29ed8"), "https://api.openbanking-sim.com", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(9343), true, "Simulated Open Banking", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(9345) },
                    { new Guid("e0af082c-bf9a-4df5-bf21-e2e4efb7fffe"), "https://api.demo-bank.com", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(9247), true, "Demo Bank Connector", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(9249) }
                });

            migrationBuilder.InsertData(
                table: "categories",
                columns: new[] { "id", "color", "created_at", "icon", "is_system_defined", "name", "parent_id", "type", "updated_at" },
                values: new object[,]
                {
                    { new Guid("033ec4c4-9f02-4685-b6cc-98351f314a46"), "#FF6B6B", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(8435), "restaurant", true, "Food & Dining", null, "EXPENSE", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(8437) },
                    { new Guid("05c787a1-17aa-4e69-9e46-608e129b1853"), "#95A5A6", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(8542), "exchange", true, "Transfer", null, "TRANSFER", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(8543) },
                    { new Guid("3056e4a4-c858-4cb0-af01-a7f645a2514a"), "#F7DC6F", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(8509), "plane", true, "Travel", null, "EXPENSE", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(8510) },
                    { new Guid("45c6a13f-5dfd-4b58-9b6c-eaf418ff1dde"), "#2ECC71", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(8517), "dollar-sign", true, "Salary", null, "INCOME", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(8518) },
                    { new Guid("863d73ad-1f51-48d4-b82b-f3a2f0e1e4f8"), "#DDA0DD", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(8483), "medical", true, "Healthcare", null, "EXPENSE", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(8484) },
                    { new Guid("a4fd4890-eedd-4055-a727-bbf13f544726"), "#16A085", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(8534), "trending-up", true, "Investment Returns", null, "INCOME", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(8535) },
                    { new Guid("af1d1e46-4f83-4c65-b295-8a1182fae494"), "#27AE60", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(8525), "briefcase", true, "Freelance", null, "INCOME", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(8527) },
                    { new Guid("b7716fdd-11a8-4647-85fc-7dc9220ab95d"), "#98D8C8", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(8500), "graduation-cap", true, "Education", null, "EXPENSE", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(8501) },
                    { new Guid("c85348e4-d718-4b24-90ef-80daf7a205d8"), "#45B7D1", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(8455), "shopping-cart", true, "Shopping", null, "EXPENSE", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(8457) },
                    { new Guid("e572cb80-a489-4041-8639-850b293b997d"), "#96CEB4", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(8465), "movie", true, "Entertainment", null, "EXPENSE", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(8466) },
                    { new Guid("f7faf112-dd62-4688-870b-433a29c04fa2"), "#FFEAA7", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(8474), "receipt", true, "Bills & Utilities", null, "EXPENSE", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(8475) },
                    { new Guid("fbcd73cf-db49-4ef4-81ce-46a01b8e16d5"), "#4ECDC4", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(8446), "car", true, "Transportation", null, "EXPENSE", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(8448) }
                });

            migrationBuilder.InsertData(
                table: "merchants",
                columns: new[] { "id", "category_id", "created_at", "is_verified", "mcc_code", "name", "updated_at" },
                values: new object[,]
                {
                    { new Guid("834c46d6-fcbf-44b7-bff8-dd6e344205b2"), new Guid("fbcd73cf-db49-4ef4-81ce-46a01b8e16d5"), new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(9488), true, "4121", "Uber", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(9489) },
                    { new Guid("b22225bb-32ef-44b0-8b8e-384147690dfb"), new Guid("f7faf112-dd62-4688-870b-433a29c04fa2"), new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(9524), true, "4900", "BSES Delhi", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(9525) },
                    { new Guid("c54934e5-e0b7-40d4-bcb6-183c8dcc69d1"), new Guid("c85348e4-d718-4b24-90ef-80daf7a205d8"), new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(9505), true, "5399", "Amazon", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(9506) },
                    { new Guid("e6f0da85-d7ec-4a37-bd41-684f24dcc391"), new Guid("033ec4c4-9f02-4685-b6cc-98351f314a46"), new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(9461), true, "5812", "Swiggy", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(9462) },
                    { new Guid("f2ddcd4e-bbd5-4a13-8f57-893ad4d05716"), new Guid("e572cb80-a489-4041-8639-850b293b997d"), new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(9515), true, "4899", "Netflix", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(9516) },
                    { new Guid("f3308749-5997-474b-b56e-9508fb109325"), new Guid("033ec4c4-9f02-4685-b6cc-98351f314a46"), new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(9471), true, "5812", "Zomato", new DateTime(2025, 8, 27, 18, 58, 50, 285, DateTimeKind.Utc).AddTicks(9472) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "bank_aggregators",
                keyColumn: "id",
                keyValue: new Guid("7aa95763-a949-4a4d-a424-a08f1fb29ed8"));

            migrationBuilder.DeleteData(
                table: "bank_aggregators",
                keyColumn: "id",
                keyValue: new Guid("e0af082c-bf9a-4df5-bf21-e2e4efb7fffe"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("05c787a1-17aa-4e69-9e46-608e129b1853"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("3056e4a4-c858-4cb0-af01-a7f645a2514a"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("45c6a13f-5dfd-4b58-9b6c-eaf418ff1dde"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("863d73ad-1f51-48d4-b82b-f3a2f0e1e4f8"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("a4fd4890-eedd-4055-a727-bbf13f544726"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("af1d1e46-4f83-4c65-b295-8a1182fae494"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("b7716fdd-11a8-4647-85fc-7dc9220ab95d"));

            migrationBuilder.DeleteData(
                table: "merchants",
                keyColumn: "id",
                keyValue: new Guid("834c46d6-fcbf-44b7-bff8-dd6e344205b2"));

            migrationBuilder.DeleteData(
                table: "merchants",
                keyColumn: "id",
                keyValue: new Guid("b22225bb-32ef-44b0-8b8e-384147690dfb"));

            migrationBuilder.DeleteData(
                table: "merchants",
                keyColumn: "id",
                keyValue: new Guid("c54934e5-e0b7-40d4-bcb6-183c8dcc69d1"));

            migrationBuilder.DeleteData(
                table: "merchants",
                keyColumn: "id",
                keyValue: new Guid("e6f0da85-d7ec-4a37-bd41-684f24dcc391"));

            migrationBuilder.DeleteData(
                table: "merchants",
                keyColumn: "id",
                keyValue: new Guid("f2ddcd4e-bbd5-4a13-8f57-893ad4d05716"));

            migrationBuilder.DeleteData(
                table: "merchants",
                keyColumn: "id",
                keyValue: new Guid("f3308749-5997-474b-b56e-9508fb109325"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("033ec4c4-9f02-4685-b6cc-98351f314a46"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("c85348e4-d718-4b24-90ef-80daf7a205d8"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("e572cb80-a489-4041-8639-850b293b997d"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("f7faf112-dd62-4688-870b-433a29c04fa2"));

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("fbcd73cf-db49-4ef4-81ce-46a01b8e16d5"));

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "income_plans");

            migrationBuilder.AddColumn<string>(
                name: "notes",
                table: "income_plans",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "priority",
                table: "income_plans",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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
    }
}
