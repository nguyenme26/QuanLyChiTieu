import { useState, useEffect } from 'react';
import type { DashboardSummary, MonthlyReport, YearlyReport } from '../services/entities';
import { reportApi } from '../services/entities';
import { Card, CardHeader, CardBody, Button, Select, Badge, EmptyState } from '../components/UI';
import { Layout } from '../components/Layout';
import { AreaChart, Area, BarChart, Bar, PieChart, Pie, Cell, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer, ComposedChart } from 'recharts';
import { format, subMonths } from 'date-fns';
import { vi } from 'date-fns/locale';
import { formatCurrency, formatPercent } from '../utils/format';
import toast from 'react-hot-toast';

const COLORS = ['#10b981', '#3b82f6', '#f59e0b', '#ef4444', '#8b5cf6', '#06b6d4', '#ec4899', '#6366f1', '#14b8a6', '#f97316'];

export function ReportsPage() {
  const [activeTab, setActiveTab] = useState<'dashboard' | 'monthly' | 'yearly'>('dashboard');
  const [dashboardData, setDashboardData] = useState<DashboardSummary | null>(null);
  const [monthlyData, setMonthlyData] = useState<MonthlyReport | null>(null);
  const [yearlyData, setYearlyData] = useState<YearlyReport | null>(null);
  const [_isLoading, setIsLoading] = useState(true);

  // Filters
  const [selectedMonth, setSelectedMonth] = useState(new Date());
  const [selectedYear, setSelectedYear] = useState(new Date().getFullYear());
  const [availableYears, setAvailableYears] = useState<number[]>([]);

  useEffect(() => {
    loadDashboardData();
    // Generate available years (current year - 5 to current year + 1)
    const currentYear = new Date().getFullYear();
    setAvailableYears(Array.from({ length: 7 }, (_, i) => currentYear - 5 + i));
  }, []);

  useEffect(() => {
    loadMonthlyReport();
  }, [selectedMonth]);

  useEffect(() => {
    loadYearlyReport();
  }, [selectedYear]);

  const loadDashboardData = async () => {
    try {
      const response = await reportApi.getDashboard();
      setDashboardData(response.data);
    } catch (err) {
      toast.error('Không thể tải tổng quan');
    } finally {
      setIsLoading(false);
    }
  };

  const loadMonthlyReport = async () => {
    try {
      const year = selectedMonth.getFullYear();
      const month = selectedMonth.getMonth() + 1;
      const response = await reportApi.getMonthly(year, month);
      setMonthlyData(response.data);
    } catch (err) {
      toast.error('Không thể tải báo cáo tháng');
    }
  };

  const loadYearlyReport = async () => {
    try {
      const response = await reportApi.getYearly(selectedYear);
      setYearlyData(response.data);
    } catch (err) {
      toast.error('Không thể tải báo cáo năm');
    }
  };



  const handleExport = async (_type: 'csv' | 'excel') => {
    try {
      // For now, we'll create a simple CSV export
      let csvContent = '';
      let filename = '';
      
      if (activeTab === 'monthly' && monthlyData) {
        csvContent = generateMonthlyCSV(monthlyData);
        filename = `bao-cao-thang-${selectedMonth.getMonth() + 1}-${selectedMonth.getFullYear()}.csv`;
      } else if (activeTab === 'yearly' && yearlyData) {
        csvContent = generateYearlyCSV(yearlyData);
        filename = `bao-cao-nam-${selectedYear}.csv`;
      } else if (dashboardData) {
        csvContent = generateDashboardCSV(dashboardData);
        filename = `tong-quan-${format(new Date(), 'dd-MM-yyyy')}.csv`;
      }
      
      if (csvContent) {
        downloadCSV(csvContent, filename);
        toast.success('Xuất báo cáo thành công');
      }
    } catch (err) {
      toast.error('Xuất báo cáo thất bại');
    }
  };

  const generateMonthlyCSV = (data: MonthlyReport): string => {
    const rows = [
      ['BÁO CÁO THÁNG', `${data.month}/${data.year}`],
      ['', ''],
      ['TỔNG QUAN', ''],
      ['Tổng thu nhập', formatCurrency(data.totalIncome)],
      ['Tổng chi tiêu', formatCurrency(data.totalExpense)],
      ['Số dư', formatCurrency(data.balance)],
      ['', ''],
      ['THU NHẬP THEO DANH MỤC', ''],
      ['Danh mục', 'Số tiền', 'Số giao dịch', 'Tỷ lệ %'],
      ...data.incomeByCategory.map(c => [c.categoryName, formatCurrency(c.amount), c.transactionCount.toString(), `${c.percentage.toFixed(1)}%`]),
      ['', ''],
      ['CHI TIÊU THEO DANH MỤC', ''],
      ['Danh mục', 'Số tiền', 'Số giao dịch', 'Tỷ lệ %'],
      ...data.expenseByCategory.map(c => [c.categoryName, formatCurrency(c.amount), c.transactionCount.toString(), `${c.percentage.toFixed(1)}%`]),
    ];
    return rows.map(r => r.map(v => `"${v}"`).join(',')).join('\n');
  };

  const generateYearlyCSV = (data: YearlyReport): string => {
    const rows = [
      ['BÁO CÁO NĂM', data.year.toString()],
      ['', ''],
      ['TỔNG QUAN', ''],
      ['Tổng thu nhập', formatCurrency(data.totalIncome)],
      ['Tổng chi tiêu', formatCurrency(data.totalExpense)],
      ['Số dư', formatCurrency(data.balance)],
      ['', ''],
      ['TỔNG HỢP HÀNG THÁNG', ''],
      ['Tháng', 'Thu nhập', 'Chi tiêu', 'Số dư'],
      ...data.monthlySummaries.map(m => [m.monthName, formatCurrency(m.income), formatCurrency(m.expense), formatCurrency(m.balance)]),
    ];
    return rows.map(r => r.map(v => `"${v}"`).join(',')).join('\n');
  };

  const generateDashboardCSV = (data: DashboardSummary): string => {
    const rows = [
      ['TỔNG QUAN TÀI CHÍNH', format(new Date(), 'dd/MM/yyyy', { locale: vi })],
      ['', ''],
      ['Tổng thu nhập', formatCurrency(data.totalIncome)],
      ['Tổng chi tiêu', formatCurrency(data.totalExpense)],
      ['Số dư', formatCurrency(data.balance)],
      ['Số ví', data.totalWallets.toString()],
      ['Tổng giao dịch', data.totalTransactions.toString()],
      ['Ngân sách đang theo dõi', data.activeBudgets.toString()],
    ];
    return rows.map(r => r.map(v => `"${v}"`).join(',')).join('\n');
  };

  const downloadCSV = (content: string, filename: string) => {
    const blob = new Blob(['\uFEFF' + content], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = filename;
    link.click();
    URL.revokeObjectURL(link.href);
  };

  return (
    <Layout>
      <div className="page-header">
        <div className="flex items-center justify-between flex-wrap gap-4">
          <div>
            <h1 style={{ fontSize: '1.75rem', fontWeight: '700', marginBottom: 'var(--space-1)' }}>Báo cáo & Thống kê</h1>
            <p style={{ color: 'var(--text-secondary)' }}>Phân tích tài chính chi tiết với biểu đồ trực quan</p>
          </div>
          <Button variant="primary" leftIcon="📥" onClick={() => handleExport('csv')}>Xuất CSV</Button>
        </div>
      </div>

      {/* Tabs */}
      <div className="tabs" style={{ marginBottom: 'var(--space-6)' }} role="tablist">
        <button
          role="tab"
          aria-selected={activeTab === 'dashboard'}
          className={`tab ${activeTab === 'dashboard' ? 'active' : ''}`}
          onClick={() => setActiveTab('dashboard')}
        >
          📊 Tổng quan
        </button>
        <button
          role="tab"
          aria-selected={activeTab === 'monthly'}
          className={`tab ${activeTab === 'monthly' ? 'active' : ''}`}
          onClick={() => setActiveTab('monthly')}
        >
          📅 Báo cáo tháng
        </button>
        <button
          role="tab"
          aria-selected={activeTab === 'yearly'}
          className={`tab ${activeTab === 'yearly' ? 'active' : ''}`}
          onClick={() => setActiveTab('yearly')}
        >
          📈 Báo cáo năm
        </button>
      </div>

      {activeTab === 'dashboard' && dashboardData && (
        <DashboardView data={dashboardData} />
      )}

      {activeTab === 'monthly' && monthlyData && (
        <MonthlyView 
          data={monthlyData} 
          selectedMonth={selectedMonth}
          onMonthChange={setSelectedMonth}
        />
      )}

      {activeTab === 'yearly' && yearlyData && (
        <YearlyView 
          data={yearlyData} 
          selectedYear={selectedYear}
          onYearChange={setSelectedYear}
          availableYears={availableYears}
        />
      )}

      {activeTab === 'monthly' && !monthlyData && (
        <EmptyState icon="📊" title="Chưa có dữ liệu tháng này" message="Thêm giao dịch để xem báo cáo" />
      )}

      {activeTab === 'yearly' && !yearlyData && (
        <EmptyState icon="📈" title="Chưa có dữ liệu năm này" message="Thêm giao dịch để xem báo cáo" />
      )}
    </Layout>
  );
}

// Standalone CustomTooltip component used across report views
const CustomTooltip = ({ active, payload, label }: any) => {
  if (active && payload && payload.length) {
    return (
      <div style={{ background: 'var(--bg-card)', border: '1px solid var(--border-primary)', borderRadius: 'var(--radius-md)', padding: 'var(--space-3)', boxShadow: 'var(--shadow-lg)' }}>
        <p style={{ fontWeight: '600', marginBottom: 'var(--space-2)' }}>{label}</p>
        {payload.map((entry: any, index: number) => (
          <p key={index} style={{ color: entry.color, margin: 'var(--space-1) 0', display: 'flex', gap: 'var(--space-2)' }}>
            <span style={{ width: '10px', height: '10px', borderRadius: '50%', background: entry.color, marginTop: '4px' }} />
            <span>{entry.name}: {formatCurrency(entry.value)}</span>
          </p>
        ))}
      </div>
    );
  }
  return null;
};

// Dashboard View Component
function DashboardView({ data }: { data: DashboardSummary }) {
  return (
    <>
      {/* Summary Cards */}
      <div className="grid grid-4" style={{ marginBottom: 'var(--space-6)' }}>
        <Card>
          <CardBody style={{ padding: 'var(--space-5)' }}>
            <div className="flex items-center justify-between">
              <div>
                <p style={{ fontSize: '0.875rem', color: 'var(--text-secondary)', marginBottom: 'var(--space-1)' }}>Tổng thu nhập</p>
                <p style={{ fontSize: '1.5rem', fontWeight: '700', color: 'var(--accent-primary)' }}>{formatCurrency(data.totalIncome)}</p>
              </div>
              <div className="avatar avatar-lg" style={{ background: 'linear-gradient(135deg, #10b981 0%, #059669 100%)' }}>📈</div>
            </div>
          </CardBody>
        </Card>
        <Card>
          <CardBody style={{ padding: 'var(--space-5)' }}>
            <div className="flex items-center justify-between">
              <div>
                <p style={{ fontSize: '0.875rem', color: 'var(--text-secondary)', marginBottom: 'var(--space-1)' }}>Tổng chi tiêu</p>
                <p style={{ fontSize: '1.5rem', fontWeight: '700', color: 'var(--accent-danger)' }}>{formatCurrency(data.totalExpense)}</p>
              </div>
              <div className="avatar avatar-lg" style={{ background: 'linear-gradient(135deg, #ef4444 0%, #dc2626 100%)' }}>📉</div>
            </div>
          </CardBody>
        </Card>
        <Card>
          <CardBody style={{ padding: 'var(--space-5)' }}>
            <div className="flex items-center justify-between">
              <div>
                <p style={{ fontSize: '0.875rem', color: 'var(--text-secondary)', marginBottom: 'var(--space-1)' }}>Số dư ròng</p>
                <p style={{ fontSize: '1.5rem', fontWeight: '700', color: data.balance >= 0 ? 'var(--accent-primary)' : 'var(--accent-danger)' }}>
                  {formatCurrency(data.balance)}
                </p>
              </div>
              <div className="avatar avatar-lg" style={{ background: 'var(--gradient-secondary)' }}>💰</div>
            </div>
          </CardBody>
        </Card>
        <Card>
          <CardBody style={{ padding: 'var(--space-5)' }}>
            <div className="flex items-center justify-between">
              <div>
                <p style={{ fontSize: '0.875rem', color: 'var(--text-secondary)', marginBottom: 'var(--space-1)' }}>Tỷ lệ tiết kiệm</p>
                <p style={{ fontSize: '1.5rem', fontWeight: '700' }}>
                  {data.totalIncome > 0 ? formatPercent(((data.totalIncome - data.totalExpense) / data.totalIncome) * 100) : '0%'}
                </p>
              </div>
              <div className="avatar avatar-lg" style={{ background: 'linear-gradient(135deg, #8b5cf6 0%, #7c3aed 100%)' }}>📊</div>
            </div>
          </CardBody>
        </Card>
      </div>

      <div className="grid" style={{ gridTemplateColumns: '1fr 1fr', gap: 'var(--space-6)' }}>
        {/* Expense by Category Pie Chart */}
        <Card>
          <CardHeader>
            <h3 style={{ fontSize: '1.125rem', fontWeight: '600' }}>Chi tiêu theo danh mục</h3>
          </CardHeader>
          <CardBody>
            <div style={{ height: 300 }}>
              <ResponsiveContainer width="100%" height="100%">
                <PieChart>
                  <Pie
                    data={data.walletBalances?.slice(0, 8) || []}
                    cx="50%"
                    cy="50%"
                    innerRadius={60}
                    outerRadius={100}
                    paddingAngle={2}
                    dataKey="balance"
                    nameKey="walletName"
                    label={({ name, value }) => {
                      const walletName = name || 'Unknown';
                      const valueNum = typeof value === 'number' ? value : 0;
                      return `${walletName} ${(valueNum).toFixed(0)} VND`;
                    }}
                    labelLine={false}
                  >
                    {data.walletBalances?.slice(0, 8).map((_, index) => (
                      <Cell key={index} fill={COLORS[index % COLORS.length]} />
                    ))}
                  </Pie>
                  <Tooltip formatter={((value: number | undefined) => formatCurrency(value || 0)) as any} />
                </PieChart>
              </ResponsiveContainer>
            </div>
          </CardBody>
        </Card>

        {/* Wallet Balances */}
        <Card>
          <CardHeader>
            <h3 style={{ fontSize: '1.125rem', fontWeight: '600' }}>Số dư các ví</h3>
          </CardHeader>
          <CardBody>
            <div style={{ height: 300 }}>
              <ResponsiveContainer width="100%" height="100%">
                <BarChart data={data.walletBalances || []} layout="vertical">
                  <CartesianGrid strokeDasharray="3 3" stroke="var(--border-primary)" vertical={false} />
                  <XAxis type="number" tickFormatter={(value) => formatCurrency(typeof value === 'number' ? value : 0)} stroke="var(--text-muted)" tickLine={false} axisLine={false} />
                  <YAxis dataKey="walletName" type="category" width={120} stroke="var(--text-muted)" tickLine={false} axisLine={false} />
                  <Tooltip content={<CustomTooltip />} formatter={((value: number | undefined) => formatCurrency(value || 0)) as any} />
                  <Bar dataKey="balance" fill="var(--accent-primary)" radius={[0, 6, 6, 0]} maxBarSize={40} />
                </BarChart>
              </ResponsiveContainer>
            </div>
          </CardBody>
        </Card>
      </div>

      {/* Budget Alerts */}
      {data.budgetAlerts && data.budgetAlerts.length > 0 && (
        <Card style={{ marginTop: 'var(--space-6)' }}>
          <CardHeader>
            <h3 style={{ fontSize: '1.125rem', fontWeight: '600', display: 'flex', alignItems: 'center', gap: 'var(--space-2)' }}>
              <span style={{ color: 'var(--accent-warning)' }}>⚠</span>
              Cảnh báo ngân sách
            </h3>
          </CardHeader>
          <CardBody>
            <div className="grid grid-auto" style={{ gap: 'var(--space-4)' }}>
              {data.budgetAlerts.map((alert) => (
                <div key={alert.budgetId} className="budget-alert-card" style={{ borderLeft: `4px solid ${alert.alertType === 'OVER_BUDGET' ? 'var(--accent-danger)' : 'var(--accent-warning)'}`, padding: 'var(--space-4)', background: 'var(--bg-card)', borderRadius: 'var(--radius-md)' }}>
                  <div className="flex items-center justify-between">
                    <div>
                      <p style={{ fontWeight: '600' }}>{alert.budgetName}</p>
                      <p style={{ fontSize: '0.875rem', color: 'var(--text-secondary)' }}>{alert.categoryName}</p>
                    </div>
                    <Badge variant={alert.alertType === 'OVER_BUDGET' ? 'danger' : 'warning'}>
                      {alert.alertType === 'OVER_BUDGET' ? 'Vượt ngân sách' : 'Sắp vượt (80%)'}
                    </Badge>
                  </div>
                  <div style={{ marginTop: 'var(--space-3)' }}>
                    <div className="flex justify-between text-sm" style={{ marginBottom: 'var(--space-1)' }}>
                      <span>{formatCurrency(alert.spentAmount)} / {formatCurrency(alert.amount)}</span>
                      <span style={{ fontWeight: '600', color: alert.alertType === 'OVER_BUDGET' ? 'var(--accent-danger)' : 'var(--accent-warning)' }}>
                        {alert.percentageUsed.toFixed(1)}%
                      </span>
                    </div>
                    <div style={{ height: '6px', background: 'var(--bg-tertiary)', borderRadius: 'var(--radius-full)', overflow: 'hidden' }}>
                      <div style={{ height: '100%', width: `${Math.min(alert.percentageUsed, 100)}%`, background: alert.alertType === 'OVER_BUDGET' ? 'var(--accent-danger)' : 'var(--accent-warning)', borderRadius: 'var(--radius-full)', transition: 'width 0.3s ease' }} />
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </CardBody>
        </Card>
      )}
    </>
  );
}

// Monthly View Component
function MonthlyView({ data, selectedMonth, onMonthChange }: { data: MonthlyReport; selectedMonth: Date; onMonthChange: (d: Date) => void }) {
  const monthName = format(selectedMonth, 'MMMM yyyy', { locale: vi });
  
  return (
    <>
      {/* Month Selector */}
      <Card style={{ marginBottom: 'var(--space-6)' }}>
        <CardBody style={{ padding: 'var(--space-4) var(--space-6)' }}>
          <div className="flex items-center justify-between flex-wrap gap-4">
            <div className="flex items-center gap-4">
              <Button variant="ghost" size="sm" onClick={() => onMonthChange(subMonths(selectedMonth, 1))} leftIcon="‹" />
              <h3 style={{ fontSize: '1.25rem', fontWeight: '700', textTransform: 'capitalize' }}>{monthName}</h3>
              <Button variant="ghost" size="sm" onClick={() => onMonthChange(new Date(selectedMonth.getFullYear(), selectedMonth.getMonth() + 1, 1))} rightIcon="›" />
            </div>
            <div className="flex items-center gap-4">
              <div className="stat-mini">
                <span style={{ color: 'var(--accent-primary)' }}>Thu: {formatCurrency(data.totalIncome)}</span>
              </div>
              <div className="stat-mini">
                <span style={{ color: 'var(--accent-danger)' }}>Chi: {formatCurrency(data.totalExpense)}</span>
              </div>
              <div className="stat-mini">
                <span style={{ color: data.balance >= 0 ? 'var(--accent-primary)' : 'var(--accent-danger)' }}>Dư: {formatCurrency(data.balance)}</span>
              </div>
            </div>
          </div>
        </CardBody>
      </Card>

      {/* Charts Row */}
      <div className="grid" style={{ gridTemplateColumns: '1fr 1fr', gap: 'var(--space-6)', marginBottom: 'var(--space-6)' }}>
        {/* Daily Balance Chart */}
        <Card>
          <CardHeader>
            <h3 style={{ fontSize: '1.125rem', fontWeight: '600' }}>Số dư hàng ngày</h3>
          </CardHeader>
          <CardBody>
            <div style={{ height: 300 }}>
              <ResponsiveContainer width="100%" height="100%">
                <AreaChart data={data.dailyBalances}>
                  <defs>
                    <linearGradient id="incomeGradient" x1="0" y1="0" x2="0" y2="1">
                      <stop offset="5%" stopColor="var(--accent-primary)" stopOpacity={0.3}/>
                      <stop offset="95%" stopColor="var(--accent-primary)" stopOpacity={0}/>
                    </linearGradient>
                    <linearGradient id="expenseGradient" x1="0" y1="0" x2="0" y2="1">
                      <stop offset="5%" stopColor="var(--accent-danger)" stopOpacity={0.3}/>
                      <stop offset="95%" stopColor="var(--accent-danger)" stopOpacity={0}/>
                    </linearGradient>
                    <linearGradient id="balanceGradient" x1="0" y1="0" x2="0" y2="1">
                      <stop offset="5%" stopColor="var(--accent-secondary)" stopOpacity={0.3}/>
                      <stop offset="95%" stopColor="var(--accent-secondary)" stopOpacity={0}/>
                    </linearGradient>
                  </defs>
                  <CartesianGrid strokeDasharray="3 3" stroke="var(--border-primary)" />
                  <XAxis dataKey="date" tickFormatter={(d) => format(new Date(d), 'dd/MM', { locale: vi })} stroke="var(--text-muted)" tickLine={false} axisLine={false} />
                  <YAxis tickFormatter={(value) => formatCurrency(typeof value === 'number' ? value : 0)} stroke="var(--text-muted)" tickLine={false} axisLine={false} />
                  <Tooltip content={<CustomTooltip />} formatter={((value: number | undefined) => formatCurrency(value || 0)) as any} />
                  <Legend />
                  <Area type="monotone" dataKey="income" name="Thu nhập" fill="url(#incomeGradient)" stroke="var(--accent-primary)" strokeWidth={2} />
                  <Area type="monotone" dataKey="expense" name="Chi tiêu" fill="url(#expenseGradient)" stroke="var(--accent-danger)" strokeWidth={2} />
                  <Area type="monotone" dataKey="balance" name="Số dư" fill="url(#balanceGradient)" stroke="var(--accent-secondary)" strokeWidth={2} />
                </AreaChart>
              </ResponsiveContainer>
            </div>
          </CardBody>
        </Card>

        {/* Expense by Category Pie */}
        <Card>
          <CardHeader>
            <h3 style={{ fontSize: '1.125rem', fontWeight: '600' }}>Phân bổ chi tiêu</h3>
          </CardHeader>
          <CardBody>
            <div style={{ height: 300 }}>
              <ResponsiveContainer width="100%" height="100%">
                <PieChart>
                  <Pie
                    data={data.expenseByCategory}
                    cx="50%"
                    cy="50%"
                    innerRadius={60}
                    outerRadius={100}
                    paddingAngle={2}
                    dataKey="amount"
                    nameKey="categoryName"
                    label={({ name, value }) => {
                      const categoryName = name || 'Unknown';
                      const valueNum = typeof value === 'number' ? value : 0;
                      return `${categoryName} ${(valueNum).toFixed(0)} VND`;
                    }}
                    labelLine={false}
                  >
                    {data.expenseByCategory.map((_, index) => (
                      <Cell key={index} fill={COLORS[index % COLORS.length]} />
                    ))}
                  </Pie>
                  <Tooltip formatter={((value: number | undefined) => formatCurrency(value || 0)) as any} />
                </PieChart>
              </ResponsiveContainer>
            </div>
          </CardBody>
        </Card>
      </div>

      {/* Category Breakdown Tables */}
      <div className="grid" style={{ gridTemplateColumns: '1fr 1fr', gap: 'var(--space-6)' }}>
        <Card>
          <CardHeader>
            <h3 style={{ fontSize: '1.125rem', fontWeight: '600', display: 'flex', alignItems: 'center', gap: 'var(--space-2)' }}>
              <span style={{ color: 'var(--accent-primary)' }}>📈</span>
              Thu nhập theo danh mục
            </h3>
          </CardHeader>
          <CardBody style={{ padding: 0 }}>
            <div className="table-container">
              <table className="table">
                <thead>
                  <tr>
                    <th>Danh mục</th>
                    <th style={{ textAlign: 'right', width: '140px' }}>Số tiền</th>
                    <th style={{ textAlign: 'right', width: '100px' }}>Số GD</th>
                    <th style={{ textAlign: 'right', width: '100px' }}>Tỷ lệ</th>
                  </tr>
                </thead>
                <tbody>
                  {data.incomeByCategory.map((cat, i) => (
                    <tr key={cat.categoryId} className={i === data.incomeByCategory.length - 1 ? 'last' : ''}>
                      <td>
                        <div className="flex items-center gap-2">
                          <span className="category-icon" style={{ background: cat.categoryColor + '20', color: cat.categoryColor, width: '28px', height: '28px', borderRadius: 'var(--radius-sm)', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '0.875rem' }}>
                            {cat.categoryIcon}
                          </span>
                          {cat.categoryName}
                        </div>
                      </td>
                      <td style={{ textAlign: 'right', color: 'var(--accent-primary)', fontWeight: '600' }}>{formatCurrency(cat.amount)}</td>
                      <td style={{ textAlign: 'right' }}>{cat.transactionCount}</td>
                      <td style={{ textAlign: 'right' }}>{formatPercent(cat.percentage)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </CardBody>
        </Card>

        <Card>
          <CardHeader>
            <h3 style={{ fontSize: '1.125rem', fontWeight: '600', display: 'flex', alignItems: 'center', gap: 'var(--space-2)' }}>
              <span style={{ color: 'var(--accent-danger)' }}>📉</span>
              Chi tiêu theo danh mục
            </h3>
          </CardHeader>
          <CardBody style={{ padding: 0 }}>
            <div className="table-container">
              <table className="table">
                <thead>
                  <tr>
                    <th>Danh mục</th>
                    <th style={{ textAlign: 'right', width: '140px' }}>Số tiền</th>
                    <th style={{ textAlign: 'right', width: '100px' }}>Số GD</th>
                    <th style={{ textAlign: 'right', width: '100px' }}>Tỷ lệ</th>
                  </tr>
                </thead>
                <tbody>
                  {data.expenseByCategory.map((cat, i) => (
                    <tr key={cat.categoryId} className={i === data.expenseByCategory.length - 1 ? 'last' : ''}>
                      <td>
                        <div className="flex items-center gap-2">
                          <span className="category-icon" style={{ background: cat.categoryColor + '20', color: cat.categoryColor, width: '28px', height: '28px', borderRadius: 'var(--radius-sm)', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '0.875rem' }}>
                            {cat.categoryIcon}
                          </span>
                          {cat.categoryName}
                        </div>
                      </td>
                      <td style={{ textAlign: 'right', color: 'var(--accent-danger)', fontWeight: '600' }}>{formatCurrency(cat.amount)}</td>
                      <td style={{ textAlign: 'right' }}>{cat.transactionCount}</td>
                      <td style={{ textAlign: 'right' }}>{formatPercent(cat.percentage)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </CardBody>
        </Card>
      </div>
    </>
  );
}

// Yearly View Component
function YearlyView({ data, selectedYear, onYearChange, availableYears }: { data: YearlyReport; selectedYear: number; onYearChange: (y: number) => void; availableYears: number[] }) {
  return (
    <>
      {/* Year Selector */}
      <Card style={{ marginBottom: 'var(--space-6)' }}>
        <CardBody style={{ padding: 'var(--space-4) var(--space-6)' }}>
          <div className="flex items-center justify-between flex-wrap gap-4">
            <div className="flex items-center gap-4">
              <Select
                value={selectedYear.toString()}
                onChange={(e) => onYearChange(parseInt(e.target.value))}
                options={availableYears.map(y => ({ value: y.toString(), label: `Năm ${y}` }))}
                style={{ width: 'auto', minWidth: '140px' }}
              />
              <h3 style={{ fontSize: '1.25rem', fontWeight: '700' }}>Báo cáo năm {selectedYear}</h3>
            </div>
            <div className="flex items-center gap-4">
              <div className="stat-mini"><span style={{ color: 'var(--accent-primary)' }}>Thu: {formatCurrency(data.totalIncome)}</span></div>
              <div className="stat-mini"><span style={{ color: 'var(--accent-danger)' }}>Chi: {formatCurrency(data.totalExpense)}</span></div>
              <div className="stat-mini"><span style={{ color: data.balance >= 0 ? 'var(--accent-primary)' : 'var(--accent-danger)' }}>Dư: {formatCurrency(data.balance)}</span></div>
            </div>
          </div>
        </CardBody>
      </Card>

      {/* Yearly Charts */}
      <div className="grid" style={{ gridTemplateColumns: '1fr 1fr', gap: 'var(--space-6)', marginBottom: 'var(--space-6)' }}>
        {/* Monthly Trend */}
        <Card>
          <CardHeader>
            <h3 style={{ fontSize: '1.125rem', fontWeight: '600' }}>Xu hướng hàng tháng</h3>
          </CardHeader>
          <CardBody>
            <div style={{ height: 300 }}>
              <ResponsiveContainer width="100%" height="100%">
                <ComposedChart data={data.monthlySummaries}>
                  <CartesianGrid strokeDasharray="3 3" stroke="var(--border-primary)" />
                  <XAxis dataKey="monthName" stroke="var(--text-muted)" tickLine={false} axisLine={false} />
                  <YAxis tickFormatter={(value) => formatCurrency(typeof value === 'number' ? value : 0)} stroke="var(--text-muted)" tickLine={false} axisLine={false} />
                  <Tooltip content={<CustomTooltip />} formatter={((value: number | undefined) => formatCurrency(value || 0)) as any} />
                  <Legend />
                  <Bar dataKey="income" name="Thu nhập" fill="var(--accent-primary)" radius={[6, 6, 0, 0]} maxBarSize={30} />
                  <Bar dataKey="expense" name="Chi tiêu" fill="var(--accent-danger)" radius={[6, 6, 0, 0]} maxBarSize={30} />
                  <Line type="monotone" dataKey="balance" name="Số dư" stroke="var(--accent-secondary)" strokeWidth={3} dot={{ r: 4 }} activeDot={{ r: 6 }} />
                </ComposedChart>
              </ResponsiveContainer>
            </div>
          </CardBody>
        </Card>

        {/* Top Expense Categories */}
        <Card>
          <CardHeader>
            <h3 style={{ fontSize: '1.125rem', fontWeight: '600' }}>Top danh mục chi tiêu năm</h3>
          </CardHeader>
          <CardBody>
            <div style={{ height: 300 }}>
              <ResponsiveContainer width="100%" height="100%">
                <BarChart data={data.topExpenseCategories} layout="vertical">
                  <CartesianGrid strokeDasharray="3 3" stroke="var(--border-primary)" vertical={false} />
                  <XAxis type="number" tickFormatter={(value) => formatCurrency(typeof value === 'number' ? value : 0)} stroke="var(--text-muted)" tickLine={false} axisLine={false} />
                  <YAxis dataKey="categoryName" type="category" width={140} stroke="var(--text-muted)" tickLine={false} axisLine={false} />
                  <Tooltip content={<CustomTooltip />} formatter={((value: number | undefined) => formatCurrency(value || 0)) as any} />
                  <Bar dataKey="amount" fill="var(--accent-danger)" radius={[0, 6, 6, 0]} maxBarSize={40}>
                    {data.topExpenseCategories.map((_, index) => (
                      <Cell key={index} fill={COLORS[index % COLORS.length]} />
                    ))}
                  </Bar>
                </BarChart>
              </ResponsiveContainer>
            </div>
          </CardBody>
        </Card>
      </div>

      {/* Monthly Summary Table */}
      <Card>
        <CardHeader>
          <h3 style={{ fontSize: '1.125rem', fontWeight: '600' }}>Chi tiết từng tháng</h3>
        </CardHeader>
        <CardBody style={{ padding: 0 }}>
          <div className="table-container">
            <table className="table">
              <thead>
                <tr>
                  <th style={{ width: '120px' }}>Tháng</th>
                  <th style={{ textAlign: 'right', width: '160px' }}>Thu nhập</th>
                  <th style={{ textAlign: 'right', width: '160px' }}>Chi tiêu</th>
                  <th style={{ textAlign: 'right', width: '160px' }}>Số dư</th>
                  <th style={{ textAlign: 'right', width: '120px' }}>Tiết kiệm</th>
                </tr>
              </thead>
              <tbody>
                {data.monthlySummaries.map((month, i) => (
                  <tr key={month.month} className={i === data.monthlySummaries.length - 1 ? 'last' : ''}>
                    <td style={{ fontWeight: '600' }}>{month.monthName}</td>
                    <td style={{ textAlign: 'right', color: 'var(--accent-primary)' }}>{formatCurrency(month.income)}</td>
                    <td style={{ textAlign: 'right', color: 'var(--accent-danger)' }}>{formatCurrency(month.expense)}</td>
                    <td style={{ textAlign: 'right', fontWeight: '600', color: month.balance >= 0 ? 'var(--accent-primary)' : 'var(--accent-danger)' }}>
                      {formatCurrency(month.balance)}
                    </td>
                    <td style={{ textAlign: 'right', color: month.income > 0 ? 'var(--accent-primary)' : 'var(--text-muted)' }}>
                      {month.income > 0 ? formatPercent(((month.income - month.expense) / month.income) * 100) : '0%'}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </CardBody>
      </Card>

      {/* Top Categories Tables */}
      <div className="grid" style={{ gridTemplateColumns: '1fr 1fr', gap: 'var(--space-6)' }}>
        <Card>
          <CardHeader>
            <h3 style={{ fontSize: '1.125rem', fontWeight: '600' }}>Top thu nhập</h3>
          </CardHeader>
          <CardBody style={{ padding: 0 }}>
            <div className="table-container">
              <table className="table">
                <thead>
                  <tr><th>Danh mục</th><th style={{ textAlign: 'right' }}>Số tiền</th><th style={{ textAlign: 'right' }}>Tỷ lệ</th></tr>
                </thead>
                <tbody>
                  {data.topIncomeCategories.map((cat, i) => (
                    <tr key={cat.categoryId} className={i === data.topIncomeCategories.length - 1 ? 'last' : ''}>
                      <td><div className="flex items-center gap-2"><span className="category-icon" style={{ background: cat.categoryColor + '20', color: cat.categoryColor, width: '28px', height: '28px', borderRadius: 'var(--radius-sm)', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '0.875rem' }}>{cat.categoryIcon}</span>{cat.categoryName}</div></td>
                      <td style={{ textAlign: 'right', color: 'var(--accent-primary)', fontWeight: '600' }}>{formatCurrency(cat.amount)}</td>
                      <td style={{ textAlign: 'right' }}>{formatPercent(cat.percentage)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </CardBody>
        </Card>

        <Card>
          <CardHeader>
            <h3 style={{ fontSize: '1.125rem', fontWeight: '600' }}>Top chi tiêu</h3>
          </CardHeader>
          <CardBody style={{ padding: 0 }}>
            <div className="table-container">
              <table className="table">
                <thead>
                  <tr><th>Danh mục</th><th style={{ textAlign: 'right' }}>Số tiền</th><th style={{ textAlign: 'right' }}>Tỷ lệ</th></tr>
                </thead>
                <tbody>
                  {data.topExpenseCategories.map((cat, i) => (
                    <tr key={cat.categoryId} className={i === data.topExpenseCategories.length - 1 ? 'last' : ''}>
                      <td><div className="flex items-center gap-2"><span className="category-icon" style={{ background: cat.categoryColor + '20', color: cat.categoryColor, width: '28px', height: '28px', borderRadius: 'var(--radius-sm)', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '0.875rem' }}>{cat.categoryIcon}</span>{cat.categoryName}</div></td>
                      <td style={{ textAlign: 'right', color: 'var(--accent-danger)', fontWeight: '600' }}>{formatCurrency(cat.amount)}</td>
                      <td style={{ textAlign: 'right' }}>{formatPercent(cat.percentage)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </CardBody>
        </Card>
      </div>
    </>
  );
}
