import { useState, useEffect } from 'react';
import type { Budget, CreateBudgetDto, UpdateBudgetDto, Category, Wallet } from '../services/entities';
import { Card, CardHeader, CardBody, Button, Input, Select, Modal, Badge, EmptyState, Spinner } from '../components/UI';
import { formatCurrency, formatDate } from '../utils/format';
import { Layout } from '../components/Layout';
import toast from 'react-hot-toast';
import { budgetApi, categoryApi, walletApi } from '../services/entities';

export function BudgetsPage() {
  const [budgets, setBudgets] = useState<Budget[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [wallets, setWallets] = useState<Wallet[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  
  // Modals
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState<Budget | null>(null);
  const [showDeleteModal, setShowDeleteModal] = useState<Budget | null>(null);
  
  // Forms
  const [createForm, setCreateForm] = useState<CreateBudgetDto>({
    name: '',
    amount: 0,
    startDate: new Date().toISOString().split('T')[0],
    endDate: new Date(new Date().setMonth(new Date().getMonth() + 1)).toISOString().split('T')[0],
    categoryId: 0,
    walletId: undefined,
  });
  const [editForm, setEditForm] = useState<UpdateBudgetDto>({
    name: '',
    amount: 0,
    startDate: '',
    endDate: '',
    categoryId: 0,
    walletId: undefined,
    isActive: true,
  });
  
  // Loading states
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    loadInitialData();
  }, []);

  const loadInitialData = async () => {
    try {
      setIsLoading(true);
      const [budgetsRes, categoriesRes, walletsRes] = await Promise.allSettled([
        budgetApi.getAll(),
        categoryApi.getAll(false), // Only expense categories for budgets
        walletApi.getAll(),
      ]);
      
      if (budgetsRes.status === 'fulfilled') setBudgets(budgetsRes.value.data);
      if (categoriesRes.status === 'fulfilled') setCategories(categoriesRes.value.data.filter(c => c.isActive));
      if (walletsRes.status === 'fulfilled') setWallets(walletsRes.value.data.filter(w => w.isActive));
    } catch (err: any) {
      toast.error('Không thể tải dữ liệu');
    } finally {
      setIsLoading(false);
    }
  };

  const loadBudgets = async () => {
    try {
      const response = await budgetApi.getAll();
      setBudgets(response.data);
    } catch (err) {
      toast.error('Không thể tải ngân sách');
    }
  };

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    try {
      const payload = { 
        ...createForm, 
        amount: Number(createForm.amount),
        categoryId: Number(createForm.categoryId),
        walletId: createForm.walletId ? Number(createForm.walletId) : undefined,
      };
      const response = await budgetApi.create(payload);
      setBudgets(prev => [...prev, response.data]);
      toast.success('Tạo ngân sách thành công');
      setShowCreateModal(false);
      resetCreateForm();
    } catch (err: any) {
      toast.error(err.response?.data?.message || 'Tạo ngân sách thất bại');
    } finally {
      setSubmitting(false);
    }
  };

  const handleUpdate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!showEditModal) return;
    setSubmitting(true);
    try {
      const payload = { 
        ...editForm, 
        amount: Number(editForm.amount),
        categoryId: Number(editForm.categoryId),
        walletId: editForm.walletId ? Number(editForm.walletId) : undefined,
      };
      const response = await budgetApi.update(showEditModal.id, payload);
      setBudgets(prev => prev.map(b => b.id === showEditModal!.id ? response.data : b));
      toast.success('Cập nhật ngân sách thành công');
      setShowEditModal(null);
    } catch (err: any) {
      toast.error(err.response?.data?.message || 'Cập nhật ngân sách thất bại');
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async () => {
    if (!showDeleteModal) return;
    setSubmitting(true);
    try {
      await budgetApi.delete(showDeleteModal.id);
      setBudgets(prev => prev.filter(b => b.id !== showDeleteModal!.id));
      toast.success('Xóa ngân sách thành công');
      setShowDeleteModal(null);
    } catch (err: any) {
      toast.error(err.response?.data?.message || 'Xóa ngân sách thất bại');
    } finally {
      setSubmitting(false);
    }
  };

  const handleRecalculate = async (budget: Budget) => {
    try {
      const response = await budgetApi.recalculate(budget.id);
      setBudgets(prev => prev.map(b => b.id === budget.id ? response.data : b));
      toast.success('Đã tính lại ngân sách');
    } catch (err: any) {
      toast.error('Không thể tính lại ngân sách');
    }
  };

  const openEditModal = (budget: Budget) => {
    setEditForm({
      name: budget.name,
      amount: budget.amount,
      startDate: budget.startDate.split('T')[0],
      endDate: budget.endDate.split('T')[0],
      categoryId: budget.categoryId,
      walletId: budget.walletId,
      isActive: !budget.isOverBudget, // Approximate
    });
    setShowEditModal(budget);
  };

  const resetCreateForm = () => {
    setCreateForm({
      name: '',
      amount: 0,
      startDate: new Date().toISOString().split('T')[0],
      endDate: new Date(new Date().setMonth(new Date().getMonth() + 1)).toISOString().split('T')[0],
      categoryId: 0,
      walletId: undefined,
    });
  };

  const expenseCategories = categories.filter(c => !c.isIncome);

  const getProgressColor = (percentage: number) => {
    if (percentage >= 100) return 'var(--accent-danger)';
    if (percentage >= 80) return 'var(--accent-warning)';
    return 'var(--accent-primary)';
  };

  const getAlertBadge = (budget: Budget) => {
    if (budget.isOverBudget) return <Badge variant="danger">Vượt ngân sách</Badge>;
    if (budget.isNearLimit) return <Badge variant="warning">Cảnh báo 80%</Badge>;
    return <Badge variant="primary">An toàn</Badge>;
  };

  if (isLoading) {
    return (
      <Layout>
        <div className="loading-container" style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', minHeight: '400px' }}>
          <Spinner size="lg" />
          <p style={{ marginTop: 'var(--space-4)', color: 'var(--text-secondary)' }}>Đang tải ngân sách...</p>
        </div>
      </Layout>
    );
  }

  // Separate budgets by status
  const overBudget = budgets.filter(b => b.isOverBudget);
  const nearLimit = budgets.filter(b => b.isNearLimit && !b.isOverBudget);
  const safe = budgets.filter(b => !b.isNearLimit && !b.isOverBudget);

  return (
    <Layout>
      <div className="page-header">
        <div className="flex items-center justify-between flex-wrap gap-4">
          <div>
            <h1 style={{ fontSize: '1.75rem', fontWeight: '700', marginBottom: 'var(--space-1)' }}>Ngân sách</h1>
            <p style={{ color: 'var(--text-secondary)' }}>Theo dõi chi tiêu theo danh mục, nhận cảnh báo khi sắp vượt hạn mức</p>
          </div>
          <Button variant="primary" leftIcon="➕" onClick={() => setShowCreateModal(true)}>Tạo ngân sách</Button>
        </div>
      </div>

      {/* Alerts Banner */}
      {(overBudget.length > 0 || nearLimit.length > 0) && (
        <Card style={{ marginBottom: 'var(--space-6)', borderColor: overBudget.length > 0 ? 'var(--accent-danger)' : 'var(--accent-warning)' }}>
          <CardBody style={{ padding: 'var(--space-4) var(--space-6)' }}>
            <div className="flex items-center justify-between flex-wrap gap-4">
              <div className="flex items-center gap-3">
                <div style={{ fontSize: '1.5rem' }}>{overBudget.length > 0 ? '🚨' : '⚠️'}</div>
                <div>
                  <p style={{ fontWeight: '600' }}>
                    {overBudget.length > 0 
                      ? `${overBudget.length} ngân sách đã vượt hạn mức!` 
                      : `${nearLimit.length} ngân sách sắp vượt hạn mức (80%)`}
                  </p>
                  <p style={{ fontSize: '0.875rem', color: 'var(--text-secondary)' }}>
                    Hãy xem xét lại chi tiêu hoặc điều chỉnh ngân sách
                  </p>
                </div>
              </div>
              <Button variant="ghost" size="sm" onClick={() => setTimeout(loadBudgets, 500)} leftIcon="🔄">
                Tải lại
              </Button>
            </div>
          </CardBody>
        </Card>
      )}

      {/* Stats */}
      <div className="grid grid-4" style={{ marginBottom: 'var(--space-8)' }}>
        <Card>
          <CardBody style={{ padding: 'var(--space-5)' }}>
            <div className="flex items-center justify-between">
              <div>
                <p style={{ fontSize: '0.875rem', color: 'var(--text-secondary)', marginBottom: 'var(--space-1)' }}>Tổng ngân sách</p>
                <p style={{ fontSize: '1.5rem', fontWeight: '700' }}>{budgets.length}</p>
              </div>
              <div className="avatar avatar-lg" style={{ background: 'var(--gradient-primary)' }}>🎯</div>
            </div>
          </CardBody>
        </Card>
        <Card>
          <CardBody style={{ padding: 'var(--space-5)' }}>
            <div className="flex items-center justify-between">
              <div>
                <p style={{ fontSize: '0.875rem', color: 'var(--text-secondary)', marginBottom: 'var(--space-1)' }}>Đã vượt hạn</p>
                <p style={{ fontSize: '1.5rem', fontWeight: '700', color: 'var(--accent-danger)' }}>{overBudget.length}</p>
              </div>
              <div className="avatar avatar-lg" style={{ background: 'linear-gradient(135deg, #ef4444 0%, #dc2626 100%)' }}>🚨</div>
            </div>
          </CardBody>
        </Card>
        <Card>
          <CardBody style={{ padding: 'var(--space-5)' }}>
            <div className="flex items-center justify-between">
              <div>
                <p style={{ fontSize: '0.875rem', color: 'var(--text-secondary)', marginBottom: 'var(--space-1)' }}>Sắp vượt (80%)</p>
                <p style={{ fontSize: '1.5rem', fontWeight: '700', color: 'var(--accent-warning)' }}>{nearLimit.length}</p>
              </div>
              <div className="avatar avatar-lg" style={{ background: 'linear-gradient(135deg, #f59e0b 0%, #d97706 100%)' }}>⚠️</div>
            </div>
          </CardBody>
        </Card>
        <Card>
          <CardBody style={{ padding: 'var(--space-5)' }}>
            <div className="flex items-center justify-between">
              <div>
                <p style={{ fontSize: '0.875rem', color: 'var(--text-secondary)', marginBottom: 'var(--space-1)' }}>An toàn</p>
                <p style={{ fontSize: '1.5rem', fontWeight: '700', color: 'var(--accent-primary)' }}>{safe.length}</p>
              </div>
              <div className="avatar avatar-lg" style={{ background: 'linear-gradient(135deg, #10b981 0%, #059669 100%)' }}>✅</div>
            </div>
          </CardBody>
        </Card>
      </div>

      {/* Budgets List */}
      <Card>
        <CardHeader>
          <h3 style={{ fontSize: '1.125rem', fontWeight: '600' }}>Danh sách ngân sách</h3>
        </CardHeader>
        <CardBody style={{ padding: 0 }}>
          {budgets.length === 0 ? (
            <EmptyState
              icon="🎯"
              title="Chưa có ngân sách nào"
              message="Tạo ngân sách đầu tiên để bắt đầu theo dõi chi tiêu"
              action={<Button variant="primary" leftIcon="➕" onClick={() => setShowCreateModal(true)}>Tạo ngân sách</Button>}
            />
          ) : (
            <div className="budget-list">
              {budgets.map((budget, index) => (
                <div key={budget.id} className={`budget-item ${index === budgets.length - 1 ? 'last' : ''} ${budget.isOverBudget ? 'over-budget' : budget.isNearLimit ? 'near-limit' : ''}`}>
                  <div className="budget-main">
                    <div className="budget-icon" style={{ background: budget.categoryColor + '20', color: budget.categoryColor }}>
                      {budget.categoryIcon}
                    </div>
                    <div className="budget-info">
                      <div className="budget-name-row">
                        <span className="budget-name">{budget.name}</span>
                        {getAlertBadge(budget)}
                      </div>
                      <div className="budget-meta">
                        <span>{budget.categoryName}</span>
                        {budget.walletName && <span>•</span>}
                        {budget.walletName && <span>{budget.walletName}</span>}
                        <span>•</span>
                        <span>{formatDate(budget.startDate)} - {formatDate(budget.endDate)}</span>
                      </div>
                      <div className="budget-progress-wrapper">
                        <div className="budget-progress-bar">
                          <div 
                            className="budget-progress-fill" 
                            style={{ 
                              width: `${Math.min(budget.percentageUsed, 100)}%`,
                              background: getProgressColor(budget.percentageUsed),
                              transition: 'width 0.3s ease'
                            }} 
                          />
                        </div>
                        <div className="budget-progress-labels">
                          <span>{formatCurrency(budget.spentAmount)} / {formatCurrency(budget.amount)}</span>
                          <span style={{ fontWeight: '600', color: getProgressColor(budget.percentageUsed) }}>
                            {budget.percentageUsed.toFixed(1)}%
                          </span>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div className="budget-actions">
                    <Button variant="ghost" size="sm" leftIcon="🔄" onClick={() => handleRecalculate(budget)}>
                      Tính lại
                    </Button>
                    <Button variant="ghost" size="sm" leftIcon="✏️" onClick={() => openEditModal(budget)}>
                      Sửa
                    </Button>
                    <Button variant="ghost" size="sm" leftIcon="🗑️" onClick={() => setShowDeleteModal(budget)} danger>
                      Xóa
                    </Button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </CardBody>
      </Card>

      {/* Create Modal */}
      <Modal
        isOpen={showCreateModal}
        onClose={() => setShowCreateModal(false)}
        title="Tạo ngân sách mới"
        footer={
          <>
            <Button variant="secondary" onClick={() => setShowCreateModal(false)}>Hủy</Button>
            <Button variant="primary" onClick={handleCreate} loading={submitting}>Tạo ngân sách</Button>
          </>
        }
      >
        <form onSubmit={handleCreate} className="flex flex-col gap-4">
          <Input
            label="Tên ngân sách"
            value={createForm.name}
            onChange={(e) => setCreateForm({ ...createForm, name: e.target.value })}
            placeholder="Ví dụ: Ngân sách ăn uống tháng này"
            required
            autoFocus
          />
          <div className="grid grid-2" style={{ gap: 'var(--space-4)' }}>
            <Input
              label="Số tiền"
              type="number"
              step="10000"
              min="1"
              value={createForm.amount}
              onChange={(e) => setCreateForm({ ...createForm, amount: parseFloat(e.target.value) || 0 })}
              required
              suffix="VND"
            />
            <Select
              label="Danh mục"
              value={createForm.categoryId.toString()}
              onChange={(e) => setCreateForm({ ...createForm, categoryId: parseInt(e.target.value) })}
              options={expenseCategories.map(c => ({ value: c.id.toString(), label: `${c.icon} ${c.name}` }))}
              placeholder="Chọn danh mục chi tiêu"
              required
            />
          </div>
          <div className="grid grid-2" style={{ gap: 'var(--space-4)' }}>
            <Input
              label="Từ ngày"
              type="date"
              value={createForm.startDate}
              onChange={(e) => setCreateForm({ ...createForm, startDate: e.target.value })}
              required
            />
            <Input
              label="Đến ngày"
              type="date"
              value={createForm.endDate}
              onChange={(e) => setCreateForm({ ...createForm, endDate: e.target.value })}
              required
            />
          </div>
          <Select
            label="Ví (tùy chọn - để trống áp dụng cho tất cả ví)"
            value={createForm.walletId?.toString() || ''}
            onChange={(e) => setCreateForm({ ...createForm, walletId: e.target.value ? parseInt(e.target.value) : undefined })}
            options={[{ value: '', label: 'Tất cả ví' }, ...wallets.map(w => ({ value: w.id.toString(), label: w.name }))]}
          />
        </form>
      </Modal>

      {/* Edit Modal */}
      <Modal
        isOpen={!!showEditModal}
        onClose={() => setShowEditModal(null)}
        title="Chỉnh sửa ngân sách"
        footer={
          <>
            <Button variant="secondary" onClick={() => setShowEditModal(null)}>Hủy</Button>
            <Button variant="primary" onClick={handleUpdate} loading={submitting}>Lưu thay đổi</Button>
          </>
        }
      >
        <form onSubmit={handleUpdate} className="flex flex-col gap-4">
          <Input
            label="Tên ngân sách"
            value={editForm.name}
            onChange={(e) => setEditForm({ ...editForm, name: e.target.value })}
            required
          />
          <div className="grid grid-2" style={{ gap: 'var(--space-4)' }}>
            <Input
              label="Số tiền"
              type="number"
              step="10000"
              min="1"
              value={editForm.amount}
              onChange={(e) => setEditForm({ ...editForm, amount: parseFloat(e.target.value) || 0 })}
              required
              suffix="VND"
            />
            <Select
              label="Danh mục"
              value={editForm.categoryId.toString()}
              onChange={(e) => setEditForm({ ...editForm, categoryId: parseInt(e.target.value) })}
              options={expenseCategories.map(c => ({ value: c.id.toString(), label: `${c.icon} ${c.name}` }))}
              required
            />
          </div>
          <div className="grid grid-2" style={{ gap: 'var(--space-4)' }}>
            <Input
              label="Từ ngày"
              type="date"
              value={editForm.startDate}
              onChange={(e) => setEditForm({ ...editForm, startDate: e.target.value })}
              required
            />
            <Input
              label="Đến ngày"
              type="date"
              value={editForm.endDate}
              onChange={(e) => setEditForm({ ...editForm, endDate: e.target.value })}
              required
            />
          </div>
          <Select
            label="Ví"
            value={editForm.walletId?.toString() || ''}
            onChange={(e) => setEditForm({ ...editForm, walletId: e.target.value ? parseInt(e.target.value) : undefined })}
            options={[{ value: '', label: 'Tất cả ví' }, ...wallets.map(w => ({ value: w.id.toString(), label: w.name }))]}
          />
          <label className="checkbox-wrapper">
            <input
              type="checkbox"
              checked={editForm.isActive}
              onChange={(e) => setEditForm({ ...editForm, isActive: e.target.checked })}
            />
            <span>Kích hoạt</span>
          </label>
        </form>
      </Modal>

      {/* Delete Modal */}
      <Modal
        isOpen={!!showDeleteModal}
        onClose={() => setShowDeleteModal(null)}
        title="Xóa ngân sách"
        footer={
          <>
            <Button variant="secondary" onClick={() => setShowDeleteModal(null)}>Hủy</Button>
            <Button variant="danger" onClick={handleDelete} loading={submitting}>Xóa</Button>
          </>
        }
      >
        <p>Bạn có chắc chắn muốn xóa ngân sách <strong>{showDeleteModal?.name}</strong>?</p>
        <p style={{ color: 'var(--text-secondary)', marginTop: 'var(--space-2)', fontSize: '0.875rem' }}>
          Hành động này không thể hoàn tác. Dữ liệu giao dịch không bị ảnh hưởng.
        </p>
      </Modal>
    </Layout>
  );
}
