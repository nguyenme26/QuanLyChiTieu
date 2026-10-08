import React, { ReactNode, forwardRef } from 'react';

// Button Component
export interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'ghost' | 'danger';
  size?: 'sm' | 'md' | 'lg' | 'icon';
  fullWidth?: boolean;
  loading?: boolean;
  leftIcon?: ReactNode;
  rightIcon?: ReactNode;
}

export const Button = forwardRef<HTMLButtonElement, ButtonProps>(
  ({ 
    variant = 'primary', 
    size = 'md', 
    fullWidth = false, 
    loading = false, 
    leftIcon, 
    rightIcon, 
    children, 
    className = '', 
    disabled,
    ...props 
  }, ref) => {
    const classes = [
      'btn',
      `btn-${variant}`,
      size !== 'md' && `btn-${size}`,
      fullWidth && 'btn-full',
      className,
    ].filter(Boolean).join(' ');

    return (
      <button
        ref={ref}
        className={classes}
        disabled={disabled || loading}
        {...props}
      >
        {loading && <span className="spinner" style={{ width: 16, height: 16 }} />}
        {!loading && leftIcon && <span>{leftIcon}</span>}
        {children}
        {!loading && rightIcon && <span>{rightIcon}</span>}
      </button>
    );
  }
);

Button.displayName = 'Button';

// Input Component
export interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
  hint?: string;
  leftIcon?: ReactNode;
  rightIcon?: ReactNode;
  suffix?: string;
}

export const Input = forwardRef<HTMLInputElement, InputProps>(
  ({ label, error, hint, leftIcon, rightIcon, suffix, className = '', id, ...props }, ref) => {
    const inputId = id || React.useId();
    const errorId = error ? `${inputId}-error` : undefined;
    const hintId = hint ? `${inputId}-hint` : undefined;

    return (
      <div className={`input-group ${className}`}>
        {label && <label htmlFor={inputId} className="input-label">{label}</label>}
        <div className="input-wrapper">
          {leftIcon && <span className="input-icon">{leftIcon}</span>}
          <input
            ref={ref}
            id={inputId}
            className={`input ${error ? 'input-error' : ''}`}
            aria-invalid={error ? 'true' : 'false'}
            aria-describedby={`${errorId || ''} ${hintId || ''}`.trim() || undefined}
            {...props}
          />
          {suffix && <span className="input-suffix">{suffix}</span>}
          {rightIcon && <span className="input-icon" style={{ left: 'auto', right: '16px' }}>{rightIcon}</span>}
        </div>
        {error && <p id={errorId} className="input-error-message" role="alert">⚠ {error}</p>}
        {hint && !error && <p id={hintId} className="input-error-message" style={{ color: 'var(--text-muted)' }}>{hint}</p>}
      </div>
    );
  }
);

Input.displayName = 'Input';

// TextArea Component
export interface TextAreaProps extends React.TextareaHTMLAttributes<HTMLTextAreaElement> {
  label?: string;
  error?: string;
  hint?: string;
}

export const TextArea = forwardRef<HTMLTextAreaElement, TextAreaProps>(
  ({ label, error, hint, className = '', id, ...props }, ref) => {
    const inputId = id || React.useId();
    const errorId = error ? `${inputId}-error` : undefined;
    const hintId = hint ? `${inputId}-hint` : undefined;

    return (
      <div className={`input-group ${className}`}>
        {label && <label htmlFor={inputId} className="input-label">{label}</label>}
        <textarea
          ref={ref}
          id={inputId}
          className={`input ${error ? 'input-error' : ''}`}
          aria-invalid={error ? 'true' : 'false'}
          aria-describedby={`${errorId || ''} ${hintId || ''}`.trim() || undefined}
          {...props}
        />
        {error && <p id={errorId} className="input-error-message" role="alert">⚠ {error}</p>}
        {hint && !error && <p id={hintId} className="input-error-message" style={{ color: 'var(--text-muted)' }}>{hint}</p>}
      </div>
    );
  }
);

TextArea.displayName = 'TextArea';

// Select Component
export interface SelectProps extends React.SelectHTMLAttributes<HTMLSelectElement> {
  label?: string;
  error?: string;
  hint?: string;
  options: { value: string; label: string }[];
  placeholder?: string;
}

