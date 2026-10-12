import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CustomerService } from '../../../core/services/customer.service';
import { ToastService } from '../../../core/services/toast.service';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'sc-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LoadingSpinnerComponent],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly customerService = inject(CustomerService);
  private readonly toast = inject(ToastService);

  readonly loading = signal(true);
  readonly savingProfile = signal(false);
  readonly savingPassword = signal(false);

  readonly profileForm = this.fb.nonNullable.group({
    fullName: ['', Validators.required],
    phoneNumber: [''],
    businessName: [''],
    gstNumber: [''],
    address: [''],
    city: [''],
    state: [''],
    pinCode: ['']
  });

  readonly passwordForm = this.fb.nonNullable.group({
    currentPassword: ['', Validators.required],
    newPassword: ['', [Validators.required, Validators.minLength(6)]],
    confirmNewPassword: ['', Validators.required]
  });

  ngOnInit(): void {
    this.customerService.getMyProfile().subscribe({
      next: (res) => {
        const profile = res.data;
        if (profile) {
          this.profileForm.patchValue({
            fullName: profile.fullName,
            phoneNumber: profile.phoneNumber ?? '',
            businessName: profile.businessName ?? '',
            gstNumber: profile.gstNumber ?? '',
            address: profile.address ?? '',
            city: profile.city ?? '',
            state: profile.state ?? '',
            pinCode: profile.pinCode ?? ''
          });
        }
      },
      complete: () => this.loading.set(false)
    });
  }

  saveProfile() {
    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      return;
    }

    this.savingProfile.set(true);
    this.customerService.updateMyProfile(this.profileForm.getRawValue()).subscribe({
      next: () => this.toast.success('Profile updated successfully.'),
      complete: () => this.savingProfile.set(false)
    });
  }

  changePassword() {
    if (this.passwordForm.invalid) {
      this.passwordForm.markAllAsTouched();
      return;
    }

    this.savingPassword.set(true);
    this.customerService.changeMyPassword(this.passwordForm.getRawValue()).subscribe({
      next: () => {
        this.toast.success('Password changed successfully.');
        this.passwordForm.reset();
      },
      complete: () => this.savingPassword.set(false)
    });
  }
}
