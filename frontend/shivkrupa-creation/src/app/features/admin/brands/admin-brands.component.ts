import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { BrandService } from '../../../core/services/brand.service';
import { ToastService } from '../../../core/services/toast.service';
import { Brand } from '../../../core/models/category-brand.model';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';

@Component({
  selector: 'sc-admin-brands',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LoadingSpinnerComponent, EmptyStateComponent],
  templateUrl: './admin-brands.component.html'
})
export class AdminBrandsComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly brandService = inject(BrandService);
  private readonly toast = inject(ToastService);

  readonly brands = signal<Brand[]>([]);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly showModal = signal(false);
  readonly editingId = signal<number | null>(null);

  readonly form = this.fb.nonNullable.group({
    name: ['', Validators.required],
    logoUrl: [''],
    description: [''],
    isActive: [true]
  });

  ngOnInit(): void {
    this.load();
  }

  load() {
    this.loading.set(true);
    this.brandService.getAll(true).subscribe({
      next: (res) => this.brands.set(res.data ?? []),
      complete: () => this.loading.set(false)
    });
  }

  openCreateModal() {
    this.editingId.set(null);
    this.form.reset({ name: '', logoUrl: '', description: '', isActive: true });
    this.showModal.set(true);
  }

  openEditModal(brand: Brand) {
    this.editingId.set(brand.id);
    this.form.reset({
      name: brand.name,
      logoUrl: brand.logoUrl ?? '',
      description: brand.description ?? '',
      isActive: brand.isActive
    });
    this.showModal.set(true);
  }

  closeModal() {
    this.showModal.set(false);
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    const id = this.editingId();
    const request = id ? this.brandService.update(id, this.form.getRawValue()) : this.brandService.create(this.form.getRawValue());

    request.subscribe({
      next: () => {
        this.toast.success(id ? 'Brand updated.' : 'Brand created.');
        this.closeModal();
        this.load();
      },
      complete: () => this.saving.set(false)
    });
  }

  toggleStatus(brand: Brand) {
    this.brandService.setActiveStatus(brand.id, !brand.isActive).subscribe({
      next: () => {
        this.toast.success(brand.isActive ? 'Brand deactivated.' : 'Brand activated.');
        this.load();
      }
    });
  }

  deleteBrand(brand: Brand) {
    if (!confirm(`Delete brand "${brand.name}"?`)) return;
    this.brandService.delete(brand.id).subscribe({ next: () => { this.toast.success('Brand deleted.'); this.load(); } });
  }
}
