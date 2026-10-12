import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import {
  AuthResponse,
  LoginRequest,
  RefreshTokenRequest,
  RegisterRequest,
  User
} from '../models/user.model';

const ACCESS_TOKEN_KEY = 'sc_access_token';
const REFRESH_TOKEN_KEY = 'sc_refresh_token';
const USER_KEY = 'sc_current_user';

/**
 * Central authentication service. Holds the current user as a signal so
 * components (navbar, guards, etc.) can reactively respond to login/logout
 * without subscribing manually. Tokens are persisted to localStorage so a
 * page refresh doesn't log the user out.
 */
@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/auth`;

  private readonly _currentUser = signal<User | null>(this.readStoredUser());

  readonly currentUser = this._currentUser.asReadonly();
  readonly isLoggedIn = computed(() => this._currentUser() !== null);
  readonly isAdmin = computed(() => this._currentUser()?.roles?.includes('Admin') ?? false);
  readonly isCustomer = computed(() => this._currentUser()?.roles?.includes('Customer') ?? false);

  register(request: RegisterRequest): Observable<ApiResponse<AuthResponse>> {
    return this.http
      .post<ApiResponse<AuthResponse>>(`${this.baseUrl}/register`, request)
      .pipe(tap((res) => res.data && this.persistSession(res.data)));
  }

  login(request: LoginRequest): Observable<ApiResponse<AuthResponse>> {
    return this.http
      .post<ApiResponse<AuthResponse>>(`${this.baseUrl}/login`, request)
      .pipe(tap((res) => res.data && this.persistSession(res.data)));
  }

  refreshToken(): Observable<ApiResponse<AuthResponse>> {
    const request: RefreshTokenRequest = {
      accessToken: this.getAccessToken() ?? '',
      refreshToken: this.getRefreshToken() ?? ''
    };

    return this.http
      .post<ApiResponse<AuthResponse>>(`${this.baseUrl}/refresh`, request)
      .pipe(tap((res) => res.data && this.persistSession(res.data)));
  }

  logout(): Observable<ApiResponse<object>> | void {
    const refreshToken = this.getRefreshToken();
    this.clearSession();

    if (refreshToken) {
      return this.http.post<ApiResponse<object>>(`${this.baseUrl}/logout`, { refreshToken });
    }
  }

  getAccessToken(): string | null {
    return localStorage.getItem(ACCESS_TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(REFRESH_TOKEN_KEY);
  }

  private persistSession(auth: AuthResponse) {
    localStorage.setItem(ACCESS_TOKEN_KEY, auth.accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, auth.refreshToken);
    localStorage.setItem(USER_KEY, JSON.stringify(auth.user));
    this._currentUser.set(auth.user);
  }

  private clearSession() {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this._currentUser.set(null);
  }

  private readStoredUser(): User | null {
    const raw = localStorage.getItem(USER_KEY);
    if (!raw) return null;
    try {
      return JSON.parse(raw) as User;
    } catch {
      return null;
    }
  }
}