export const Select = forwardRef<HTMLSelectElement, SelectProps>(
  ({ label, error, hint, options, placeholder, className = '', id, ...props }, ref) => {
    const inputId = id || React.useId();
    const errorId = error ? `${inputId}-error` : undefined;
    const hintId = hint ? `${inputId}-hint` : undefined;

    return (
      <div className={`input-group ${className}`}>
        {label && <label htmlFor={inputId} className="input-label">{label}</label>}
        <div className="input-wrapper">
          <select
            ref={ref}
            id={inputId}
            className={`input ${error ? 'input-error' : ''}`}
            aria-invalid={error ? 'true' : 'false'}
            aria-describedby={`${errorId || ''} ${hintId || ''}`.trim() || undefined}
            {...props}
          >
            {placeholder && <option value="" disabled>{placeholder}</option>}
            {options.map(opt => (
              <option key={opt.value} value={opt.value}>{opt.label}</option>
            ))}
          </select>
        </div>
        {error && <p id={errorId} className="input-error-message" role="alert">⚠ {error}</p>}
        {hint && !error && <p id={hintId} className="input-error-message" style={{ color: 'var(--text-muted)' }}>{hint}</p>}
      </div>
    );
  }
);

Select.displayName = 'Select';

// Card Component
export interface CardProps {
  children: ReactNode;
  className?: string;
  hover?: boolean;
  padding?: 'none' | 'sm' | 'md' | 'lg';
}

export function Card({ children, className = '', hover = false, padding = 'md' }: CardProps) {
  const paddingClasses = {
    none: '',
    sm: 'p-4',
    md: 'p-6',
    lg: 'p-8',
  };

  return (
    <div className={`card ${hover ? 'card-hover' : ''} ${paddingClasses[padding]} ${className}`}>
      {children}
    </div>
  );
}

export function CardHeader({ children, className = '' }: { children: ReactNode; className?: string }) {
  return <div className={`card-header ${className}`}>{children}</div>;
}

export function CardBody({ children, className = '' }: { children: ReactNode; className?: string }) {
  return <div className={`card-body ${className}`}>{children}</div>;
}

export function CardFooter({ children, className = '' }: { children: ReactNode; className?: string }) {
  return <div className={`card-footer ${className}`}>{children}</div>;
}

// Modal Component
export interface ModalProps {
  isOpen: boolean;
  onClose: () => void;
  title?: string;
  children: ReactNode;
  footer?: ReactNode;
  size?: 'sm' | 'md' | 'lg' | 'xl' | 'full';
  closeOnOverlayClick?: boolean;
}

export function Modal({ 
  isOpen, 
  onClose, 
  title, 
  children, 
  footer, 
  size = 'md', 
  closeOnOverlayClick = true 
}: ModalProps) {
  if (!isOpen) return null;

  const sizeClasses = {
    sm: 'max-w-md',
    md: 'max-w-lg',
    lg: 'max-w-2xl',
    xl: 'max-w-4xl',
    full: 'max-w-full mx-4',
  };

  return (
    <div 
      className="modal-overlay" 
      onClick={closeOnOverlayClick ? onClose : undefined}
      role="dialog"
      aria-modal="true"
      aria-labelledby={title ? 'modal-title' : undefined}
    >
      <div 
        className={`modal ${sizeClasses[size]}`}
        onClick={e => e.stopPropagation()}
      >
        {(title || footer) && (
          <div className="modal-header">
            {title && <h2 id="modal-title" className="modal-title">{title}</h2>}
            <button className="modal-close" onClick={onClose} aria-label="Đóng">
              ✕
            </button>
          </div>
        )}
        <div className="modal-body">
          {children}
        </div>
        {footer && (
          <div className="modal-footer">
            {footer}
          </div>
        )}
      </div>
    </div>
  );
}

// Badge Component
export interface BadgeProps {
  children: ReactNode;
  variant?: 'primary' | 'warning' | 'danger' | 'info' | 'gray';
  className?: string;
}

export function Badge({ children, variant = 'primary', className = '' }: BadgeProps) {
  return (
    <span className={`badge badge-${variant} ${className}`}>
      {children}
    </span>
  );
}

// Avatar Component
export interface AvatarProps {
  src?: string;
  alt?: string;
  name?: string;
  size?: 'sm' | 'md' | 'lg' | 'xl';
  className?: string;
}

export function Avatar({ src, alt, name, size = 'md', className = '' }: AvatarProps) {
  const sizeClasses = {
    sm: 'avatar-sm',
    md: '',
    lg: 'avatar-lg',
    xl: 'avatar-xl',
  };

  const initials = name 
    ? name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2)
    : '?';

  return (
    <div className={`avatar ${sizeClasses[size]} ${className}`} aria-label={alt || name}>
      {src ? (
        <img src={src} alt={alt || name || ''} />
      ) : (
        initials
      )}
    </div>
  );
}

// Spinner Component
export interface SpinnerProps {
  size?: 'sm' | 'md' | 'lg';
  className?: string;
}

export function Spinner({ size = 'md', className = '' }: SpinnerProps) {
  const sizeClasses = {
    sm: 'spinner',
    md: 'spinner',
    lg: 'spinner-lg',
  };

  return <div className={`${sizeClasses[size]} ${className}`} role="status" aria-label="Đang tải..." />;
}

