import React, { useEffect, useState } from 'react';
import { 
  transactionApi, walletApi, categoryApi,
  Transaction, CreateTransactionDto, UpdateTransactionDto, TransactionFilter, PagedResult,
  Wallet, Category
} from '../services/entities';
import { 
  Card, CardHeader, CardBody, Button, Input, Select, Modal, Badge, 
  EmptyState, Dropdown, DropdownItem, Spinner, formatCurrency, formatDate, formatRelativeTime
} from '../components/UI';
import { Layout } from '../components/Layout';
import toast from 'react-hot-toast';

export function TransactionsPage() {
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [wallets, setWallets] = useState<Wallet[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  
  // Filters & pagination
  const [filter, setFilter] = useState<TransactionFilter>({
    page: 1,
    pageSize: 20,
  });
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  
  // Modals
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState<Transaction | null>(null);
  const [showDeleteModal, setShowDeleteModal] = useState<Transaction | null>(null);
  
  // Forms
  const [createForm, setCreateForm] = useState<CreateTransactionDto>({
    amount: 0,
    note: '',
    transactionDate: new Date().toISOString().split('T')[0],
    isIncome: false,
    walletId: 0,
    categoryId: 0,
  });
  const [editForm, setEditForm] = useState<UpdateTransactionDto>({
    amount: 0,
    note: '',
    transactionDate: '',
    isIncome: false,
    walletId: 0,
    categoryId: 0,
  });
  
  // Loading states
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    loadInitialData();
  }, []);

  useEffect(() => {
    if (wallets.length && categories.length) {
      loadTransactions();
    }
  }, [filter, wallets.length, categories.length]);

  const loadInitialData = async () => {
    try {
      setIsLoading(true);
      const [walletsRes, categoriesRes] = await Promise.all([
        walletApi.getAll(),
        categoryApi.getAll(),
      ]);
      setWallets(walletsRes.data);
      setCategories(categoriesRes.data);
    } catch (err: any) {
      setError('Không thể tải dữ liệu');
      toast.error('Không thể tải dữ liệu');
    } finally {
      setIsLoading(false);
    }
  };

  const loadTransactions = async () => {
    try {
      setIsLoading(true);
      const response = await transactionApi.getAll(filter);
      setTransactions(response.data.items);
      setTotalCount(response.data.totalCount);
      setTotalPages(response.data.totalPages);
    } catch (err: any) {
      toast.error('Không thể tải giao dịch');
    } finally {
      setIsLoading(false);
    }
  };

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    try {
      const payload = { ...createForm, amount: Number(createForm.amount), walletId: Number(createForm.walletId), categoryId: Number(createForm.categoryId) };
      const response = await transactionApi.create(payload);
      setTransactions(prev => [response.data, ...prev]);
      setTotalCount(prev => prev + 1);
      toast.success('Tạo giao dịch thành công');
      setShowCreateModal(false);
      resetCreateForm();
    } catch (err: any) {
      toast.error(err.response?.data?.message || 'Tạo giao dịch thất bại');
    } finally {
      setSubmitting(false);
    }
  };

  const handleUpdate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!showEditModal) return;
    setSubmitting(true);
    try {
      const payload = { ...editForm, amount: Number(editForm.amount), walletId: Number(editForm.walletId), categoryId: Number(editForm.categoryId) };
      const response = await transactionApi.update(showEditModal.id, payload);
      setTransactions(prev => prev.map(t => t.id === showEditModal!.id ? response.data : t));
      toast.success('Cập nhật giao dịch thành công');
      setShowEditModal(null);
    } catch (err: any) {
      toast.error(err.response?.data?.message || 'Cập nhật giao dịch thất bại');
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async () => {
    if (!showDeleteModal) return;
    setSubmitting(true);
    try {
      await transactionApi.delete(showDeleteModal.id);
      setTransactions(prev => prev.filter(t => t.id !== showDeleteModal!.id));
      setTotalCount(prev => prev - 1);
      toast.success('Xóa giao dịch thành công');
      setShowDeleteModal(null);
    } catch (err: any) {
      toast.error(err.response?.data?.message || 'Xóa giao dịch thất bại');
    } finally {
      setSubmitting(false);
    }
  };

  const openEditModal = (tx: Transaction) => {
    setEditForm({
      amount: tx.amount,
      note: tx.note || '',
      transactionDate: tx.transactionDate.split('T')[0],
      isIncome: tx.isIncome,
      walletId: tx.walletId,
      categoryId: tx.categoryId,
    });
    setShowEditModal(tx);
  };

  const resetCreateForm = () => {
    setCreateForm({
      amount: 0,
      note: '',
      transactionDate: new Date().toISOString().split('T')[0],
      isIncome: false,
      walletId: wallets[0]?.id || 0,
      categoryId: 0,
    });
  };

  const incomeCategories = categories.filter(c => c.isIncome && c.isActive);
  const expenseCategories = categories.filter(c => !c.isIncome && c.isActive);
  const activeWallets = wallets.filter(w => w.isActive);

  const handleFilterChange = (key: keyof TransactionFilter, value: any) => {
    setFilter(prev => ({ ...prev, [key]: value, page: 1 }));
  };

  const goToPage = (page: number) => {
    if (page >= 1 && page <= totalPages) {
      setFilter(prev => ({ ...prev, page }));
    }
  };

  if (isLoading) {
    return (
      <Layout>
        <div className="loading-container" style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', minHeight: '400px' }}>
          <Spinner size="lg" />
          <p style={{ marginTop: 'var(--space-4)', color: 'var(--text-secondary)' }}>Đang tải giao dịch...</p>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="page-header">
        <div className="flex items-center justify-between flex-wrap gap-4">
          <div>
            <h1 style={{ fontSize: '1.75rem', fontWeight: '700', marginBottom: 'var(--space-1)' }}>Giao dịch</h1>
            <p style={{ color: 'var(--text-secondary)' }}>Quản lý thu chi chi tiết</p>
          </div>
          <Button variant="primary" leftIcon="➕" onClick={() => setShowCreateModal(true)}>Thêm giao dịch</Button>
        </div>
      </div>

      {/* Filters */}
      <Card>
        <CardBody style={{ padding: 'var(--space-5)' }}>
          <div className="grid grid-4" style={{ gap: 'var(--space-4)', alignItems: 'end' }}>
            <div>
              <label style={{ display: 'block', fontSize: '0.75rem', color: 'var(--text-muted)', marginBottom: 'var(--space-1)' }}>Từ ngày</label>
              <Input
                type="date"
                value={filter.startDate || ''}
                onChange={(e) => handleFilterChange('startDate', e.target.value || undefined)}
              />
            </div>
            <div>
              <label style={{ display: 'block', fontSize: '0.75rem', color: 'var(--text-muted)', marginBottom: 'var(--space-1)' }}>Đến ngày</label>
              <Input
                type="date"
                value={filter.endDate || ''}
                onChange={(e) => handleFilterChange('endDate', e.target.value || undefined)}
              />
            </div>
            <div>
              <label style={{ display: 'block', fontSize: '0.75rem', color: 'var(--text-muted)', marginBottom: 'var(--space-1)' }}>Ví</label>
              <Select
                value={filter.walletId?.toString() || ''}
                onChange={(e) => handleFilterChange('walletId', e.target.value ? parseInt(e.target.value) : undefined)}
                options={[{ value: '', label: 'Tất cả ví' }, ...activeWallets.map(w => ({ value: w.id.toString(), label: w.name }))]}
              />
            </div>
            <div>
              <label style={{ display: 'block', fontSize: '0.75rem', color: 'var(--text-muted)', marginBottom: 'var(--space-1)' }}>Loại</label>
              <Select
                value={filter.isIncome?.toString() || ''}
                onChange={(e) => handleFilterChange('isIncome', e.target.value === '' ? undefined : e.target.value === 'true')}
                options={[
                  { value: '', label: 'Tất cả' },
                  { value: 'true', label: 'Thu nhập' },
                  { value: 'false', label: 'Chi tiêu' },
                ]}
              />
            </div>
          </div>
        </CardBody>
      </Card>

      {/* Transactions Table */}
      <Card>
        <CardHeader>
          <div className="flex items-center justify-between flex-wrap gap-4">
            <h3 style={{ fontSize: '1.125rem', fontWeight: '600' }}>Danh sách giao dịch ({totalCount})</h3>
            <div className="flex items-center gap-2">
              <span style={{ fontSize: '0.875rem', color: 'var(--text-secondary)' }}>
                Trang {filter.page} / {totalPages || 1}
              </span>
              <Button variant="ghost" size="sm" onClick={() => goToPage(filter.page - 1)} disabled={filter.page <= 1}>‹</Button>
              <Button variant="ghost" size="sm" onClick={() => goToPage(filter.page + 1)} disabled={filter.page >= totalPages}›</Button>
            </div>
          </div>
        </CardHeader>
        <CardBody style={{ padding: 0 }}>
          {transactions.length === 0 ? (
            <EmptyState
              icon="📝"
              title="Chưa có giao dịch nào"
              message="Hãy thêm giao dịch đầu tiên hoặc điều chỉnh bộ lọc"
              action={<Button variant="primary" leftIcon="➕" onClick={() => setShowCreateModal(true)}>Thêm giao dịch</Button>}
            />
          ) : (
            <div className="table-container">
              <table className="table">
                <thead>
                  <tr>
                    <th style={{ width: '120px' }}>Ngày</th>
                    <th>Danh mục</th>
                    <th style={{ width: '140px' }}>Ví</th>
                    <th style={{ width: '140px', textAlign: 'right' }}>Số tiền</th>
                    <th style={{ width: '200px' }}>Ghi chú</th>
                    <th style={{ width: '80px' }}>Thao tác</th>
                  </tr>
                </thead>
                <tbody>
                  {transactions.map((tx, index) => (
                    <tr key={tx.id} className={index === transactions.length - 1 ? 'last' : ''}>
                      <td>{formatDate(tx.transactionDate)}</td>
                      <td>
                        <div className="flex items-center gap-2">
                          <span className="category-icon" style={{ background: tx.categoryColor + '20', color: tx.categoryColor, width: '28px', height: '28px', borderRadius: 'var(--radius-sm)', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '0.875rem' }}>
                            {tx.categoryIcon}
                          </span>
                          <span>{tx.categoryName}</span>
                        </div>
                      </td>
                      <td>{tx.walletName}</td>
                      <td style={{ textAlign: 'right', fontWeight: '600', color: tx.isIncome ? 'var(--accent-primary)' : 'var(--accent-danger)' }}>
                        {tx.isIncome ? '+' : '-'}{formatCurrency(tx.amount)}
                      </td>
                      <td style={{ color: 'var(--text-secondary)', maxWidth: '200px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                        {tx.note || '-'}
                      </td>
                      <td>
                        <Dropdown
                          trigger={<Button variant="ghost" size="sm" leftIcon="⋮" />}
                          items={[
                            { label: 'Sửa', onClick: () => openEditModal(tx), icon: '✏️' },
                            { label: 'Xóa', onClick: () => setShowDeleteModal(tx), icon: '🗑️', danger: true },
                          ]}
                        />
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </CardBody>
      </Card>

      {/* Create Modal */}
      <Modal
        isOpen={showCreateModal}
        onClose={() => setShowCreateModal(false)}
        title="Thêm giao dịch mới"
        footer={
          <>
            <Button variant="secondary" onClick={() => setShowCreateModal(false)}>Hủy</Button>
            <Button variant="primary" onClick={handleCreate} loading={submitting}>Tạo giao dịch</Button>
          </>
        }
      >
        <form onSubmit={handleCreate} className="flex flex-col gap-4">
          <div className="grid grid-2" style={{ gap: 'var(--space-4)' }}>
            <Select
              label="Loại"
              value={createForm.isIncome.toString()}
              onChange={(e) => setCreateForm({ ...createForm, isIncome: e.target.value === 'true' })}
              options={[
                { value: 'false', label: '📉 Chi tiêu' },
                { value: 'true', label: '📈 Thu nhập' },
              ]}
            />
            <Input
              label="Số tiền"
              type="number"
              step="1000"
              min="1"
              value={createForm.amount}
              onChange={(e) => setCreateForm({ ...createForm, amount: parseFloat(e.target.value) || 0 })}
              placeholder="Nhập số tiền"
              required
              suffix="VND"
            />
          </div>
          <Input
            label="Ngày giao dịch"
            type="date"
            value={createForm.transactionDate}
            onChange={(e) => setCreateForm({ ...createForm, transactionDate: e.target.value })}
            required
          />
          <Select
            label="Ví"
            value={createForm.walletId.toString()}
            onChange={(e) => setCreateForm({ ...createForm, walletId: parseInt(e.target.value) })}
            options={activeWallets.map(w => ({ value: w.id.toString(), label: `${w.name} (${formatCurrency(w.balance)})` }))}
            placeholder="Chọn ví"
            required
          />
          <Select
            label="Danh mục"
            value={createForm.categoryId.toString()}
            onChange={(e) => setCreateForm({ ...createForm, categoryId: parseInt(e.target.value) })}
            options={createForm.isIncome ? incomeCategories.map(c => ({ value: c.id.toString(), label: `${c.icon} ${c.name}` })) : expenseCategories.map(c => ({ value: c.id.toString(), label: `${c.icon} ${c.name}` }))}
            placeholder="Chọn danh mục"
            required
          />
          <Input
            label="Ghi chú (tùy chọn)"
            value={createForm.note}
            onChange={(e) => setCreateForm({ ...createForm, note: e.target.value })}
            placeholder="Ghi chú thêm..."
          />
        </form>
      </Modal>

      {/* Edit Modal */}
      <Modal
        isOpen={!!showEditModal}
        onClose={() => setShowEditModal(null)}
        title="Chỉnh sửa giao dịch"
        footer={
          <>
            <Button variant="secondary" onClick={() => setShowEditModal(null)}>Hủy</Button>
            <Button variant="primary" onClick={handleUpdate} loading={submitting}>Lưu thay đổi</Button>
          </>
        }
      >
        <form onSubmit={handleUpdate} className="flex flex-col gap-4">
          <div className="grid grid-2" style={{ gap: 'var(--space-4)' }}>
            <Select
              label="Loại"
              value={editForm.isIncome.toString()}
              onChange={(e) => setEditForm({ ...editForm, isIncome: e.target.value === 'true' })}
              options={[
                { value: 'false', label: '📉 Chi tiêu' },
                { value: 'true', label: '📈 Thu nhập' },
              ]}
            />
            <Input
              label="Số tiền"
              type="number"
              step="1000"
              min="1"
              value={editForm.amount}
              onChange={(e) => setEditForm({ ...editForm, amount: parseFloat(e.target.value) || 0 })}
              required
              suffix="VND"
            />
          </div>
          <Input
            label="Ngày giao dịch"
            type="date"
            value={editForm.transactionDate}
            onChange={(e) => setEditForm({ ...editForm, transactionDate: e.target.value })}
            required
          />
          <Select
            label="Ví"
            value={editForm.walletId.toString()}
            onChange={(e) => setEditForm({ ...editForm, walletId: parseInt(e.target.value) })}
            options={activeWallets.map(w => ({ value: w.id.toString(), label: w.name }))}
            required
          />
          <Select
            label="Danh mục"
            value={editForm.categoryId.toString()}
            onChange={(e) => setEditForm({ ...editForm, categoryId: parseInt(e.target.value) })}
            options={editForm.isIncome ? incomeCategories.map(c => ({ value: c.id.toString(), label: `${c.icon} ${c.name}` })) : expenseCategories.map(c => ({ value: c.id.toString(), label: `${c.icon} ${c.name}` }))}
            required
          />
          <Input
            label="Ghi chú"
            value={editForm.note}
            onChange={(e) => setEditForm({ ...editForm, note: e.target.value })}
            placeholder="Ghi chú..."
          />
        </form>
      </Modal>

      {/* Delete Modal */}
      <Modal
        isOpen={!!showDeleteModal}
        onClose={() => setShowDeleteModal(null)}
        title="Xóa giao dịch"
        footer={
          <>
            <Button variant="secondary" onClick={() => setShowDeleteModal(null)}>Hủy</Button>
            <Button variant="danger" onClick={handleDelete} loading={submitting}>Xóa</Button>
          </>
        }
      >
        <p>Bạn có chắc chắn muốn xóa giao dịch này?</p>
        <div style={{ marginTop: 'var(--space-3)', padding: 'var(--space-3)', background: 'var(--bg-tertiary)', borderRadius: 'var(--radius-md)' }}>
          <p><strong>Danh mục:</strong> {showDeleteModal?.categoryName}</p>
          <p><strong>Số tiền:</strong> <span style={{ color: showDeleteModal?.isIncome ? 'var(--accent-primary)' : 'var(--accent-danger)' }}>{showDeleteModal?.isIncome ? '+' : '-'}{formatCurrency(showDeleteModal?.amount || 0)}</span></p>
          <p><strong>Ngày:</strong> {showDeleteModal ? formatDate(showDeleteModal.transactionDate) : ''}</p>
        </div>
      </Modal>
    </Layout>
  );
}
