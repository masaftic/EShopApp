import { CommonModule } from '@angular/common';
import { Component, OnDestroy } from '@angular/core';
import { FormGroup, FormControl, Validators, ReactiveFormsModule, ValidatorFn, AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../auth/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent implements OnDestroy {
  shopImage = 'images/pexels-n-voitkevich-6214471.jpg'
  registerForm: FormGroup;
  authSubscription$: any;

  constructor(private authService: AuthService, private router: Router) {
    this.registerForm = new FormGroup({
      firstName: new FormControl('', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]),
      lastName: new FormControl('', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]),
      email: new FormControl('', [Validators.required, Validators.email, Validators.maxLength(50)]),
      password: new FormControl('', [Validators.required, Validators.minLength(6), Validators.maxLength(50)]),
      confirmPassword: new FormControl('', [Validators.required])
    }, { validators: this.passwordMatchValidator() });
  }
  
  submitRegistration(): void {
    if (this.registerForm.valid) {
      this.authSubscription$ = this.authService.register(this.registerForm.value).subscribe({
        next: (res) => {
          console.log(res);
          this.router.navigate(['/home']);
        },
        error: (err) => {
          const validationErrors = err.error.errors;
          if (validationErrors.DuplicateEmail) {
            const emailControl = this.registerForm.get('email');
            emailControl?.setErrors({ serverError: validationErrors.DuplicateEmail[0] });
            emailControl?.markAsTouched();
          }
          if (validationErrors.Password) {
            const passwordControl = this.registerForm.get('password');
            passwordControl?.setErrors({ serverError: validationErrors.Password.join('\n') });
            passwordControl?.markAsTouched();
          }
          else {
            alert('An error occurred while registering');
          }
          console.error(err.error.errors);
        }
      });
    } else {
      Object.keys(this.registerForm.controls).forEach(key => {
      const control = this.registerForm.get(key);
      if (control) {
        control.markAsTouched();
      }
    });
    }
  }

  ngOnDestroy() {
    if (this.authSubscription$) {
      this.authSubscription$.unsubscribe();
    }
  }


  private passwordMatchValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
      const password = control.get('password');
      const confirmPassword = control.get('confirmPassword');
      return password && confirmPassword && password.value !== confirmPassword.value ?
        { 'passwordMismatch': true } : null;
    };
  }

  getErrorMessage(controlName: string): string {
    const control = this.registerForm.get(controlName);

    if (control?.errors && (control.touched || control.errors['serverError'])) {
      if (control.errors['serverError']) return control.errors['serverError'];
      if (control.errors['required']) return 'This field is required';
      if (control.errors['email']) return 'Please enter a valid email address';
      if (control.errors['minlength'])
        return `Minimum length is ${control.errors['minlength'].requiredLength} characters`;
      if (control.errors['maxlength'])
        return `Maximum length is ${control.errors['maxlength'].requiredLength} characters`;
    }

    // Check form-level errors for password mismatch
    if (controlName === 'confirmPassword' && this.registerForm.errors?.['passwordMismatch']) {
      return 'Passwords do not match';
    }

    return '';
  }
}
