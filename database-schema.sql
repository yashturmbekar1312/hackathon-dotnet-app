-- Personal Finance Management API Database Schema
-- PostgreSQL 14+ Compatible

-- Enable necessary extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Drop existing tables if they exist (for development)
DROP TABLE IF EXISTS audit_logs CASCADE;
DROP TABLE IF EXISTS user_alerts CASCADE;
DROP TABLE IF EXISTS suggestion_history CASCADE;
DROP TABLE IF EXISTS investment_suggestions CASCADE;
DROP TABLE IF EXISTS financial_goals CASCADE;
DROP TABLE IF EXISTS monthly_summaries CASCADE;
DROP TABLE IF EXISTS budget_utilizations CASCADE;
DROP TABLE IF EXISTS budgets CASCADE;
DROP TABLE IF EXISTS transaction_flags CASCADE;
DROP TABLE IF EXISTS transactions CASCADE;
DROP TABLE IF EXISTS linked_accounts CASCADE;
DROP TABLE IF EXISTS bank_aggregators CASCADE;
DROP TABLE IF EXISTS merchants CASCADE;
DROP TABLE IF EXISTS categories CASCADE;
DROP TABLE IF EXISTS user_sessions CASCADE;
DROP TABLE IF EXISTS user_preferences CASCADE;
DROP TABLE IF EXISTS users CASCADE;

-- Core Tables

-- Users table
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    phone_number VARCHAR(20),
    date_of_birth DATE,
    is_email_verified BOOLEAN DEFAULT FALSE,
    is_phone_verified BOOLEAN DEFAULT FALSE,
    otp_code VARCHAR(6),
    otp_expires_at TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    last_login_at TIMESTAMP WITH TIME ZONE,
    is_active BOOLEAN DEFAULT TRUE
);

