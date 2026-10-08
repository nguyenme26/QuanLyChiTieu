import React, { useEffect, useState } from 'react';
import { categoryApi, Category, CreateCategoryDto, UpdateCategoryDto } from '../services/entities';
import { 
  Card, CardHeader, CardBody, Button, Input, Select, Modal, Badge, 
  EmptyState, Dropdown, DropdownItem, Avatar 
} from '../components/UI';
import { Layout } from '../components/Layout';
import { formatDate, getCategoryIcon, getCategoryColor } from '../utils/format';
import toast from 'react-hot-toast';

export function CategoriesPage() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [filterIncome, setFilterIncome] = useState<boolean | null>(null);
  
  // Modals
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState<Category | null>(null);
  const [showDeleteModal, setShowDeleteModal] = useState<Category | null>(null);
  
  // Forms
  const [createForm, setCreateForm] = useState<CreateCategoryDto>({
    name: '',
    description: '',
    icon: '',
    color: '#10b981',
    isIncome: false,
  });
  const [editForm, setEditForm] = useState<UpdateCategoryDto>({
    name: '',
    description: '',
    icon: '',
    color: '#10b981',
    isIncome: false,
    isActive: true,
  });
  
  // Loading states
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    loadCategories();
  }, [filterIncome]);

  const loadCategories = async () => {
    try {
      setIsLoading(true);
      const response = await categoryApi.getAll(filterIncome ?? undefined);
      setCategories(response.data);
    } catch (err: any) {
      setError('Không thể tải danh mục');
      toast.error('Không thể tải danh mục');
    } finally {
      setIsLoading(false);
    }
  };

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    try {
      const response = await categoryApi.create(createForm);
      setCategories(prev => [...prev, response.data]);
      toast.success('Tạo danh mục thành công');
      setShowCreateModal(false);
      resetCreateForm();
    } catch (err: any) {
      toast.error(err.response?.data?.message || 'Tạo danh mục thất bại');
    } finally {
      setSubmitting(false);
    }
  };

  const handleUpdate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!showEditModal) return;
    setSubmitting(true);
    try {
      const response = await categoryApi.update(showEditModal.id, editForm);
      setCategories(prev => prev.map(c => c.id === showEditModal!.id ? response.data : c));
      toast.success('Cập nhật danh mục thành công');
      setShowEditModal(null);
    } catch (err: any) {
      toast.error(err.response?.data?.message || 'Cập nhật danh mục thất bại');
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async () => {
    if (!showDeleteModal) return;
    setSubmitting(true);
    try {
      await categoryApi.delete(showDeleteModal.id);
      setCategories(prev => prev.filter(c => c.id !== showDeleteModal!.id));
      toast.success('Xóa danh mục thành công');
      setShowDeleteModal(null);
    } catch (err: any) {
      toast.error(err.response?.data?.message || 'Xóa danh mục thất bại');
    } finally {
      setSubmitting(false);
    }
  };

  const openEditModal = (category: Category) => {
    setEditForm({
      name: category.name,
      description: category.description || '',
      icon: category.icon,
      color: category.color,
      isIncome: category.isIncome,
      isActive: category.isActive,
    });
    setShowEditModal(category);
  };

  const resetCreateForm = () => {
    setCreateForm({ name: '', description: '', icon: '', color: '#10b981', isIncome: false });
  };

  const incomeCategories = categories.filter(c => c.isIncome);
  const expenseCategories = categories.filter(c => !c.isIncome);
  const systemCategories = categories.filter(c => c.isSystem);
  const customCategories = categories.filter(c => !c.isSystem);

  if (isLoading) {
    return (
      <Layout>
        <div className="loading-container" style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', minHeight: '400px' }}>
          <div className="spinner spinner-lg"></div>
          <p style={{ marginTop: 'var(--space-4)', color: 'var(--text-secondary)' }}>Đang tải danh mục...</p>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="page-header">
        <div className="flex items-center justify-between flex-wrap gap-4">
          <div>
            <h1 style={{ fontSize: '1.75rem', fontWeight: '700', marginBottom: 'var(--space-1)' }}>Quản lý danh mục</h1>
            <p style={{ color: 'var(--text-secondary)' }}>Tổ chức thu chi theo danh mục để dễ dàng theo dõi</p>
          </div>
          <div className="flex gap-3">
            <Button 
              variant={filterIncome === null ? 'primary' : 'secondary'} 
              leftIcon="📋" 
              onClick={() => setFilterIncome(null)}
            >
              Tất cả
            </Button>
            <Button 
              variant={filterIncome === true ? 'primary' : 'secondary'} 
              leftIcon="📈" 
              onClick={() => setFilterIncome(true)}
            >
              Thu nhập
            </Button>
            <Button 
              variant={filterIncome === false ? 'primary' : 'secondary'} 
              leftIcon="📉" 
              onClick={() => setFilterIncome(false)}
            >
              Chi tiêu
            </Button>
            <Button variant="primary" leftIcon="➕" onClick={() => setShowCreateModal(true)}>Thêm danh mục</Button>
          </div>
        </div>
      </div>

      {/* Stats */}
      <div className="grid grid-4" style={{ marginBottom: 'var(--space-8)' }}>
        <Card>
          <CardBody style={{ padding: 'var(--space-5)' }}>
            <div className="flex items-center justify-between">
              <div>
                <p style={{ fontSize: '0.875rem', color: 'var(--text-secondary)', marginBottom: 'var(--space-1)' }}>Tổng danh mục</p>
                <p style={{ fontSize: '1.5rem', fontWeight: '700' }}>{categories.length}</p>
              </div>
              <Avatar size="lg" style={{ background: 'var(--gradient-primary)' }}>📁</Avatar>
            </div>
          </CardBody>
        </Card>
        <Card>
          <CardBody style={{ padding: 'var(--space-5)' }}>
            <div className="flex items-center justify-between">
              <div>
                <p style={{ fontSize: '0.875rem', color: 'var(--text-secondary)', marginBottom: 'var(--space-1)' }}>Danh mục thu</p>
                <p style={{ fontSize: '1.5rem', fontWeight: '700', color: 'var(--accent-primary)' }}>{incomeCategories.length}</p>
              </div>
              <Avatar size="lg" style={{ background: 'linear-gradient(135deg, #10b981 0%, #059669 100%)' }}>📈</Avatar>
            </div>
          </CardBody>
        </Card>
        <Card>
          <CardBody style={{ padding: 'var(--space-5)' }}>
            <div className="flex items-center justify-between">
              <div>
                <p style={{ fontSize: '0.875rem', color: 'var(--text-secondary)', marginBottom: 'var(--space-1)' }}>Danh mục chi</p>
                <p style={{ fontSize: '1.5rem', fontWeight: '700', color: 'var(--accent-danger)' }}>{expenseCategories.length}</p>
              </div>
              <Avatar size="lg" style={{ background: 'linear-gradient(135deg, #ef4444 0%, #dc2626 100%)' }}>📉</Avatar>
            </div>
          </CardBody>
        </Card>
        <Card>
          <CardBody style={{ padding: 'var(--space-5)' }}>
            <div className="flex items-center justify-between">
              <div>
                <p style={{ fontSize: '0.875rem', color: 'var(--text-secondary)', marginBottom: 'var(--space-1)' }}>Tự tạo</p>
                <p style={{ fontSize: '1.5rem', fontWeight: '700' }}>{customCategories.length}</p>
              </div>
              <Avatar size="lg" style={{ background: 'linear-gradient(135deg, #8b5cf6 0%, #7c3aed 100%)' }}>✨</Avatar>
            </div>
          </CardBody>
        </Card>
      </div>

      {/* Categories Tabs */}
      <Card>
        <CardHeader>
          <div className="flex items-center justify-between flex-wrap gap-4">
            <h3 style={{ fontSize: '1.125rem', fontWeight: '600' }}>Danh sách danh mục</h3>
            <div className="flex items-center gap-2">
              <span style={{ fontSize: '0.875rem', color: 'var(--text-muted)' }}>Hệ thống:</span>
              <Badge variant="gray" style={{ fontSize: '0.75rem' }}>{systemCategories.length}</Badge>
              <span style={{ fontSize: '0.875rem', color: 'var(--text-muted)' }}>Tùy chỉnh:</span>
              <Badge variant="primary" style={{ fontSize: '0.75rem' }}>{customCategories.length}</Badge>
            </div>
          </div>
        </CardHeader>
        <CardBody style={{ padding: 0 }}>
          {categories.length === 0 ? (
            <EmptyState
              icon="📁"
              title="Chưa có danh mục nào"
              message={filterIncome === null ? 'Tạo danh mục đầu tiên để bắt đầu phân loại giao dịch' : `Không có danh mục ${filterIncome ? 'thu' : 'chi'} nào`}
              action={<Button variant="primary" leftIcon="➕" onClick={() => setShowCreateModal(true)}>Tạo danh mục</Button>}
            />
          ) : (
            <div className="category-list">
              {categories.map((category, index) => (
                <div key={category.id} className={`category-item ${index === categories.length - 1 ? 'last' : ''}`}>
                  <div className="category-icon" style={{ background: category.color + '20', color: category.color }}>
                    {category.icon || getCategoryIcon(category.name)}
                  </div>
                  <div className="category-details">
                    <div className="category-name-row">
                      <span className="category-name" style={{ fontWeight: '600' }}>{category.name}</span>
                      {category.isSystem && <Badge variant="gray" style={{ fontSize: '0.625rem', marginLeft: 'var(--space-2)' }}>Hệ thống</Badge>}
                      {!category.isActive && <Badge variant="gray" style={{ fontSize: '0.625rem', marginLeft: 'var(--space-2)' }}>Ẩn</Badge>}
                    </div>
                    <div className="category-meta">
                      <Badge variant={category.isIncome ? 'primary' : 'danger'} style={{ fontSize: '0.625rem' }}>
                        {category.isIncome ? '📈 Thu nhập' : '📉 Chi tiêu'}
                      </Badge>
                      {category.description && (
                        <span style={{ fontSize: '0.75rem', color: 'var(--text-muted)' }}>{category.description}</span>
                      )}
                    </div>
                  </div>
                  <div className="category-actions">
                    {!category.isSystem && (
                      <>
                        <Button variant="ghost" size="sm" leftIcon="✏️" onClick={() => openEditModal(category)}>Sửa</Button>
                        <Button variant="ghost" size="sm" leftIcon="🗑️" onClick={() => setShowDeleteModal(category)} danger>Xóa</Button>
                      </>
                    )}
                    {category.isSystem && (
                      <Button variant="ghost" size="sm" leftIcon="👁️" disabled>
                        {category.isActive ? 'Hiển thị' : 'Ẩn'}
                      </Button>
                    )}
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
        title="Tạo danh mục mới"
        footer={
          <>
            <Button variant="secondary" onClick={() => setShowCreateModal(false)}>Hủy</Button>
            <Button variant="primary" onClick={handleCreate} loading={submitting}>Tạo danh mục</Button>
          </>
        }
      >
        <form onSubmit={handleCreate} className="flex flex-col gap-4">
          <Input
            label="Tên danh mục"
            value={createForm.name}
            onChange={(e) => setCreateForm({ ...createForm, name: e.target.value })}
            placeholder="Ví dụ: Ăn uống, Lương, Mua sắm..."
            required
            autoFocus
          />
          <Input
            label="Mô tả (tùy chọn)"
            value={createForm.description}
            onChange={(e) => setCreateForm({ ...createForm, description: e.target.value })}
            placeholder="Mô tả ngắn về danh mục"
          />
          <div className="grid grid-2" style={{ gap: 'var(--space-4)' }}>
            <Input
              label="Icon (emoji)"
              value={createForm.icon}
              onChange={(e) => setCreateForm({ ...createForm, icon: e.target.value })}
              placeholder="🍔, 💰, 🚌..."
              maxLength={2}
            />
            <Input
              label="Màu sắc"
              type="color"
              value={createForm.color}
              onChange={(e) => setCreateForm({ ...createForm, color: e.target.value })}
              style={{ height: '44px', padding: 'var(--space-1)' }}
            />
          </div>
          <div className="flex items-center gap-4">
            <label className="checkbox-wrapper" style={{ flex: 1 }}>
              <input
                type="checkbox"
                checked={createForm.isIncome}
                onChange={(e) => setCreateForm({ ...createForm, isIncome: e.target.checked })}
              />
              <span>Đây là danh mục thu nhập</span>
            </label>
          </div>
        </form>
      </Modal>

      {/* Edit Modal */}
      <Modal
        isOpen={!!showEditModal}
        onClose={() => setShowEditModal(null)}
        title="Chỉnh sửa danh mục"
        footer={
          <>
            <Button variant="secondary" onClick={() => setShowEditModal(null)}>Hủy</Button>
            <Button variant="primary" onClick={handleUpdate} loading={submitting}>Lưu thay đổi</Button>
          </>
        }
      >
        <form onSubmit={handleUpdate} className="flex flex-col gap-4">
          <Input
            label="Tên danh mục"
            value={editForm.name}
            onChange={(e) => setEditForm({ ...editForm, name: e.target.value })}
            required
            disabled={showEditModal?.isSystem}
          />
          <Input
            label="Mô tả"
            value={editForm.description}
            onChange={(e) => setEditForm({ ...editForm, description: e.target.value })}
            disabled={showEditModal?.isSystem}
          />
          <div className="grid grid-2" style={{ gap: 'var(--space-4)' }}>
            <Input
              label="Icon (emoji)"
              value={editForm.icon}
              onChange={(e) => setEditForm({ ...editForm, icon: e.target.value })}
              placeholder="🍔, 💰, 🚌..."
              maxLength={2}
              disabled={showEditModal?.isSystem}
            />
            <Input
              label="Màu sắc"
              type="color"
              value={editForm.color}
              onChange={(e) => setEditForm({ ...editForm, color: e.target.value })}
              style={{ height: '44px', padding: 'var(--space-1)' }}
              disabled={showEditModal?.isSystem}
            />
          </div>
          <div className="flex flex-col gap-3">
            <label className="checkbox-wrapper" style={{ flex: 1 }}>
              <input
                type="checkbox"
                checked={editForm.isIncome}
                onChange={(e) => setEditForm({ ...editForm, isIncome: e.target.checked })}
                disabled={showEditModal?.isSystem}
              />
              <span>Đây là danh mục thu nhập</span>
            </label>
            <label className="checkbox-wrapper" style={{ flex: 1 }}>
              <input
                type="checkbox"
                checked={editForm.isActive}
                onChange={(e) => setEditForm({ ...editForm, isActive: e.target.checked })}
                disabled={showEditModal?.isSystem}
              />
              <span>Kích hoạt</span>
            </label>
          </div>
          {showEditModal?.isSystem && (
            <p style={{ fontSize: '0.75rem', color: 'var(--accent-warning)' }}>⚠ Danh mục hệ thống không thể thay đổi loại (thu/chi)</p>
          )}
        </form>
      </Modal>

      {/* Delete Modal */}
      <Modal
        isOpen={!!showDeleteModal}
        onClose={() => setShowDeleteModal(null)}
        title="Xóa danh mục"
        footer={
          <>
            <Button variant="secondary" onClick={() => setShowDeleteModal(null)}>Hủy</Button>
            <Button variant="danger" onClick={handleDelete} loading={submitting}>Xóa</Button>
          </>
        }
      >
        <p>Bạn có chắc chắn muốn xóa danh mục <strong>{showDeleteModal?.name}</strong>?</p>
        {showDeleteModal?.isSystem ? (
          <p style={{ color: 'var(--accent-warning)', marginTop: 'var(--space-2)', fontSize: '0.875rem' }}>
            ⚠ Đây là danh mục hệ thống. Việc xóa sẽ chỉ ẩn nó (soft delete).
          </p>
        ) : (
          <p style={{ color: 'var(--text-secondary)', marginTop: 'var(--space-2)', fontSize: '0.875rem' }}>
            Hành động này không thể hoàn tác.
          </p>
        )}
      </Modal>
    </Layout>
  );
}
