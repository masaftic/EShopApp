import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { RegistrationRequest } from './registration-request';
import { Observable, tap } from 'rxjs';
import { environment } from '../environment';
import { jwtDecode } from 'jwt-decode';
import { LoginRequest } from './login-request';
import { AuthResponse } from './auth-response';
import { User } from './User';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private decodedToken: any = null;
  private readonly tokenPath = 'accessToken';


  public get accessToken(): string | null {
    return localStorage.getItem('accessToken');
  }
  public set accessToken(value: string) {
    localStorage.setItem('accessToken', value);
  }

  public get refreshToken(): string | null {
    return localStorage.getItem('refreshToken');
  }
  public set refreshToken(value: string) {
    localStorage.setItem('refreshToken', value);
  }

  constructor(private http: HttpClient) { }

  register(registerForm: RegistrationRequest): Observable<any> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/users/register`, registerForm)
      .pipe(
        tap(res => {
          this.accessToken = res.accessToken,
          this.refreshToken = res.refreshToken
        }));
  }

  login(loginForm: LoginRequest): Observable<any> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/users/login`, loginForm)
      .pipe(
        tap(res => {
          this.accessToken = res.accessToken,
          this.refreshToken = res.refreshToken
        }));
  }

  renewAuthToken(): Observable<any> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/users/refresh-token`, { refreshToken: this.refreshToken })
      .pipe(
        tap(res => {
          this.accessToken = res.accessToken,
          this.refreshToken = res.refreshToken
        }));
  }

  private decodeToken(): any | null {
    if (this.accessToken) {
      return jwtDecode(this.accessToken);
    }
    return null;
  }

  getCurrentUser(): Observable<any> {
    return this.http.get<User>(`${environment.apiUrl}/users/me`);
  }

  isLoggedIn(): boolean {
    return !!this.accessToken;
  }

  isTokenExpired(): boolean {
    const decoded = this.decodeToken();
    if (!decoded) return true;
    const currentTime = Math.floor(Date.now() / 1000);
    return decoded.exp <= currentTime;
  }

  logout(): void {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    this.decodedToken = null;
  }
}


