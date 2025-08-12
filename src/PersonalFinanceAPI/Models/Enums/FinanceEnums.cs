namespace PersonalFinanceAPI.Models.Enums;

public enum PeriodType
{
    WEEKLY,
    MONTHLY,
    QUARTERLY,
    YEARLY
}

public enum TransactionType
{
    DEBIT,
    CREDIT
}

public enum InvestmentRiskLevel
{
    LOW,
    MODERATE,
    HIGH
}

public enum CategoryType
{
    INCOME,
    EXPENSE,
    TRANSFER
}

public enum AccountType
{
    SAVINGS,
    CURRENT,
    CREDIT_CARD,
    INVESTMENT
}

public enum RecurringFrequency
{
    DAILY,
    WEEKLY,
    MONTHLY,
    QUARTERLY,
    YEARLY
}

public enum GoalType
{
    SAVINGS,
    INVESTMENT,
    DEBT_PAYOFF,
    EMERGENCY_FUND
}

public enum AlertSeverity
{
    INFO,
    WARNING,
    CRITICAL
}

public enum SuggestionAction
{
    VIEWED,
    ACCEPTED,
    REJECTED,
    POSTPONED
}