-- User preferences
CREATE TABLE user_preferences (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    expense_threshold DECIMAL(12,2) DEFAULT 1000.00,
    savings_goal_monthly DECIMAL(12,2) DEFAULT 5000.00,
    investment_risk_level VARCHAR(20) DEFAULT 'MODERATE' CHECK (investment_risk_level IN ('LOW', 'MODERATE', 'HIGH')),
    notification_email BOOLEAN DEFAULT TRUE,
    notification_sms BOOLEAN DEFAULT FALSE,
    notification_push BOOLEAN DEFAULT TRUE,
    currency_code VARCHAR(3) DEFAULT 'INR',
    timezone VARCHAR(50) DEFAULT 'Asia/Kolkata',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- User sessions (for JWT refresh tokens)
CREATE TABLE user_sessions (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    refresh_token VARCHAR(512) NOT NULL,
    access_token_jti VARCHAR(255) NOT NULL,
    expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    revoked_at TIMESTAMP WITH TIME ZONE,
    ip_address INET,
    user_agent TEXT
);

-- Categories for transaction classification
CREATE TABLE categories (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(100) NOT NULL,
    parent_id UUID REFERENCES categories(id),
    type VARCHAR(20) NOT NULL CHECK (type IN ('INCOME', 'EXPENSE', 'TRANSFER')),
    icon VARCHAR(50),
    color VARCHAR(7),
    is_system_defined BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Merchants
CREATE TABLE merchants (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(255) NOT NULL,
    category_id UUID REFERENCES categories(id),
    mcc_code VARCHAR(4), -- Merchant Category Code
    is_verified BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Bank aggregators (Plaid, Yodlee, etc.)
CREATE TABLE bank_aggregators (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(100) NOT NULL,
    api_endpoint VARCHAR(255),
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Linked bank accounts
CREATE TABLE linked_accounts (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    bank_aggregator_id UUID REFERENCES bank_aggregators(id),
    account_name VARCHAR(255) NOT NULL,
    account_number VARCHAR(50),
    account_type VARCHAR(20) NOT NULL CHECK (account_type IN ('SAVINGS', 'CURRENT', 'CREDIT_CARD', 'INVESTMENT')),
    bank_name VARCHAR(255) NOT NULL,
    balance DECIMAL(15,2) DEFAULT 0.00,
    currency_code VARCHAR(3) DEFAULT 'INR',
    is_active BOOLEAN DEFAULT TRUE,
    last_synced_at TIMESTAMP WITH TIME ZONE,
    external_account_id VARCHAR(255),
    consent_given_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    consent_expires_at TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Transactions table (partitioned by month for performance)
CREATE TABLE transactions (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    linked_account_id UUID NOT NULL REFERENCES linked_accounts(id),
    merchant_id UUID REFERENCES merchants(id),
    category_id UUID REFERENCES categories(id),
    amount DECIMAL(15,2) NOT NULL,
    currency_code VARCHAR(3) DEFAULT 'INR',
    transaction_type VARCHAR(20) NOT NULL CHECK (transaction_type IN ('DEBIT', 'CREDIT')),
    description TEXT,
    reference_number VARCHAR(255),
    transaction_date DATE NOT NULL,
    posted_date DATE,
    is_recurring BOOLEAN DEFAULT FALSE,
    recurring_frequency VARCHAR(20) CHECK (recurring_frequency IN ('DAILY', 'WEEKLY', 'MONTHLY', 'QUARTERLY', 'YEARLY')),
    is_categorized_manually BOOLEAN DEFAULT FALSE,
    is_transfer BOOLEAN DEFAULT FALSE,
    transfer_to_account_id UUID REFERENCES linked_accounts(id),
    external_transaction_id VARCHAR(255),
    raw_data JSONB,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
) PARTITION BY RANGE (transaction_date);

-- Create partitions for transactions (current year and next year)
CREATE TABLE transactions_2024 PARTITION OF transactions
    FOR VALUES FROM ('2024-01-01') TO ('2025-01-01');

CREATE TABLE transactions_2025 PARTITION OF transactions
    FOR VALUES FROM ('2025-01-01') TO ('2026-01-01');

CREATE TABLE transactions_2026 PARTITION OF transactions
    FOR VALUES FROM ('2026-01-01') TO ('2027-01-01');

-- Transaction flags for special handling
CREATE TABLE transaction_flags (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    transaction_id UUID NOT NULL REFERENCES transactions(id) ON DELETE CASCADE,
    flag_type VARCHAR(50) NOT NULL,
    flag_value VARCHAR(255),
    created_by UUID REFERENCES users(id),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Budgets
CREATE TABLE budgets (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    category_id UUID NOT NULL REFERENCES categories(id),
    budget_amount DECIMAL(12,2) NOT NULL,
    period_type VARCHAR(20) NOT NULL DEFAULT 'MONTHLY' CHECK (period_type IN ('WEEKLY', 'MONTHLY', 'QUARTERLY', 'YEARLY')),
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    current_spent DECIMAL(12,2) DEFAULT 0.00,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Budget utilization tracking
CREATE TABLE budget_utilizations (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    budget_id UUID NOT NULL REFERENCES budgets(id) ON DELETE CASCADE,
    transaction_id UUID NOT NULL REFERENCES transactions(id),
    amount_utilized DECIMAL(12,2) NOT NULL,
    utilization_date DATE NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Monthly summaries for analytics
CREATE TABLE monthly_summaries (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    month_year DATE NOT NULL, -- First day of the month
    total_income DECIMAL(15,2) DEFAULT 0.00,
    total_expenses DECIMAL(15,2) DEFAULT 0.00,
    net_savings DECIMAL(15,2) DEFAULT 0.00,
    top_expense_category_id UUID REFERENCES categories(id),
    top_expense_amount DECIMAL(15,2) DEFAULT 0.00,
    transaction_count INTEGER DEFAULT 0,
    average_transaction_amount DECIMAL(15,2) DEFAULT 0.00,
    savings_rate DECIMAL(5,2) DEFAULT 0.00, -- Percentage
    is_final BOOLEAN DEFAULT FALSE,
    computed_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    UNIQUE(user_id, month_year)
);

-- Financial goals
CREATE TABLE financial_goals (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    goal_name VARCHAR(255) NOT NULL,
    goal_type VARCHAR(50) NOT NULL CHECK (goal_type IN ('SAVINGS', 'INVESTMENT', 'DEBT_PAYOFF', 'EMERGENCY_FUND')),
    target_amount DECIMAL(15,2) NOT NULL,
    current_amount DECIMAL(15,2) DEFAULT 0.00,
    target_date DATE,
    priority_level INTEGER DEFAULT 3 CHECK (priority_level BETWEEN 1 AND 5),
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Investment suggestions
CREATE TABLE investment_suggestions (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    suggestion_type VARCHAR(50) NOT NULL,
    investment_product VARCHAR(255) NOT NULL,
    suggested_amount DECIMAL(15,2) NOT NULL,
    expected_return_rate DECIMAL(5,2),
    risk_level VARCHAR(20) CHECK (risk_level IN ('LOW', 'MODERATE', 'HIGH')),
    reasoning TEXT,
    priority_score INTEGER DEFAULT 50,
    is_active BOOLEAN DEFAULT TRUE,
    expires_at TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Suggestion interaction history
CREATE TABLE suggestion_history (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    suggestion_id UUID NOT NULL REFERENCES investment_suggestions(id),
    user_id UUID NOT NULL REFERENCES users(id),
    action VARCHAR(20) NOT NULL CHECK (action IN ('VIEWED', 'ACCEPTED', 'REJECTED', 'POSTPONED')),
    action_date TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    notes TEXT
);

-- User alerts
CREATE TABLE user_alerts (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    alert_type VARCHAR(50) NOT NULL,
    title VARCHAR(255) NOT NULL,
    message TEXT NOT NULL,
    severity VARCHAR(20) DEFAULT 'INFO' CHECK (severity IN ('INFO', 'WARNING', 'CRITICAL')),
    is_read BOOLEAN DEFAULT FALSE,
    is_actionable BOOLEAN DEFAULT FALSE,
    action_url VARCHAR(255),
    metadata JSONB,
    expires_at TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    acknowledged_at TIMESTAMP WITH TIME ZONE
);

-- Audit logs for compliance
CREATE TABLE audit_logs (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES users(id),
    entity_type VARCHAR(100) NOT NULL,
    entity_id UUID,
    action VARCHAR(50) NOT NULL,
    old_values JSONB,
    new_values JSONB,
    ip_address INET,
    user_agent TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Indexes for performance optimization

-- Users indexes
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_is_active ON users(is_active);
CREATE INDEX idx_users_created_at ON users(created_at);

-- Transactions indexes
CREATE INDEX idx_transactions_user_id ON transactions(user_id);
CREATE INDEX idx_transactions_date ON transactions(transaction_date);
CREATE INDEX idx_transactions_account_id ON transactions(linked_account_id);
CREATE INDEX idx_transactions_category_id ON transactions(category_id);
CREATE INDEX idx_transactions_amount ON transactions(amount);
CREATE INDEX idx_transactions_type ON transactions(transaction_type);
CREATE INDEX idx_transactions_recurring ON transactions(is_recurring);
CREATE INDEX idx_transactions_user_date ON transactions(user_id, transaction_date);

-- Linked accounts indexes
CREATE INDEX idx_linked_accounts_user_id ON linked_accounts(user_id);
CREATE INDEX idx_linked_accounts_active ON linked_accounts(is_active);

-- Budgets indexes
CREATE INDEX idx_budgets_user_id ON budgets(user_id);
CREATE INDEX idx_budgets_category_id ON budgets(category_id);
CREATE INDEX idx_budgets_active ON budgets(is_active);
CREATE INDEX idx_budgets_period ON budgets(start_date, end_date);

-- Monthly summaries indexes
CREATE INDEX idx_monthly_summaries_user_month ON monthly_summaries(user_id, month_year);
CREATE INDEX idx_monthly_summaries_month ON monthly_summaries(month_year);

-- Alerts indexes
CREATE INDEX idx_user_alerts_user_id ON user_alerts(user_id);
CREATE INDEX idx_user_alerts_unread ON user_alerts(user_id, is_read);
CREATE INDEX idx_user_alerts_created ON user_alerts(created_at);

-- Session indexes
CREATE INDEX idx_user_sessions_user_id ON user_sessions(user_id);
CREATE INDEX idx_user_sessions_refresh_token ON user_sessions(refresh_token);
CREATE INDEX idx_user_sessions_expires ON user_sessions(expires_at);

-- Audit logs indexes
CREATE INDEX idx_audit_logs_user_id ON audit_logs(user_id);
CREATE INDEX idx_audit_logs_entity ON audit_logs(entity_type, entity_id);
CREATE INDEX idx_audit_logs_created ON audit_logs(created_at);

-- Functions and triggers for automatic timestamp updates
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Apply update timestamp triggers
CREATE TRIGGER update_users_updated_at BEFORE UPDATE ON users
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_user_preferences_updated_at BEFORE UPDATE ON user_preferences
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_categories_updated_at BEFORE UPDATE ON categories
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_merchants_updated_at BEFORE UPDATE ON merchants
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_linked_accounts_updated_at BEFORE UPDATE ON linked_accounts
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_transactions_updated_at BEFORE UPDATE ON transactions
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_budgets_updated_at BEFORE UPDATE ON budgets
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_monthly_summaries_updated_at BEFORE UPDATE ON monthly_summaries
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_financial_goals_updated_at BEFORE UPDATE ON financial_goals
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_investment_suggestions_updated_at BEFORE UPDATE ON investment_suggestions
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- Insert default categories
INSERT INTO categories (id, name, type, icon, color, is_system_defined) VALUES
    (uuid_generate_v4(), 'Food & Dining', 'EXPENSE', 'restaurant', '#FF6B6B', TRUE),
    (uuid_generate_v4(), 'Transportation', 'EXPENSE', 'car', '#4ECDC4', TRUE),
    (uuid_generate_v4(), 'Shopping', 'EXPENSE', 'shopping-cart', '#45B7D1', TRUE),
    (uuid_generate_v4(), 'Entertainment', 'EXPENSE', 'movie', '#96CEB4', TRUE),
    (uuid_generate_v4(), 'Bills & Utilities', 'EXPENSE', 'receipt', '#FFEAA7', TRUE),
    (uuid_generate_v4(), 'Healthcare', 'EXPENSE', 'medical', '#DDA0DD', TRUE),
    (uuid_generate_v4(), 'Education', 'EXPENSE', 'graduation-cap', '#98D8C8', TRUE),
    (uuid_generate_v4(), 'Travel', 'EXPENSE', 'plane', '#F7DC6F', TRUE),
    (uuid_generate_v4(), 'Salary', 'INCOME', 'dollar-sign', '#2ECC71', TRUE),
    (uuid_generate_v4(), 'Freelance', 'INCOME', 'briefcase', '#27AE60', TRUE),
    (uuid_generate_v4(), 'Investment Returns', 'INCOME', 'trending-up', '#16A085', TRUE),
    (uuid_generate_v4(), 'Transfer', 'TRANSFER', 'exchange', '#95A5A6', TRUE);

-- Insert default bank aggregators
INSERT INTO bank_aggregators (id, name, api_endpoint, is_active) VALUES
    (uuid_generate_v4(), 'Demo Bank Connector', 'https://api.demo-bank.com', TRUE),
    (uuid_generate_v4(), 'Simulated Open Banking', 'https://api.openbanking-sim.com', TRUE);

-- Sample merchants for demonstration
INSERT INTO merchants (id, name, category_id, mcc_code, is_verified) VALUES
    (uuid_generate_v4(), 'Swiggy', (SELECT id FROM categories WHERE name = 'Food & Dining' LIMIT 1), '5812', TRUE),
    (uuid_generate_v4(), 'Zomato', (SELECT id FROM categories WHERE name = 'Food & Dining' LIMIT 1), '5812', TRUE),
    (uuid_generate_v4(), 'Uber', (SELECT id FROM categories WHERE name = 'Transportation' LIMIT 1), '4121', TRUE),
    (uuid_generate_v4(), 'Amazon', (SELECT id FROM categories WHERE name = 'Shopping' LIMIT 1), '5399', TRUE),
    (uuid_generate_v4(), 'Netflix', (SELECT id FROM categories WHERE name = 'Entertainment' LIMIT 1), '4899', TRUE),
    (uuid_generate_v4(), 'BSES Delhi', (SELECT id FROM categories WHERE name = 'Bills & Utilities' LIMIT 1), '4900', TRUE);

-- Create views for common queries
CREATE VIEW user_transaction_summary AS
SELECT 
    u.id as user_id,
    u.email,
    COUNT(t.id) as total_transactions,
    SUM(CASE WHEN t.transaction_type = 'CREDIT' THEN t.amount ELSE 0 END) as total_income,
    SUM(CASE WHEN t.transaction_type = 'DEBIT' THEN t.amount ELSE 0 END) as total_expenses,
    SUM(CASE WHEN t.transaction_type = 'CREDIT' THEN t.amount ELSE -t.amount END) as net_balance
FROM users u
LEFT JOIN transactions t ON u.id = t.user_id
WHERE u.is_active = TRUE
GROUP BY u.id, u.email;

CREATE VIEW monthly_category_spending AS
SELECT 
    t.user_id,
    DATE_TRUNC('month', t.transaction_date) as month_year,
    c.name as category_name,
    SUM(t.amount) as total_spent,
    COUNT(t.id) as transaction_count
FROM transactions t
JOIN categories c ON t.category_id = c.id
WHERE t.transaction_type = 'DEBIT'
GROUP BY t.user_id, DATE_TRUNC('month', t.transaction_date), c.id, c.name;

-- Enable row level security for multi-tenant data isolation
ALTER TABLE transactions ENABLE ROW LEVEL SECURITY;
ALTER TABLE linked_accounts ENABLE ROW LEVEL SECURITY;
ALTER TABLE budgets ENABLE ROW LEVEL SECURITY;
ALTER TABLE user_alerts ENABLE ROW LEVEL SECURITY;
ALTER TABLE monthly_summaries ENABLE ROW LEVEL SECURITY;
ALTER TABLE financial_goals ENABLE ROW LEVEL SECURITY;
ALTER TABLE investment_suggestions ENABLE ROW LEVEL SECURITY;

-- Create RLS policies (will be activated when users are authenticated)
CREATE POLICY user_transactions_policy ON transactions
    FOR ALL TO authenticated_users
    USING (user_id = current_setting('app.current_user_id')::UUID);

CREATE POLICY user_accounts_policy ON linked_accounts
    FOR ALL TO authenticated_users
    USING (user_id = current_setting('app.current_user_id')::UUID);

CREATE POLICY user_budgets_policy ON budgets
    FOR ALL TO authenticated_users
    USING (user_id = current_setting('app.current_user_id')::UUID);

-- Create role for application
CREATE ROLE app_user WITH LOGIN PASSWORD 'secure_app_password_2025';
CREATE ROLE authenticated_users;
GRANT authenticated_users TO app_user;

-- Grant necessary permissions
GRANT USAGE ON SCHEMA public TO app_user;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO app_user;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO app_user;

-- Analyze tables for query optimization
ANALYZE;
