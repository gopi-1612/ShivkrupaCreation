import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CategoryService } from '../../../core/services/category.service';
import { ToastService } from '../../../core/services/toast.service';
import { Category } from '../../../core/models/category-brand.model';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';
import { consumerMarkDirty } from '@angular/core/primitives/signals';

@Component({
  selector: 'sc-admin-categories',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LoadingSpinnerComponent, EmptyStateComponent],
  templateUrl: './admin-categories.component.html'
})
export class AdminCategoriesComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly categoryService = inject(CategoryService);
  private readonly toast = inject(ToastService);

  readonly categories = signal<Category[]>([]);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly showModal = signal(false);
  readonly editingId = signal<number | null>(null);

  selectedImage: File | null = null;
  imagePreview: string | null = null;

  readonly form = this.fb.nonNullable.group({
    name: ['', Validators.required],
    description: [''],
    imageUrl: [''],
    displayOrder: [0],
    isActive: [true]
  });

  ngOnInit(): void {
    this.load();
  }

  load() {
    this.loading.set(true);
    this.categoryService.getAll(true).subscribe({
      next: (res) => this.categories.set(res.data ?? []),
      complete: () => this.loading.set(false)
    });
  }

  openCreateModal() {
    this.editingId.set(null);
    this.form.reset({ name: '', description: '', imageUrl: '', displayOrder: 0, isActive: true });
    this.showModal.set(true);
  }

  openEditModal(category: Category) {
    this.editingId.set(category.id);
    this.form.reset({
      name: category.name,
      description: category.description ?? '',
      displayOrder: category.displayOrder,
      imageUrl: category.imageUrl ?? '',
      isActive: category.isActive
    });

    // Display the existing image from your backend
    this.imagePreview = null
    this.imagePreview = category.imageUrl
      ? this.resolveImage(category.imageUrl)
      : null;
    this.showModal.set(true);
  }

  closeModal() {
    this.showModal.set(false);
    this.imagePreview = null;
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const formData = new FormData();

    formData.append('Name', this.form.get('name')?.value ?? '');
    formData.append(
      'Description',
      this.form.get('description')?.value ?? ''
    );
    formData.append(
      'DisplayOrder',
      String(this.form.get('displayOrder')?.value ?? 1)
    );
    formData.append(
      'IsActive',
      String(this.form.get('isActive')?.value ?? true)
    );

    if (this.selectedImage) {
      formData.append(
        'imageUrl',
        this.selectedImage,
        this.selectedImage.name
      );
    }

    this.saving.set(true);
    const id = this.editingId();
    const request = id ? this.categoryService.update(id, formData) : this.categoryService.create(formData);

    request.subscribe({
      next: () => {
        this.toast.success(id ? 'Category updated.' : 'Category created.');
        this.closeModal();
        this.load();
      },
      complete: () => this.saving.set(false)
    });
  }

  toggleStatus(category: Category) {
    this.categoryService.setActiveStatus(category.id, !category.isActive).subscribe({
      next: () => {
        this.toast.success(category.isActive ? 'Category deactivated.' : 'Category activated.');
        this.load();
      }
    });
  }

  deleteCategory(category: Category) {
    if (!confirm(`Delete category "${category.name}"?`)) return;
    this.categoryService.delete(category.id).subscribe({ next: () => { this.toast.success('Category deleted.'); this.load(); } });
  }

  onImageSelected(event: Event): void {

    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];

    if (!file) return;

    if (this.imagePreview?.startsWith('blob:')) {
      URL.revokeObjectURL(this.imagePreview);
    }

    this.selectedImage = file;
    this.imagePreview = URL.createObjectURL(file);
  }
  resolveImage(url?: string | null): string {
    return this.categoryService.resolveImageUrl(url);
  }

}
