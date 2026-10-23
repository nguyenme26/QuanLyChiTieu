import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import type { DashboardSummary, Wallet, Category, Transaction, BudgetAlert, DropdownItem } from '../services/entities';
import { reportApi, walletApi, categoryApi, transactionApi, budgetApi } from '../services/entities';
import { StatCard, Card, CardBody, CardHeader, Button, EmptyState, Badge, Dropdown, Avatar } from '../components/UI';
import { Layout } from '../components/Layout';
import { formatCurrency, formatRelativeTime } from '../utils/format';

export function DashboardPage() {
  const [summary, setSummary] = useState<DashboardSummary | null>(null);
  const [recentTransactions, setRecentTransactions] = useState<Transaction[]>([]);
  const [wallets, setWallets] = useState<Wallet[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [budgetAlerts, setBudgetAlerts] = useState<BudgetAlert[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadDashboardData();
  }, []);

  const loadDashboardData = async () => {
    try {
      setIsLoading(true);
      const [summaryRes, transactionsRes, walletsRes, categoriesRes, alertsRes] = await Promise.allSettled([
        reportApi.getDashboard(),
        transactionApi.getAll({ page: 1, pageSize: 5 }),
        walletApi.getAll(),
        categoryApi.getAll(),
        budgetApi.getAlerts(),
      ]);

      if (summaryRes.status === 'fulfilled') setSummary(summaryRes.value.data);
      if (transactionsRes.status === 'fulfilled') setRecentTransactions(transactionsRes.value.data.items);
      if (walletsRes.status === 'fulfilled') setWallets(walletsRes.value.data);
      if (categoriesRes.status === 'fulfilled') setCategories(categoriesRes.value.data);
      if (alertsRes.status === 'fulfilled') setBudgetAlerts(alertsRes.value.data);
    } catch (err) {
      setError('Không thể tải dữ liệu. Vui lòng thử lại.');
      console.error(err);
    } finally {
      setIsLoading(false);
    }
  };

  if (isLoading) {
    return (
      <Layout>
        <div className="loading-container">
          <div className="spinner spinner-lg"></div>
          <p style={{ marginTop: 'var(--space-4)', color: 'var(--text-secondary)' }}>Đang tải dữ liệu...</p>
        </div>
      </Layout>
    );
  }

  if (error) {
    return (
      <Layout>
        <EmptyState
          icon="⚠️"
          title="Có lỗi xảy ra"
          message={error}
          action={<Button onClick={loadDashboardData}>Thử lại</Button>}
        />
      </Layout>
    );
  }

  const totalBalance = summary?.balance || 0;
  const totalIncome = summary?.totalIncome || 0;
  const totalExpense = summary?.totalExpense || 0;

  const recentWalletOptions: DropdownItem[] = wallets.slice(0, 5).map(w => ({
    label: `${w.name} (${formatCurrency(w.balance)})`,
    onClick: () => {},
    icon: '👛',
  }));

  return (
    <Layout>
      <div className="page-header" style={{ marginBottom: 'var(--space-6)' }}>
        <div className="flex items-center justify-between flex-wrap gap-4">
          <div>
            <h1 style={{ fontSize: '1.75rem', fontWeight: '700', marginBottom: 'var(--space-1)' }}>Tổng quan tài chính</h1>
            <p style={{ color: 'var(--text-secondary)' }}>Xin chào! Dưới đây là tổng quan tài chính của bạn</p>
          </div>
          <div className="flex gap-3">
            <Dropdown
              trigger={<Button variant="secondary" leftIcon="👛">Chọn ví</Button>}
              items={recentWalletOptions}
            />
            <Button variant="primary" leftIcon="➕" onClick={() => {}}>Thêm giao dịch</Button>
          </div>
        </div>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-4" style={{ marginBottom: 'var(--space-8)' }}>
        <StatCard
          label="Tổng thu nhập"
          value={formatCurrency(totalIncome)}
          icon="📈"
          iconVariant="income"
          change="+12.5% so với tháng trước"
          changeType="positive"
        />
        <StatCard
          label="Tổng chi tiêu"
          value={formatCurrency(totalExpense)}
          icon="📉"
          iconVariant="expense"
          change="+8.2% so với tháng trước"
          changeType="negative"
        />
        <StatCard
          label="Số dư hiện tại"
          value={formatCurrency(totalBalance)}
          icon="💰"
          iconVariant={totalBalance >= 0 ? 'income' : 'expense'}
        />
        <StatCard
          label="Ngân sách đang theo dõi"
          value={summary?.activeBudgets || 0}
          icon="🎯"
          iconVariant="balance"
        />
      </div>

      <div className="grid" style={{ gridTemplateColumns: '2fr 1fr', gap: 'var(--space-6)' }}>
        {/* Recent Transactions */}
        <Card hover>
          <CardHeader>
            <div className="flex items-center justify-between">
              <h3 style={{ fontSize: '1.125rem', fontWeight: '600' }}>Giao dịch gần đây</h3>
              <Link to="/transactions" className="btn btn-ghost btn-sm">Xem tất cả</Link>
            </div>
          </CardHeader>
          <CardBody style={{ padding: 0 }}>
            {recentTransactions.length === 0 ? (
              <EmptyState
                icon="📝"
                title="Chưa có giao dịch nào"
                message="Hãy thêm giao dịch đầu tiên để bắt đầu theo dõi chi tiêu"
                action={<Button variant="primary" leftIcon="➕" onClick={() => {}}>Thêm giao dịch</Button>}
              />
            ) : (
              <div className="transaction-list">
                {recentTransactions.map((tx, index) => (
                  <Link 
                    key={tx.id} 
                    to={`/transactions/${tx.id}`}
                    className={`transaction-item ${index === recentTransactions.length - 1 ? 'last' : ''}`}
                  >
                    <div className="transaction-icon" style={{ background: tx.categoryColor + '20', color: tx.categoryColor }}>
                      {tx.categoryIcon}
                    </div>
                    <div className="transaction-details">
                      <div className="transaction-name">{tx.categoryName}</div>
                      <div className="transaction-meta">
                        <span>{tx.walletName}</span>
                        <span>•</span>
                        <span>{formatRelativeTime(tx.transactionDate)}</span>
                      </div>
                    </div>
                    <div className="transaction-amount" style={{ color: tx.isIncome ? 'var(--accent-primary)' : 'var(--accent-danger)' }}>
                      {tx.isIncome ? '+' : '-'}{formatCurrency(tx.amount)}
                    </div>
                  </Link>
                ))}
              </div>
            )}
          </CardBody>
        </Card>

        {/* Right Column: Budget Alerts & Wallets */}
        <div className="flex flex-col gap-6">
          {/* Budget Alerts */}
          <Card hover>
            <CardHeader>
              <h3 style={{ fontSize: '1.125rem', fontWeight: '600', display: 'flex', alignItems: 'center', gap: 'var(--space-2)' }}>
                <span style={{ color: 'var(--accent-warning)' }}>⚠</span>
                Cảnh báo ngân sách
              </h3>
            </CardHeader>
            <CardBody style={{ padding: 0 }}>
              {budgetAlerts.length === 0 ? (
                <div style={{ padding: 'var(--space-6)', textAlign: 'center', color: 'var(--accent-primary)' }}>
                  <div style={{ fontSize: '3rem', marginBottom: 'var(--space-3)' }}>✅</div>
                  <p style={{ fontWeight: '500' }}>Tất cả ngân sách an toàn</p>
                  <p style={{ fontSize: '0.875rem', color: 'var(--text-secondary)', marginTop: 'var(--space-1)' }}>Không có ngân sách nào vượt hạn mức</p>
                </div>
              ) : (
                <div className="alert-list">
                  {budgetAlerts.map((alert, index) => (
                    <div 
                      key={alert.budgetId} 
                      className={`alert-item ${index === budgetAlerts.length - 1 ? 'last' : ''}`}
                      style={{ borderLeftColor: alert.alertType === 'OVER_BUDGET' ? 'var(--accent-danger)' : 'var(--accent-warning)' }}
                    >
                      <div className="alert-info">
                        <div className="alert-name">{alert.budgetName}</div>
                        <div className="alert-category">{alert.categoryName}</div>
                      </div>
                      <div className="alert-stats">
                        <Badge variant={alert.alertType === 'OVER_BUDGET' ? 'danger' : 'warning'}>
                          {alert.alertType === 'OVER_BUDGET' ? 'Vượt ngân sách' : 'Sắp vượt'}
                        </Badge>
                        <div className="alert-progress" style={{ marginTop: 'var(--space-2)' }}>
                          <div 
                            className="progress-bar" 
                            style={{ 
                              width: `${Math.min(alert.percentageUsed, 100)}%`,
                              background: alert.alertType === 'OVER_BUDGET' ? 'var(--accent-danger)' : 'var(--accent-warning)'
                            }} 
                          />
                        </div>
                        <div className="alert-amount">
                          {formatCurrency(alert.spentAmount)} / {formatCurrency(alert.amount)}
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </CardBody>
          </Card>

          {/* Wallet Balances */}
          <Card hover>
            <CardHeader>
              <h3 style={{ fontSize: '1.125rem', fontWeight: '600', display: 'flex', alignItems: 'center', gap: 'var(--space-2)' }}>
                <span>👛</span>
                Số dư ví
              </h3>
            </CardHeader>
            <CardBody style={{ padding: 0 }}>
              {wallets.length === 0 ? (
                <EmptyState
                  icon="👛"
                  title="Chưa có ví nào"
                  message="Tạo ví đầu tiên để bắt đầu quản lý tiền"
                  action={<Button variant="primary" size="sm" leftIcon="➕" onClick={() => {}}>Tạo ví</Button>}
                />
              ) : (
                <div className="wallet-list">
                  {wallets.map((wallet, index) => (
                    <Link 
                      key={wallet.id} 
                      to={`/wallets/${wallet.id}`}
                      className={`wallet-item ${index === wallets.length - 1 ? 'last' : ''}`}
                    >
                      <div className="wallet-info">
                        <div className="wallet-icon" style={{ background: wallet.isDefault ? 'var(--gradient-primary)' : 'var(--bg-tertiary)' }}>
                          {wallet.isDefault ? '⭐' : '👛'}
                        </div>
                        <div className="wallet-details">
                          <div className="wallet-name">
                            {wallet.name}
                            {wallet.isDefault && <Badge variant="primary" style={{ fontSize: '0.625rem', marginLeft: 'var(--space-2)' }}>Mặc định</Badge>}
                          </div>
                          <div className="wallet-currency">{wallet.currency}</div>
                        </div>
                      </div>
                      <div className="wallet-balance" style={{ color: wallet.balance >= 0 ? 'var(--text-primary)' : 'var(--accent-danger)' }}>
                        {formatCurrency(wallet.balance)}
                      </div>
                    </Link>
                  ))}
                </div>
              )}
            </CardBody>
          </Card>
        </div>
      </div>

      {/* Quick Actions */}
      <div style={{ marginTop: 'var(--space-8)' }}>
        <Card hover>
          <CardHeader>
            <h3 style={{ fontSize: '1.125rem', fontWeight: '600' }}>Thao tác nhanh</h3>
          </CardHeader>
          <CardBody>
            <div className="grid grid-4" style={{ gap: 'var(--space-4)' }}>
              <Button variant="secondary" leftIcon="➕" className="quick-action-btn" onClick={() => {}}>
                <div className="quick-action-icon income">➕</div>
                <div className="quick-action-label">Thu nhập</div>
              </Button>
              <Button variant="secondary" leftIcon="➖" className="quick-action-btn" onClick={() => {}}>
                <div className="quick-action-icon expense">➖</div>
                <div className="quick-action-label">Chi tiêu</div>
              </Button>
              <Button variant="secondary" leftIcon="🔄" className="quick-action-btn" onClick={() => {}}>
                <div className="quick-action-icon transfer">🔄</div>
                <div className="quick-action-label">Chuyển tiền</div>
              </Button>
              <Button variant="secondary" leftIcon="🎯" className="quick-action-btn" onClick={() => {}}>
                <div className="quick-action-icon budget">🎯</div>
                <div className="quick-action-label">Ngân sách</div>
              </Button>
            </div>
          </CardBody>
        </Card>
      </div>
    </Layout>
  );
}
