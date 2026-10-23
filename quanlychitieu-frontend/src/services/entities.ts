import { api } from './api';

// Types
export interface Wallet {
  id: number;
  name: string;
  description?: string;
  balance: number;
  currency: string;
  isDefault: boolean;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateWalletDto {
  name: string;
  description?: string;
  currency?: string;
  isDefault?: boolean;
}

export interface UpdateWalletDto {
  name: string;
  description?: string;
  currency?: string;
  isDefault?: boolean;
  isActive?: boolean;
}

export interface WalletBalance {
  walletId: number;
  walletName: string;
  balance: number;
  currency: string;
}

export interface TransferDto {
  fromWalletId: number;
  toWalletId: number;
  amount: number;
  note?: string;
}

export interface Category {
  id: number;
  name: string;
  description?: string;
  icon: string;
  color: string;
  isIncome: boolean;
  isSystem: boolean;
  isActive: boolean;
  createdAt: string;
}

export interface CreateCategoryDto {
  name: string;
  description?: string;
  icon?: string;
  color?: string;
  isIncome: boolean;
}

export interface UpdateCategoryDto {
  name: string;
  description?: string;
  icon?: string;
  color?: string;
  isIncome: boolean;
  isActive: boolean;
}

export interface Transaction {
  id: number;
  amount: number;
  note?: string;
  transactionDate: string;
  isIncome: boolean;
  walletId: number;
  walletName: string;
  categoryId: number;
  categoryName: string;
  categoryIcon: string;
  categoryColor: string;
  createdAt: string;
}

export interface CreateTransactionDto {
  amount: number;
  note?: string;
  transactionDate: string;
  isIncome: boolean;
  walletId: number;
  categoryId: number;
}

export interface UpdateTransactionDto {
  amount: number;
  note?: string;
  transactionDate: string;
  isIncome: boolean;
  walletId: number;
  categoryId: number;
}

export interface TransactionFilter {
  startDate?: string;
  endDate?: string;
  walletId?: number;
  categoryId?: number;
  isIncome?: boolean;
  page?: number;
  pageSize?: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface Budget {
  id: number;
  name: string;
  amount: number;
  spentAmount: number;
  remainingAmount: number;
  percentageUsed: number;
  startDate: string;
  endDate: string;
  categoryId: number;
  categoryName: string;
  categoryIcon: string;
  categoryColor: string;
  walletId?: number;
  walletName?: string;
  isOverBudget: boolean;
  isNearLimit: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateBudgetDto {
  name: string;
  amount: number;
  startDate: string;
  endDate: string;
  categoryId: number;
  walletId?: number;
}

export interface UpdateBudgetDto {
  name: string;
  amount: number;
  startDate: string;
  endDate: string;
  categoryId: number;
  walletId?: number;
  isActive: boolean;
}

export interface BudgetAlert {
  budgetId: number;
  budgetName: string;
  categoryName: string;
  amount: number;
  spentAmount: number;
  percentageUsed: number;
  alertType: 'WARNING' | 'OVER_BUDGET';
  message: string;
}

export interface DashboardSummary {
  totalIncome: number;
  totalExpense: number;
  balance: number;
  totalWallets: number;
  totalTransactions: number;
  activeBudgets: number;
  budgetAlerts: BudgetAlert[];
  walletBalances: WalletBalance[];
}

export interface CategorySpending {
  categoryId: number;
  categoryName: string;
  categoryIcon: string;
  categoryColor: string;
  amount: number;
  transactionCount: number;
  percentage: number;
}

export interface DropdownItem {
  value: string;
  label: string;
  disabled?: boolean;
}

export interface MonthlyReport {
  year: number;
  month: number;
  totalIncome: number;
  totalExpense: number;
  balance: number;
  incomeByCategory: CategorySpending[];
  expenseByCategory: CategorySpending[];
  dailyBalances: DailyBalance[];
}

export interface DailyBalance {
  date: string;
  income: number;
  expense: number;
  balance: number;
}

export interface YearlyReport {
  year: number;
  totalIncome: number;
  totalExpense: number;
  balance: number;
  monthlySummaries: MonthlySummary[];
  topExpenseCategories: CategorySpending[];
  topIncomeCategories: CategorySpending[];
}

export interface MonthlySummary {
  month: number;
  monthName: string;
  income: number;
  expense: number;
  balance: number;
}

export interface SummaryResponse {
  totalIncome: number;
  totalExpense: number;
  balance: number;
}

// Wallet API
export const walletApi = {
  getAll: () => api.get<Wallet[]>('/wallets'),
  getById: (id: number) => api.get<Wallet>(`/wallets/${id}`),
  create: (data: CreateWalletDto) => api.post<Wallet>('/wallets', data),
  update: (id: number, data: UpdateWalletDto) => api.put<Wallet>(`/wallets/${id}`, data),
  delete: (id: number) => api.delete(`/wallets/${id}`),
  setDefault: (id: number) => api.put(`/wallets/${id}/set-default`),
  getBalance: (id: number) => api.get<WalletBalance>(`/wallets/${id}/balance`),
  transfer: (data: TransferDto) => api.post('/wallets/transfer', data),
};

// Category API
export const categoryApi = {
  getAll: (isIncome?: boolean) => api.get<Category[]>('/categories', { params: { isIncome } }),
  getById: (id: number) => api.get<Category>(`/categories/${id}`),
  create: (data: CreateCategoryDto) => api.post<Category>('/categories', data),
  update: (id: number, data: UpdateCategoryDto) => api.put<Category>(`/categories/${id}`, data),
  delete: (id: number) => api.delete(`/categories/${id}`),
};

// Transaction API
export const transactionApi = {
  getAll: (filter: TransactionFilter) => api.get<PagedResult<Transaction>>('/transactions', { params: filter }),
  getById: (id: number) => api.get<Transaction>(`/transactions/${id}`),
  create: (data: CreateTransactionDto) => api.post<Transaction>('/transactions', data),
  update: (id: number, data: UpdateTransactionDto) => api.put<Transaction>(`/transactions/${id}`, data),
  delete: (id: number) => api.delete(`/transactions/${id}`),
  getSummary: (startDate: string, endDate: string, walletId?: number) => 
    api.get<SummaryResponse>('/transactions/summary', { params: { startDate, endDate, walletId } }),
};

// Budget API
export const budgetApi = {
  getAll: () => api.get<Budget[]>('/budgets'),
  getById: (id: number) => api.get<Budget>(`/budgets/${id}`),
  create: (data: CreateBudgetDto) => api.post<Budget>('/budgets', data),
  update: (id: number, data: UpdateBudgetDto) => api.put<Budget>(`/budgets/${id}`, data),
  delete: (id: number) => api.delete(`/budgets/${id}`),
  getAlerts: () => api.get<BudgetAlert[]>('/budgets/alerts'),
  recalculate: (id: number) => api.post<Budget>(`/budgets/${id}/recalculate`),
};

// Report API (these endpoints need to be implemented on backend)
export const reportApi = {
  getDashboard: () => api.get<DashboardSummary>('/reports/dashboard'),
  getMonthly: (year: number, month: number) => api.get<MonthlyReport>(`/reports/monthly/${year}/${month}`),
  getYearly: (year: number) => api.get<YearlyReport>(`/reports/yearly/${year}`),
};
