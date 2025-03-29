import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { RegistrationRequest } from './registration-request';
import { Observable } from 'rxjs';
import { environment } from '../environment';
import { jwtDecode } from 'jwt-decode';
import { LoginRequest } from './login-request';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  constructor(private http: HttpClient) { }

  register(registerForm: RegistrationRequest): Observable<any> {
    return this.http.post(`${environment.apiUrl}/users/register`, registerForm);
  }

  login(loginForm: LoginRequest): Observable<any> {
    return this.http.post(`${environment.apiUrl}/users/login`, loginForm);
  }

  setToken(token: string): void {
    localStorage.setItem('token', token);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getCurrentUser(): any {
    const token = this.getToken();
    if (!token) return null;
    return jwtDecode(token);
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) return false;
    const decoded = jwtDecode<any>(token);
    const currentTime = Math.floor(Date.now() / 1000);
    return decoded.exp > currentTime;
  }

  logout(): void {
    localStorage.removeItem('token');
  }
}
