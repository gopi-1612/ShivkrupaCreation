import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'sc-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: '../auth.shared.scss'
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly submitting = signal(false);
  readonly showPassword = signal(false);

  readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]]
  });

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.auth.login(this.form.getRawValue()).subscribe({
      next: (res) => {
        this.toast.success(`Welcome back, ${res.data?.user.fullName}!`);
        const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
        const isAdmin = res.data?.user.roles.includes('Admin');
        this.router.navigateByUrl(returnUrl || (isAdmin ? '/admin/dashboard' : '/'));
      },
      complete: () => this.submitting.set(false)
    });
  }
}
