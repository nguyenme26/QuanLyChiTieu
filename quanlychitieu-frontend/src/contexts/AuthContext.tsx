import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { apiService } from '../services/api';
import toast from 'react-hot-toast';

export interface User {
  id: number;
  fullName: string;
  email: string;
  phoneNumber?: string;
  createdAt: string;
}

export interface AuthResponse {
  token: string;
  expiresAt: string;
  user: User;
}

interface AuthContextType {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (email: string, password: string) => Promise<boolean>;
  register: (fullName: string, email: string, password: string, phoneNumber?: string) => Promise<boolean>;
  logout: () => void;
  refreshUser: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const initAuth = async () => {
      const token = apiService.getToken();
      if (token) {
        try {
          // Try to get user from localStorage first
          const storedUser = localStorage.getItem('user');
          if (storedUser) {
            setUser(JSON.parse(storedUser));
          }
          // Could also call /api/auth/me endpoint here if available
        } catch (error) {
          console.error('Failed to restore auth:', error);
          apiService.setToken(null);
          localStorage.removeItem('user');
        }
      }
      setIsLoading(false);
    };
    initAuth();
  }, []);

  const login = async (email: string, password: string): Promise<boolean> => {
    try {
      const response = await apiService.getClient().post<AuthResponse>('/auth/login', { email, password });
      const { token, user } = response.data;
      apiService.setToken(token);
      localStorage.setItem('user', JSON.stringify(user));
      setUser(user);
      toast.success('Đăng nhập thành công!');
      return true;
    } catch (error: any) {
      const message = error.response?.data?.message || 'Đăng nhập thất bại';
      toast.error(message);
      return false;
    }
  };

  const register = async (fullName: string, email: string, password: string, phoneNumber?: string): Promise<boolean> => {
    try {
      const response = await apiService.getClient().post<AuthResponse>('/auth/register', { 
        fullName, 
        email, 
        password, 
        phoneNumber 
      });
      const { token, user } = response.data;
      apiService.setToken(token);
      localStorage.setItem('user', JSON.stringify(user));
      setUser(user);
      toast.success('Đăng ký thành công!');
      return true;
    } catch (error: any) {
      const message = error.response?.data?.message || 'Đăng ký thất bại';
      toast.error(message);
      return false;
    }
  };

  const logout = () => {
    apiService.setToken(null);
    localStorage.removeItem('user');
    setUser(null);
    toast.success('Đã đăng xuất');
  };

  const refreshUser = async () => {
    const token = apiService.getToken();
    if (!token) return;
    try {
      // If you have a /auth/me endpoint, use it here
      // const response = await api.get<User>('/auth/me');
      // setUser(response.data);
    } catch (error) {
      logout();
    }
  };

  return (
    <AuthContext.Provider value={{
      user,
      isAuthenticated: !!user,
      isLoading,
      login,
      register,
      logout,
      refreshUser,
    }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}
