import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../auth/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  shopImage = 'images/pexels-n-voitkevich-6214471.jpg'
  loginForm: FormGroup;

  constructor(private authService: AuthService, private router: Router) {
    this.loginForm = new FormGroup({
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [Validators.required])
    });

    // Check initial form values for autofill
    setTimeout(() => {
      Object.keys(this.loginForm.controls).forEach(key => {
        if (this.loginForm.get(key)?.value) {
          this.onInputChange(key);
        }
      });
    }, 100);
  }

  onInputChange(controlName: string): void {
    const control = this.loginForm.get(controlName);
    if (control) {
      control.markAsTouched();
      control.updateValueAndValidity();
    }
  }

  submitLogin(): void {
    if (this.loginForm.valid) {
      this.authService.login(this.loginForm.value).subscribe({
        next: (res) => {
          this.router.navigate(['/home']);
        },
        error: (err) => {
          if (err.error) {
            const errors = err.error;
            if (errors.title == "Invalid credentials") {
              this.loginForm.get('email')?.setErrors({ serverError: 'Invalid email or password' });
              this.loginForm.get('password')?.setErrors({ serverError: 'Invalid email or password' });
              this.loginForm.get('email')?.markAsTouched();
              this.loginForm.get('password')?.markAsTouched();
            } 
          } else {
            alert('Invalid email or password');
          }
          console.error(err);
        }
      });
    } else {
      Object.keys(this.loginForm.controls).forEach(key => {
        const control = this.loginForm.get(key);
        if (control) {
          control.markAsTouched();
        }
      });
    }
  }

  getErrorMessage(controlName: string): string {
    const control = this.loginForm.get(controlName);

    if (control?.errors && (control.touched || control.errors['serverError'])) {
      if (control.errors['serverError']) return control.errors['serverError'];
      if (control.errors['required']) return 'This field is required';
      if (control.errors['email']) return 'Please enter a valid email address';
    }

    return '';
  }
}
