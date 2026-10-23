import { useEffect, useState } from 'react';
import type { Wallet, CreateWalletDto, UpdateWalletDto, TransferDto } from '../services/entities';
import { walletApi } from '../services/entities';
import { Card, CardHeader, CardBody, Button, Input, Select, Modal, Badge, EmptyState, Avatar, Dropdown, Spinner } from '../components/UI';
import { Layout } from '../components/Layout';
import { formatCurrency, formatDate } from '../utils/format';
import toast from 'react-hot-toast';

export function WalletsPage() {
  const [wallets, setWallets] = useState<Wallet[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  
  // Modals
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState<Wallet | null>(null);
  const [showTransferModal, setShowTransferModal] = useState(false);
  const [showDeleteModal, setShowDeleteModal] = useState<Wallet | null>(null);
  const [showBalanceModal, setShowBalanceModal] = useState<Wallet | null>(null);
  
  // Forms
  const [createForm, setCreateForm] = useState<CreateWalletDto>({
    name: '',
    description: '',
    currency: 'VND',
    isDefault: false,
  });
  const [editForm, setEditForm] = useState<UpdateWalletDto>({
    name: '',
    description: '',
    currency: 'VND',
    isDefault: false,
    isActive: true,
  });
  const [transferForm, setTransferForm] = useState<TransferDto>({
    fromWalletId: 0,
    toWalletId: 0,
    amount: 0,
    note: '',
  });
  
  // Loading states
  const [submitting, setSubmitting] = useState(false);
  const [transferring, setTransferring] = useState(false);

  useEffect(() => {
    loadWallets();
  }, []);

  const loadWallets = async () => {
    try {
      setIsLoading(true);
      const response = await walletApi.getAll();
      setWallets(response.data);
    } catch (err: any) {
      setError('Không thể tải danh sách ví');
      toast.error('Không thể tải danh sách ví');
    } finally {
      setIsLoading(false);
    }
  };

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    try {
      const response = await walletApi.create(createForm);
      setWallets(prev => [...prev, response.data]);
      toast.success('Tạo ví thành công');
      setShowCreateModal(false);
      resetCreateForm();
    } catch (err: any) {
      toast.error(err.response?.data?.message || 'Tạo ví thất bại');
    } finally {
      setSubmitting(false);
    }
  };

  const handleUpdate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!showEditModal) return;
    setSubmitting(true);
    try {
      const response = await walletApi.update(showEditModal.id, editForm);
      setWallets(prev => prev.map(w => w.id === showEditModal.id ? response.data : w));
      toast.success('Cập nhật ví thành công');
      setShowEditModal(null);
    } catch (err: any) {
      toast.error(err.response?.data?.message || 'Cập nhật ví thất bại');
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async () => {
    if (!showDeleteModal) return;
    setSubmitting(true);
    try {
      await walletApi.delete(showDeleteModal.id);
      setWallets(prev => prev.filter(w => w.id !== showDeleteModal!.id));
      toast.success('Xóa ví thành công');
      setShowDeleteModal(null);
    } catch (err: any) {
      toast.error(err.response?.data?.message || 'Xóa ví thất bại');
    } finally {
      setSubmitting(false);
    }
  };

  const handleSetDefault = async (wallet: Wallet) => {
    try {
      await walletApi.setDefault(wallet.id);
      setWallets(prev => prev.map(w => ({
        ...w,
        isDefault: w.id === wallet.id
      })));
      toast.success('Đã đặt làm ví mặc định');
    } catch (err: any) {
      toast.error('Không thể đặt ví mặc định');
    }
  };

  const handleTransfer = async (e: React.FormEvent) => {
    e.preventDefault();
    if (transferForm.fromWalletId === transferForm.toWalletId) {
      toast.error('Vui lòng chọn ví khác nhau');
      return;
    }
    if (transferForm.amount <= 0) {
      toast.error('Số tiền phải lớn hơn 0');
      return;
    }
    
    const fromWallet = wallets.find(w => w.id === transferForm.fromWalletId);
    if (fromWallet && fromWallet.balance < transferForm.amount) {
      toast.error('Số dư không đủ');
      return;
    }

    setTransferring(true);
    try {
      await walletApi.transfer(transferForm);
      toast.success('Chuyển tiền thành công');
      setShowTransferModal(false);
      resetTransferForm();
      loadWallets(); // Reload to get updated balances
    } catch (err: any) {
      toast.error(err.response?.data?.message || 'Chuyển tiền thất bại');
    } finally {
      setTransferring(false);
    }
  };

  const openEditModal = (wallet: Wallet) => {
    setEditForm({
      name: wallet.name,
      description: wallet.description || '',
      currency: wallet.currency,
      isDefault: wallet.isDefault,
      isActive: wallet.isActive,
    });
    setShowEditModal(wallet);
  };

  const openTransferModal = (fromWallet?: Wallet) => {
    setTransferForm({
      fromWalletId: fromWallet?.id || 0,
      toWalletId: 0,
      amount: 0,
      note: '',
    });
    setShowTransferModal(true);
  };

  const resetCreateForm = () => {
    setCreateForm({ name: '', description: '', currency: 'VND', isDefault: false });
  };

  const resetTransferForm = () => {
    setTransferForm({ fromWalletId: 0, toWalletId: 0, amount: 0, note: '' });
  };

  const currencyOptions = [
    { value: 'VND', label: 'VND (₫)' },
    { value: 'USD', label: 'USD ($)' },
    { value: 'EUR', label: 'EUR (€)' },
  ];

  if (isLoading) {
    return (
      <Layout>
        <div className="loading-container">
          <Spinner size="lg" />
          <p style={{ marginTop: 'var(--space-4)', color: 'var(--text-secondary)' }}>Đang tải ví...</p>
        </div>
      </Layout>
    );
  }

  const totalBalance = wallets.reduce((sum, w) => sum + w.balance, 0);
  const defaultWallet = wallets.find(w => w.isDefault);

  return (
    <Layout>
      <div className="page-header">
        <div className="flex items-center justify-between flex-wrap gap-4">
          <div>
            <h1 style={{ fontSize: '1.75rem', fontWeight: '700', marginBottom: 'var(--space-1)' }}>Quản lý ví</h1>
            <p style={{ color: 'var(--text-secondary)' }}>Quản lý các ví tiền mặt, ngân hàng và ví điện tử</p>
          </div>
          <div className="flex gap-3">
            <Button variant="secondary" leftIcon="🔄" onClick={loadWallets} disabled={isLoading}>Làm mới</Button>
            <Button variant="primary" leftIcon="➕" onClick={() => setShowCreateModal(true)}>Thêm ví mới</Button>
          </div>
        </div>
      </div>

      {/* Summary Cards */}
      <div className="grid grid-4" style={{ marginBottom: 'var(--space-8)' }}>
        <Card>
          <CardBody style={{ padding: 'var(--space-5)' }}>
            <div className="flex items-center justify-between">
              <div>
                <p style={{ fontSize: '0.875rem', color: 'var(--text-secondary)', marginBottom: 'var(--space-1)' }}>Tổng số dư</p>
                <p style={{ fontSize: '1.5rem', fontWeight: '700', color: totalBalance >= 0 ? 'var(--accent-primary)' : 'var(--accent-danger)' }}>
                  {formatCurrency(totalBalance)}
                </p>
              </div>
              <Avatar size="lg" style={{ background: 'var(--gradient-primary)' }}>💰</Avatar>
            </div>
          </CardBody>
        </Card>
        <Card>
          <CardBody style={{ padding: 'var(--space-5)' }}>
            <div className="flex items-center justify-between">
              <div>
                <p style={{ fontSize: '0.875rem', color: 'var(--text-secondary)', marginBottom: 'var(--space-1)' }}>Số lượng ví</p>
                <p style={{ fontSize: '1.5rem', fontWeight: '700' }}>{wallets.length}</p>
              </div>
              <Avatar size="lg" style={{ background: 'var(--gradient-secondary)' }}>👛</Avatar>
            </div>
          </CardBody>
        </Card>
        <Card>
          <CardBody style={{ padding: 'var(--space-5)' }}>
            <div className="flex items-center justify-between">
              <div>
                <p style={{ fontSize: '0.875rem', color: 'var(--text-secondary)', marginBottom: 'var(--space-1)' }}>Ví mặc định</p>
                <p style={{ fontSize: '1.5rem', fontWeight: '700' }}>{defaultWallet?.name || 'Chưa có'}</p>
              </div>
              <Avatar size="lg" style={{ background: 'linear-gradient(135deg, #f59e0b 0%, #d97706 100%)' }}>⭐</Avatar>
            </div>
          </CardBody>
        </Card>
        <Card>
          <CardBody style={{ padding: 'var(--space-5)' }}>
            <div className="flex items-center justify-between">
              <div>
                <p style={{ fontSize: '0.875rem', color: 'var(--text-secondary)', marginBottom: 'var(--space-1)' }}>Loại tiền tệ</p>
                <p style={{ fontSize: '1.5rem', fontWeight: '700' }}>
                  {new Set(wallets.map(w => w.currency)).size} loại
                </p>
              </div>
              <Avatar size="lg" style={{ background: 'linear-gradient(135deg, #8b5cf6 0%, #7c3aed 100%)' }}>💱</Avatar>
            </div>
          </CardBody>
        </Card>
      </div>

      {/* Wallets List */}
      <Card>
        <CardHeader>
          <h3 style={{ fontSize: '1.125rem', fontWeight: '600' }}>Danh sách ví</h3>
        </CardHeader>
        <CardBody style={{ padding: 0 }}>
          {wallets.length === 0 ? (
            <EmptyState
              icon="👛"
              title="Chưa có ví nào"
              message="Tạo ví đầu tiên để bắt đầu quản lý tài chính"
              action={<Button variant="primary" leftIcon="➕" onClick={() => setShowCreateModal(true)}>Tạo ví đầu tiên</Button>}
            />
          ) : (
            <div className="wallet-grid">
              {wallets.map((wallet) => (
                <div key={wallet.id} className="wallet-card">
                  <div className="wallet-card-header">
                    <div className="flex items-center gap-3">
                      <div className={`wallet-icon ${wallet.isDefault ? 'default' : ''}`}>
                        {wallet.isDefault ? '⭐' : '👛'}
                      </div>
                      <div>
                        <div className="wallet-card-name">
                          {wallet.name}
                          {wallet.isDefault && <Badge variant="primary" style={{ fontSize: '0.625rem', marginLeft: 'var(--space-2)' }}>Mặc định</Badge>}
                          {!wallet.isActive && <Badge variant="gray" style={{ fontSize: '0.625rem', marginLeft: 'var(--space-2)' }}>Ẩn</Badge>}
                        </div>
                        <div className="wallet-card-currency">{wallet.currency}</div>
                      </div>
                    </div>
                    <div className="wallet-card-balance" style={{ color: wallet.balance >= 0 ? 'var(--text-primary)' : 'var(--accent-danger)' }}>
                      {formatCurrency(wallet.balance)}
                    </div>
                  </div>
                  {wallet.description && (
                    <p className="wallet-card-desc">{wallet.description}</p>
                  )}
                  <div className="wallet-card-actions">
                    <Button variant="ghost" size="sm" leftIcon="💰" onClick={() => openTransferModal(wallet)}>
                      Chuyển tiền
                    </Button>
                    <Button variant="ghost" size="sm" leftIcon="✏️" onClick={() => openEditModal(wallet)}>
                      Sửa
                    </Button>
                    {!wallet.isDefault && (
                      <Button variant="ghost" size="sm" leftIcon="⭐" onClick={() => handleSetDefault(wallet)}>
                        Đặt mặc định
                      </Button>
                    )}
                    <Dropdown
                      trigger={<Button variant="ghost" size="sm" leftIcon="⋮" />}
                      items={[
                        { label: 'Xem số dư chi tiết', onClick: () => setShowBalanceModal(wallet), icon: '📊' },
                        { label: 'Xóa ví', onClick: () => setShowDeleteModal(wallet), icon: '🗑️', danger: true },
                      ]}
                    />
                  </div>
                </div>
              ))}
            </div>
          )}
        </CardBody>
      </Card>

      {/* Modals */}
      <Modal
        isOpen={showCreateModal}
        onClose={() => setShowCreateModal(false)}
        title="Tạo ví mới"
        footer={
          <>
            <Button variant="secondary" onClick={() => setShowCreateModal(false)}>Hủy</Button>
            <Button variant="primary" onClick={handleCreate} loading={submitting}>Tạo ví</Button>
          </>
        }
      >
        <form onSubmit={handleCreate} className="flex flex-col gap-4">
          <Input
            label="Tên ví"
            value={createForm.name}
            onChange={(e) => setCreateForm({ ...createForm, name: e.target.value })}
            placeholder="Ví chính, Ví tiết kiệm, Momo..."
            required
            autoFocus
          />
          <Input
            label="Mô tả (tùy chọn)"
            value={createForm.description}
            onChange={(e) => setCreateForm({ ...createForm, description: e.target.value })}
            placeholder="Mô tả về ví này"
          />
          <div className="grid grid-2" style={{ gap: 'var(--space-4)' }}>
            <Select
              label="Loại tiền tệ"
              value={createForm.currency}
              onChange={(e) => setCreateForm({ ...createForm, currency: e.target.value })}
              options={currencyOptions}
            />
            <div className="flex items-end">
              <label className="checkbox-wrapper" style={{ width: '100%' }}>
                <input
                  type="checkbox"
                  checked={createForm.isDefault}
                  onChange={(e) => setCreateForm({ ...createForm, isDefault: e.target.checked })}
                />
                <span>Đặt làm ví mặc định</span>
              </label>
            </div>
          </div>
        </form>
      </Modal>

      <Modal
        isOpen={!!showEditModal}
        onClose={() => setShowEditModal(null)}
        title="Chỉnh sửa ví"
        footer={
          <>
            <Button variant="secondary" onClick={() => setShowEditModal(null)}>Hủy</Button>
            <Button variant="primary" onClick={handleUpdate} loading={submitting}>Lưu thay đổi</Button>
          </>
        }
      >
        <form onSubmit={handleUpdate} className="flex flex-col gap-4">
          <Input
            label="Tên ví"
            value={editForm.name}
            onChange={(e) => setEditForm({ ...editForm, name: e.target.value })}
            required
          />
          <Input
            label="Mô tả"
            value={editForm.description}
            onChange={(e) => setEditForm({ ...editForm, description: e.target.value })}
          />
          <div className="grid grid-2" style={{ gap: 'var(--space-4)' }}>
            <Select
              label="Loại tiền tệ"
              value={editForm.currency}
              onChange={(e) => setEditForm({ ...editForm, currency: e.target.value })}
              options={currencyOptions}
            />
            <div className="flex flex-col gap-2">
              <label className="checkbox-wrapper">
                <input
                  type="checkbox"
                  checked={editForm.isDefault}
                  onChange={(e) => setEditForm({ ...editForm, isDefault: e.target.checked })}
                />
                <span>Đặt làm ví mặc định</span>
              </label>
              <label className="checkbox-wrapper">
                <input
                  type="checkbox"
                  checked={editForm.isActive}
                  onChange={(e) => setEditForm({ ...editForm, isActive: e.target.checked })}
                />
                <span>Kích hoạt</span>
              </label>
            </div>
          </div>
        </form>
      </Modal>

      <Modal
        isOpen={showTransferModal}
        onClose={() => setShowTransferModal(false)}
        title="Chuyển tiền giữa các ví"
        footer={
          <>
            <Button variant="secondary" onClick={() => setShowTransferModal(false)}>Hủy</Button>
            <Button variant="primary" onClick={handleTransfer} loading={transferring}>Chuyển tiền</Button>
          </>
        }
      >
        <form onSubmit={handleTransfer} className="flex flex-col gap-4">
          <Select
            label="Từ ví"
            value={transferForm.fromWalletId.toString()}
            onChange={(e) => setTransferForm({ ...transferForm, fromWalletId: parseInt(e.target.value) })}
            options={wallets.map(w => ({ value: w.id.toString(), label: `${w.name} (${formatCurrency(w.balance)})` }))}
            placeholder="Chọn ví nguồn"
            required
          />
          <Select
            label="Đến ví"
            value={transferForm.toWalletId.toString()}
            onChange={(e) => setTransferForm({ ...transferForm, toWalletId: parseInt(e.target.value) })}
            options={wallets.map(w => ({ value: w.id.toString(), label: w.name }))}
            placeholder="Chọn ví đích"
            required
          />
          <Input
            label="Số tiền"
            type="number"
            value={transferForm.amount}
            onChange={(e) => setTransferForm({ ...transferForm, amount: parseFloat(e.target.value) || 0 })}
            placeholder="Nhập số tiền"
            required
            suffix="VND"
          />
          <Input
            label="Ghi chú (tùy chọn)"
            value={transferForm.note}
            onChange={(e) => setTransferForm({ ...transferForm, note: e.target.value })}
            placeholder="Ghi chú cho giao dịch chuyển tiền"
          />
        </form>
      </Modal>

      <Modal
        isOpen={!!showDeleteModal}
        onClose={() => setShowDeleteModal(null)}
        title="Xóa ví"
        footer={
          <>
            <Button variant="secondary" onClick={() => setShowDeleteModal(null)}>Hủy</Button>
            <Button variant="danger" onClick={handleDelete} loading={submitting}>Xóa</Button>
          </>
        }
      >
        <p>Bạn có chắc chắn muốn xóa ví <strong>{showDeleteModal?.name}</strong>?</p>
        <p style={{ color: 'var(--text-secondary)', marginTop: 'var(--space-2)', fontSize: '0.875rem' }}>
          Hành động này không thể hoàn tác. Nếu ví có giao dịch, nó sẽ được ẩn thay vì xóa hoàn toàn.
        </p>
      </Modal>

      <Modal
        isOpen={!!showBalanceModal}
        onClose={() => setShowBalanceModal(null)}
        title={`Số dư chi tiết: ${showBalanceModal?.name}`}
        size="sm"
      >
        <div className="space-y-4">
          <div className="flex justify-between items-center p-4 bg-gradient-to-r from-emerald-600/20 to-emerald-600/10 rounded-lg border border-emerald-600/30">
            <span style={{ fontSize: '0.875rem', color: 'var(--text-secondary)' }}>Số dư hiện tại</span>
            <span style={{ fontSize: '1.5rem', fontWeight: '700', color: 'var(--accent-primary)' }}>
              {formatCurrency(showBalanceModal?.balance || 0)}
            </span>
          </div>
          <div className="grid grid-2 gap-4 text-center">
            <div className="p-4 bg-emerald-600/10 border border-emerald-600/20 rounded-lg">
              <p style={{ fontSize: '0.75rem', color: 'var(--text-muted)', textTransform: 'uppercase', marginBottom: 'var(--space-1)' }}>Loại tiền tệ</p>
              <p style={{ fontWeight: '600' }}>{showBalanceModal?.currency}</p>
            </div>
            <div className="p-4 bg-blue-600/10 border border-blue-600/20 rounded-lg">
              <p style={{ fontSize: '0.75rem', color: 'var(--text-muted)', textTransform: 'uppercase', marginBottom: 'var(--space-1)' }}>Trạng thái</p>
              <Badge variant={showBalanceModal?.isActive ? 'primary' : 'gray'}>
                {showBalanceModal?.isActive ? 'Đang hoạt động' : 'Đã ẩn'}
              </Badge>
            </div>
          </div>
          <div className="text-center">
            <p style={{ fontSize: '0.875rem', color: 'var(--text-muted)' }}>
              Tạo ngày: {showBalanceModal ? formatDate(showBalanceModal.createdAt) : ''}
            </p>
          </div>
        </div>
      </Modal>
    </Layout>
  );
}
