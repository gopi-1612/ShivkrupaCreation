import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToastService } from '../../../core/services/toast.service';
import { ContactService } from '../../../core/services/contact.service';

@Component({
  selector: 'sc-contact',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './contact.component.html'
})
export class ContactComponent {
  private readonly fb = inject(FormBuilder);
  private readonly toast = inject(ToastService);
  private readonly contactService = inject(ContactService);

  readonly submitted = signal(false);

  readonly form = this.fb.nonNullable.group({
    name: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    phone: [''],
    message: ['', Validators.required]
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitted.set(true);

    const value = this.form.getRawValue();
    this.contactService
      .submit({
        name: value.name ?? '',
        email: value.email ?? '',
        phone: value.phone ?? '',
        message: value.message ?? ''
      })
      .subscribe({
        next: res => {
          this.submitted.set(true);
          this.toast.success('Thanks for reaching out! Our team will contact you shortly.');
          this.form.reset();
        },
        error: () => {
          this.submitted.set(false);
          this.toast.error('Something went wrong sending your message. Please try again or email us directly.');
        }
      });
  }

  sendAnother(): void {
    this.submitted.set(false);
    this.form.reset();
  }

}
