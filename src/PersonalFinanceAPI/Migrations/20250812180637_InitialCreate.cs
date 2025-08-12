using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PersonalFinanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,")
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "bank_aggregators",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    api_endpoint = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bank_aggregators", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    is_system_defined = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.id);
                    table.ForeignKey(
                        name: "FK_categories_categories_parent_id",
                        column: x => x.parent_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: true),
                    is_email_verified = table.Column<bool>(type: "boolean", nullable: false),
                    is_phone_verified = table.Column<bool>(type: "boolean", nullable: false),
                    otp_code = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: true),
                    otp_expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_login_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "merchants",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: true),
                    mcc_code = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
                    is_verified = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_merchants", x => x.id);
                    table.ForeignKey(
                        name: "FK_merchants_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    entity_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    entity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    old_values = table.Column<string>(type: "jsonb", nullable: true),
                    new_values = table.Column<string>(type: "jsonb", nullable: true),
                    ip_address = table.Column<string>(type: "text", nullable: true),
                    user_agent = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_logs", x => x.id);
                    table.ForeignKey(
                        name: "FK_audit_logs_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "budgets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    budget_amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    period_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    current_spent = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_budgets", x => x.id);
                    table.ForeignKey(
                        name: "FK_budgets_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_budgets_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "financial_goals",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    goal_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    goal_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    target_amount = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false),
                    current_amount = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false),
                    target_date = table.Column<DateOnly>(type: "date", nullable: true),
                    priority_level = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_financial_goals", x => x.id);
                    table.ForeignKey(
                        name: "FK_financial_goals_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "income_plans",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    target_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    current_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    plan_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_income_plans", x => x.id);
                    table.ForeignKey(
                        name: "FK_income_plans_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "investment_suggestions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    suggestion_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    investment_product = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    suggested_amount = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false),
                    expected_return_rate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    risk_level = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    reasoning = table.Column<string>(type: "text", nullable: true),
                    priority_score = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_investment_suggestions", x => x.id);
                    table.ForeignKey(
                        name: "FK_investment_suggestions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "linked_accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bank_aggregator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    account_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    account_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    account_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    bank_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    balance = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false),
                    currency_code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    last_synced_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    external_account_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    consent_given_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    consent_expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_linked_accounts", x => x.id);
                    table.ForeignKey(
                        name: "FK_linked_accounts_bank_aggregators_bank_aggregator_id",
                        column: x => x.bank_aggregator_id,
                        principalTable: "bank_aggregators",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_linked_accounts_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "monthly_summaries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    month_year = table.Column<DateOnly>(type: "date", nullable: false),
                    total_income = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false),
                    total_expenses = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false),
                    net_savings = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false),
                    top_expense_category_id = table.Column<Guid>(type: "uuid", nullable: true),
                    top_expense_amount = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false),
                    transaction_count = table.Column<int>(type: "integer", nullable: false),
                    average_transaction_amount = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false),
                    savings_rate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    is_final = table.Column<bool>(type: "boolean", nullable: false),
                    computed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_monthly_summaries", x => x.id);
                    table.ForeignKey(
                        name: "FK_monthly_summaries_categories_top_expense_category_id",
                        column: x => x.top_expense_category_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_monthly_summaries_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_alerts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    alert_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    severity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false),
                    is_actionable = table.Column<bool>(type: "boolean", nullable: false),
                    action_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    metadata = table.Column<string>(type: "jsonb", nullable: true),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    acknowledged_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_alerts", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_alerts_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_preferences",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    expense_threshold = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    savings_goal_monthly = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    investment_risk_level = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    notification_email = table.Column<bool>(type: "boolean", nullable: false),
                    notification_sms = table.Column<bool>(type: "boolean", nullable: false),
                    notification_push = table.Column<bool>(type: "boolean", nullable: false),
                    currency_code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    timezone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_preferences", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_preferences_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_sessions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    refresh_token = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    access_token_jti = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ip_address = table.Column<string>(type: "text", nullable: true),
                    user_agent = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_sessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_sessions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "income_plan_milestones",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    income_plan_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    target_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    target_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_achieved = table.Column<bool>(type: "boolean", nullable: false),
                    achieved_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_income_plan_milestones", x => x.id);
                    table.ForeignKey(
                        name: "FK_income_plan_milestones_income_plans_income_plan_id",
                        column: x => x.income_plan_id,
                        principalTable: "income_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "income_sources",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    income_plan_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    source_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    expected_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    actual_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    frequency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_income_sources", x => x.id);
                    table.ForeignKey(
                        name: "FK_income_sources_income_plans_income_plan_id",
                        column: x => x.income_plan_id,
                        principalTable: "income_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "suggestion_history",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    suggestion_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    action = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    action_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suggestion_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_suggestion_history_investment_suggestions_suggestion_id",
                        column: x => x.suggestion_id,
                        principalTable: "investment_suggestions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_suggestion_history_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    linked_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    merchant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    category_id = table.Column<Guid>(type: "uuid", nullable: true),
                    amount = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false),
                    currency_code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    transaction_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    reference_number = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    transaction_date = table.Column<DateOnly>(type: "date", nullable: false),
                    posted_date = table.Column<DateOnly>(type: "date", nullable: true),
                    is_recurring = table.Column<bool>(type: "boolean", nullable: false),
                    recurring_frequency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    is_categorized_manually = table.Column<bool>(type: "boolean", nullable: false),
                    is_transfer = table.Column<bool>(type: "boolean", nullable: false),
                    transfer_to_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    external_transaction_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    raw_data = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transactions", x => x.id);
                    table.ForeignKey(
                        name: "FK_transactions_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_transactions_linked_accounts_linked_account_id",
                        column: x => x.linked_account_id,
                        principalTable: "linked_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_transactions_linked_accounts_transfer_to_account_id",
                        column: x => x.transfer_to_account_id,
                        principalTable: "linked_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_transactions_merchants_merchant_id",
                        column: x => x.merchant_id,
                        principalTable: "merchants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_transactions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "income_entries",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    income_source_id = table.Column<int>(type: "integer", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    received_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    reference_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_income_entries", x => x.id);
                    table.ForeignKey(
                        name: "FK_income_entries_income_sources_income_source_id",
                        column: x => x.income_source_id,
                        principalTable: "income_sources",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "budget_utilizations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    budget_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount_utilized = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    utilization_date = table.Column<DateOnly>(type: "date", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_budget_utilizations", x => x.id);
                    table.ForeignKey(
                        name: "FK_budget_utilizations_budgets_budget_id",
                        column: x => x.budget_id,
                        principalTable: "budgets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_budget_utilizations_transactions_transaction_id",
                        column: x => x.transaction_id,
                        principalTable: "transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transaction_flags",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    flag_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    flag_value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transaction_flags", x => x.id);
                    table.ForeignKey(
                        name: "FK_transaction_flags_transactions_transaction_id",
                        column: x => x.transaction_id,
                        principalTable: "transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transaction_flags_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_user_id",
                table: "audit_logs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_budget_utilizations_budget_id",
                table: "budget_utilizations",
                column: "budget_id");

            migrationBuilder.CreateIndex(
                name: "IX_budget_utilizations_transaction_id",
                table: "budget_utilizations",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "IX_budgets_category_id",
                table: "budgets",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_budgets_user_id",
                table: "budgets",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_categories_parent_id",
                table: "categories",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_financial_goals_user_id",
                table: "financial_goals",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_income_entries_income_source_id",
                table: "income_entries",
                column: "income_source_id");

            migrationBuilder.CreateIndex(
                name: "IX_income_plan_milestones_income_plan_id",
                table: "income_plan_milestones",
                column: "income_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_income_plans_user_id",
                table: "income_plans",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_income_sources_income_plan_id",
                table: "income_sources",
                column: "income_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_investment_suggestions_user_id",
                table: "investment_suggestions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_linked_accounts_bank_aggregator_id",
                table: "linked_accounts",
                column: "bank_aggregator_id");

            migrationBuilder.CreateIndex(
                name: "IX_linked_accounts_user_id",
                table: "linked_accounts",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_merchants_category_id",
                table: "merchants",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_monthly_summaries_top_expense_category_id",
                table: "monthly_summaries",
                column: "top_expense_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_monthly_summaries_user_id_month_year",
                table: "monthly_summaries",
                columns: new[] { "user_id", "month_year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_suggestion_history_suggestion_id",
                table: "suggestion_history",
                column: "suggestion_id");

            migrationBuilder.CreateIndex(
                name: "IX_suggestion_history_user_id",
                table: "suggestion_history",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_transaction_flags_created_by",
                table: "transaction_flags",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_transaction_flags_transaction_id",
                table: "transaction_flags",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_category_id",
                table: "transactions",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_linked_account_id",
                table: "transactions",
                column: "linked_account_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_merchant_id",
                table: "transactions",
                column: "merchant_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_transfer_to_account_id",
                table: "transactions",
                column: "transfer_to_account_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_user_id",
                table: "transactions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_alerts_user_id",
                table: "user_alerts",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_preferences_user_id",
                table: "user_preferences",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_sessions_refresh_token",
                table: "user_sessions",
                column: "refresh_token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_sessions_user_id",
                table: "user_sessions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "budget_utilizations");

            migrationBuilder.DropTable(
                name: "financial_goals");

            migrationBuilder.DropTable(
                name: "income_entries");

            migrationBuilder.DropTable(
                name: "income_plan_milestones");

            migrationBuilder.DropTable(
                name: "monthly_summaries");

            migrationBuilder.DropTable(
                name: "suggestion_history");

            migrationBuilder.DropTable(
                name: "transaction_flags");

            migrationBuilder.DropTable(
                name: "user_alerts");

            migrationBuilder.DropTable(
                name: "user_preferences");

            migrationBuilder.DropTable(
                name: "user_sessions");

            migrationBuilder.DropTable(
                name: "budgets");

            migrationBuilder.DropTable(
                name: "income_sources");

            migrationBuilder.DropTable(
                name: "investment_suggestions");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "income_plans");

            migrationBuilder.DropTable(
                name: "linked_accounts");

            migrationBuilder.DropTable(
                name: "merchants");

            migrationBuilder.DropTable(
                name: "bank_aggregators");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "categories");
        }
    }
}
