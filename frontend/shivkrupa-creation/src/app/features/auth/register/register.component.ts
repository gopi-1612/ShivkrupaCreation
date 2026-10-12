import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';
import { ColdObservable } from 'rxjs/internal/testing/ColdObservable';

function passwordsMatchValidator(control: AbstractControl): ValidationErrors | null {
  const password = control.get('password')?.value;
  const confirmPassword = control.get('confirmPassword')?.value;
  return password && confirmPassword && password !== confirmPassword ? { passwordMismatch: true } : null;
}

@Component({
  selector: 'sc-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: '../auth.shared.scss'
})
export class RegisterComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);

  readonly submitting = signal(false);

  readonly form = this.fb.nonNullable.group(
    {
      fullName: ['', [Validators.required, Validators.maxLength(150)]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', [Validators.pattern(/^[6-9]\d{9}$/)]],
      businessName: [''],
      gstNumber: [''],
      city: [''],
      state: [''],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    },
    { validators: passwordsMatchValidator }
  );

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.submitting.set(true);
    this.auth.register(this.form.getRawValue()).subscribe({
      next: (res) => {
        this.toast.success(`Welcome to Shivkrupa Creation, ${res.data?.user.fullName}!`);
        this.router.navigateByUrl('/');
      },
      error: (err) => {
        debugger;
        if (err.error?.errors) {
          for (const [key, value] of Object.entries(err.error.errors)) {
            // const control = this.form.get(key);
            // if (control) {
            //   control.setErrors({ serverError: value });
            // }
            this.toast.error(value as string);
          }
        }
        this.toast.error(err.error?.message ?? 'An error occurred during registration.');
        this.submitting.set(false);
      },
      complete: () => this.submitting.set(false)
    });
  }
}
