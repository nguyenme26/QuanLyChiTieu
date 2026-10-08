import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { Button, Input, Card, CardBody, CardHeader } from '../components/UI';

export function LoginPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [errors, setErrors] = useState<{ email?: string; password?: string }>({});
  const [isLoading, setIsLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const validate = () => {
    const newErrors: { email?: string; password?: string } = {};
    if (!email) newErrors.email = 'Email là bắt buộc';
    else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) newErrors.email = 'Email không hợp lệ';
    if (!password) newErrors.password = 'Mật khẩu là bắt buộc';
    else if (password.length < 6) newErrors.password = 'Mật khẩu tối thiểu 6 ký tự';
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;
    setIsLoading(true);
    const success = await login(email, password);
    if (success) navigate('/dashboard');
    setIsLoading(false);
  };

  return (
    <div className="auth-page">
      <div className="auth-container">
        <div className="auth-card">
          <div className="auth-header">
            <div className="auth-logo">💰</div>
            <h1>Quản Lý Chi Tiêu</h1>
            <p>Đăng nhập để tiếp tục</p>
          </div>
          <form onSubmit={handleSubmit} className="auth-form">
            <Input
              label="Email"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              error={errors.email}
              placeholder="nhap@email.com"
              leftIcon="📧"
              autoComplete="email"
              autoFocus
            />
            <Input
              label="Mật khẩu"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              error={errors.password}
              placeholder="••••••••"
              leftIcon="🔒"
              autoComplete="current-password"
            />
            <div className="auth-options">
              <label className="checkbox-wrapper">
                <input type="checkbox" />
                <span>Ghi nhớ đăng nhập</span>
              </label>
              <Link to="/forgot-password" className="forgot-link">Quên mật khẩu?</Link>
            </div>
            <Button type="submit" fullWidth loading={isLoading} size="lg">
              Đăng nhập
            </Button>
          </form>
          <div className="auth-footer">
            <p>Chưa có tài khoản? <Link to="/register">Đăng ký ngay</Link></p>
          </div>
        </div>
      </div>
    </div>
  );
}

export function RegisterPage() {
  const [fullName, setFullName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [phoneNumber, setPhoneNumber] = useState('');
  const [errors, setErrors] = useState<{ 
    fullName?: string; 
    email?: string; 
    password?: string; 
    confirmPassword?: string;
    phoneNumber?: string;
  }>({});
  const [isLoading, setIsLoading] = useState(false);
  const { register } = useAuth();
  const navigate = useNavigate();

  const validate = () => {
    const newErrors: typeof errors = {};
    if (!fullName.trim()) newErrors.fullName = 'Họ tên là bắt buộc';
    else if (fullName.trim().length < 2) newErrors.fullName = 'Họ tên tối thiểu 2 ký tự';
    if (!email) newErrors.email = 'Email là bắt buộc';
    else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) newErrors.email = 'Email không hợp lệ';
    if (!password) newErrors.password = 'Mật khẩu là bắt buộc';
    else if (password.length < 6) newErrors.password = 'Mật khẩu tối thiểu 6 ký tự';
    if (password !== confirmPassword) newErrors.confirmPassword = 'Mật khẩu không khớp';
    if (phoneNumber && !/^[0-9]{10,11}$/.test(phoneNumber)) newErrors.phoneNumber = 'Số điện thoại không hợp lệ';
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;
    setIsLoading(true);
    const success = await register(fullName.trim(), email, password, phoneNumber || undefined);
    if (success) navigate('/dashboard');
    setIsLoading(false);
  };

  return (
    <div className="auth-page">
      <div className="auth-container">
        <div className="auth-card">
          <div className="auth-header">
            <div className="auth-logo">💰</div>
            <h1>Tạo tài khoản</h1>
            <p>Bắt đầu quản lý tài chính ngay hôm nay</p>
          </div>
          <form onSubmit={handleSubmit} className="auth-form">
            <Input
              label="Họ và tên"
              type="text"
              value={fullName}
              onChange={(e) => setFullName(e.target.value)}
              error={errors.fullName}
              placeholder="Nguyễn Văn A"
              leftIcon="👤"
              autoComplete="name"
              autoFocus
            />
            <Input
              label="Email"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              error={errors.email}
              placeholder="nhap@email.com"
              leftIcon="📧"
              autoComplete="email"
            />
            <Input
              label="Số điện thoại (tùy chọn)"
              type="tel"
              value={phoneNumber}
              onChange={(e) => setPhoneNumber(e.target.value)}
              error={errors.phoneNumber}
              placeholder="09xxxxxxxx"
              leftIcon="📱"
              autoComplete="tel"
            />
            <Input
              label="Mật khẩu"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              error={errors.password}
              placeholder="••••••••"
              leftIcon="🔒"
              autoComplete="new-password"
              hint="Tối thiểu 6 ký tự"
            />
            <Input
              label="Xác nhận mật khẩu"
              type="password"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              error={errors.confirmPassword}
              placeholder="••••••••"
              leftIcon="🔒"
              autoComplete="new-password"
            />
            <div className="auth-options">
              <label className="checkbox-wrapper">
                <input type="checkbox" required />
                <span>Tôi đồng ý với <a href="#">Điều khoản</a> và <a href="#">Chính sách bảo mật</a></span>
              </label>
            </div>
            <Button type="submit" fullWidth loading={isLoading} size="lg">
              Tạo tài khoản
            </Button>
          </form>
          <div className="auth-footer">
            <p>Đã có tài khoản? <Link to="/login">Đăng nhập ngay</Link></p>
          </div>
        </div>
      </div>
    </div>
  );
}