// EmptyState Component
export interface EmptyStateProps {
  icon?: ReactNode;
  title: string;
  message?: string;
  action?: ReactNode;
  className?: string;
}

export function EmptyState({ icon, title, message, action, className = '' }: EmptyStateProps) {
  return (
    <div className={`empty-state ${className}`}>
      <div className="empty-state-icon">
        {icon || '📭'}
      </div>
      <h3 className="empty-state-title">{title}</h3>
      {message && <p className="empty-state-message">{message}</p>}
      {action && <div className="empty-state-action" style={{ marginTop: 'var(--space-4)' }}>{action}</div>}
    </div>
  );
}

// StatCard Component
export interface StatCardProps {
  label: string;
  value: string | number;
  icon: ReactNode;
  iconVariant?: 'income' | 'expense' | 'balance' | 'warning';
  change?: string;
  changeType?: 'positive' | 'negative' | 'neutral';
  className?: string;
}

export function StatCard({ 
  label, 
  value, 
  icon, 
  iconVariant = 'balance', 
  change, 
  changeType = 'neutral', 
  className = '' 
}: StatCardProps) {
  return (
    <div className={`stat-card ${iconVariant} ${className}`}>
      <div className="stat-header">
        <span className="stat-label">{label}</span>
        <div className={`stat-icon ${iconVariant}`}>{icon}</div>
      </div>
      <div className="stat-value">{value}</div>
      {change && (
        <div className={`stat-change ${changeType === 'positive' ? 'positive' : changeType === 'negative' ? 'negative' : ''}`}>
          {changeType === 'positive' && '↑'}
          {changeType === 'negative' && '↓'}
          {change}
        </div>
      )}
    </div>
  );
}

// Dropdown Component
export interface DropdownItem {
  label: string;
  onClick: () => void;
  icon?: ReactNode;
  danger?: boolean;
  disabled?: boolean;
}

export interface DropdownProps {
  trigger: ReactNode;
  items: DropdownItem[];
  align?: 'left' | 'right';
  className?: string;
}

export function Dropdown({ trigger, items, align = 'right', className = '' }: DropdownProps) {
  const [isOpen, setIsOpen] = React.useState(false);
  const dropdownRef = React.useRef<HTMLDivElement>(null);

  React.useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    }

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  return (
    <div className={`dropdown ${isOpen ? 'open' : ''} ${className}`} ref={dropdownRef}>
      <div onClick={() => setIsOpen(!isOpen)}>{trigger}</div>
      <div className="dropdown-menu" style={{ [align]: 0 }}>
        {items.map((item, index) => (
          <button
            key={index}
            className={`dropdown-item ${item.danger ? 'danger' : ''} ${item.disabled ? 'disabled' : ''}`}
            onClick={() => { item.onClick(); setIsOpen(false); }}
            disabled={item.disabled}
          >
            {item.icon && <span>{item.icon}</span>}
            {item.label}
          </button>
        ))}
      </div>
    </div>
  );
}

// Tooltip Component
export interface TooltipProps {
  content: string;
  children: ReactNode;
  position?: 'top' | 'bottom' | 'left' | 'right';
}

export function Tooltip({ content, children, position = 'top' }: TooltipProps) {
  return (
    <span className="tooltip" data-tooltip={content} style={{ '--tooltip-position': position }}>
      {children}
    </span>
  );
}

// Tabs Component
export interface TabProps {
  id: string;
  label: string;
  icon?: ReactNode;
  disabled?: boolean;
}

export interface TabsProps {
  tabs: TabProps[];
  activeTab: string;
  onChange: (tabId: string) => void;
  className?: string;
}

export function Tabs({ tabs, activeTab, onChange, className = '' }: TabsProps) {
  return (
    <div className={`tabs ${className}`} role="tablist">
      {tabs.map(tab => (
        <button
          key={tab.id}
          role="tab"
          aria-selected={activeTab === tab.id}
          aria-controls={`${tab.id}-panel`}
          id={`${tab.id}-tab`}
          className={`tab ${activeTab === tab.id ? 'active' : ''} ${tab.disabled ? 'disabled' : ''}`}
          onClick={() => !tab.disabled && onChange(tab.id)}
          disabled={tab.disabled}
        >
          {tab.icon && <span className="tab-icon">{tab.icon}</span>}
          <span className="tab-label">{tab.label}</span>
        </button>
      ))}
    </div>
  );
}

export function TabPanel({ id, children, active }: { id: string; children: ReactNode; active: boolean }) {
  return (
    <div 
      role="tabpanel" 
      id={`${id}-panel`} 
      aria-labelledby={`${id}-tab`}
      hidden={!active}
      className="tab-panel"
    >
      {active && children}
    </div>
  );
}
