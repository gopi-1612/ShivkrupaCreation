import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ProductService } from '../../../core/services/product.service';
import { CategoryService } from '../../../core/services/category.service';
import { BrandService } from '../../../core/services/brand.service';
import { ToastService } from '../../../core/services/toast.service';
import { Category, Brand } from '../../../core/models/category-brand.model';
import { ProductDetail } from '../../../core/models/product.model';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'sc-admin-product-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, LoadingSpinnerComponent],
  templateUrl: './admin-product-form.component.html'
})
export class AdminProductFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly productService = inject(ProductService);
  private readonly categoryService = inject(CategoryService);
  private readonly brandService = inject(BrandService);
  private readonly toast = inject(ToastService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly categories = signal<Category[]>([]);
  readonly brands = signal<Brand[]>([]);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly uploadingImage = signal(false);
  readonly uploadingVideo = signal(false);

  selectedImageFiles: File[] = [];

  readonly productId = signal<number | null>(null);
  readonly currentProduct = signal<ProductDetail | null>(null);
  get isEditMode(): boolean {
    return this.productId() !== null;
  }

  readonly form = this.fb.nonNullable.group({
    name: ['', Validators.required],
    sku: ['', Validators.required],
    shortDescription: [''],
    description: [''],
    fabric: ['', Validators.required],
    pattern: ['', Validators.required],
    setType: [''],
    work: [''],
    style: [''],
    bottomStyle: [''],
    stitchType: [''],
    sleeveType: [''],
    neck: [''],
    occasion: [''],
    mrp: [0, [Validators.required, Validators.min(1)]],
    wholesalePrice: [0, [Validators.required, Validators.min(1)]],
    minWholesaleQuantity: [1, [Validators.required, Validators.min(1)]],
    stockQuantity: [0, [Validators.required, Validators.min(0)]],
    categoryId: [0, [Validators.required, Validators.min(1)]],
    brandId: [0, [Validators.required, Validators.min(1)]],
    isFeatured: [false],
    isActive: [true],
    variants: this.fb.array<ReturnType<typeof this.buildVariantGroup>>([])
  });

  get variants(): FormArray {
    return this.form.controls.variants;
  }

  ngOnInit(): void {
    this.categoryService.getAll(true).subscribe((res) => this.categories.set(res.data ?? []));
    this.brandService.getAll(true).subscribe((res) => this.brands.set(res.data ?? []));

    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      const id = Number(idParam);
      this.productId.set(id);
      this.productService.getById(id).subscribe({
        next: (res) => {
          if (res.data) this.populateForm(res.data);
        },
        complete: () => this.loading.set(false)
      });
    } else {
      this.addVariant();
      this.loading.set(false);
    }
    this.form.get('name')?.valueChanges.subscribe(value => {
      const sku = this.generateSku(value || '');

      this.form.patchValue({
        sku: sku
      }, { emitEvent: false });
    });

    this.variants.valueChanges.subscribe(() => {
      this.variants.controls.forEach((_, index) => {
        this.attachVariantSkuListeners(index);
      });
    });
  }

  private attachVariantSkuListeners(index: number): void {
    const variant = this.variants.at(index);
    if (!variant) return;

    variant.get('color')?.valueChanges.subscribe(() => {
      variant.patchValue({
        sku: this.generateVariantSku(index)
      }, { emitEvent: false });
    });
    variant.get('size')?.valueChanges.subscribe(() => {
      const variantSku = this.generateVariantSku(index);
      variant.patchValue({
        sku: variantSku
      }, { emitEvent: false });
    });
  }


  private buildVariantGroup(variant?: ProductDetail['variants'][number]) {
    return this.fb.nonNullable.group({
      id: [variant?.id ?? null],
      color: [variant?.color ?? ''],
      colorHex: [variant?.colorHex ?? '#800000'],
      size: [variant?.size ?? ''],
      sku: [variant?.sku ?? ''],
      stockQuantity: [variant?.stockQuantity ?? 0],
      priceOverride: [variant?.priceOverride ?? null],
      isActive: [variant?.isActive ?? true]
    });
  }

  addVariant() {
    this.variants.push(this.buildVariantGroup());
  }

  removeVariant(index: number) {
    this.variants.removeAt(index);
  }

  private populateForm(product: ProductDetail) {
    this.currentProduct.set(product);
    this.form.patchValue({
      name: product.name,
      sku: product.sku,
      shortDescription: product.shortDescription ?? '',
      description: product.description ?? '',
      fabric: product.fabric,
      pattern: product.pattern,
      setType: product.setType ?? '',
      work: product.work ?? '',
      style: product.style ?? '',
      bottomStyle: product.bottomStyle ?? '',
      stitchType: product.stitchType ?? '',
      sleeveType: product.sleeveType ?? '',
      neck: product.neck ?? '',
      occasion: product.occasion ?? '',
      mrp: product.mrp,
      wholesalePrice: product.wholesalePrice,
      minWholesaleQuantity: product.minWholesaleQuantity,
      stockQuantity: product.stockQuantity,
      categoryId: product.categoryId,
      brandId: product.brandId,
      isFeatured: product.isFeatured,
      isActive: product.isActive
    });

    this.variants.clear();
    product.variants.forEach((v) => this.variants.push(this.buildVariantGroup(v)));
    if (this.variants.length === 0) this.addVariant();
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.toast.error('Please fill in all required fields correctly.');
      return;
    }
    console.log('Form Value:', this.form.getRawValue());
    this.saving.set(true);
    const payload = this.form.getRawValue();

    const request = this.isEditMode
      ? this.productService.update(this.productId()!, payload)
      : this.productService.create(payload);

    request.subscribe({
      next: (res) => {
        this.toast.success(this.isEditMode ? 'Product updated.' : 'Product created.');
        if (!this.isEditMode && res.data) {
          // Move into edit mode on the new product so images/videos can be uploaded.
          this.router.navigate(['/admin/products', res.data.id, 'edit']);
        } else if (res.data) {
          this.populateForm(res.data);
        }
      },
      complete: () => this.saving.set(false)
    });
  }

  // ---------- Image / video upload (edit mode only) ----------

  onImageSelected(event: Event) {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file || !this.productId()) return;
    this.selectedImageFiles = Array.from((event.target as HTMLInputElement).files || []);
  }

  uploadImage() {
    if (!this.productId() || !this.selectedImageFiles.length) return;

    const formData = new FormData();
    for (let file of this.selectedImageFiles) {
      formData.append('files', file);
    }
    formData.append("altText", this.form.value.name || '');
    formData.append('isPrimary', 'false'); // Set isPrimary to false for all uploaded images

    this.uploadingImage.set(true);
    this.productService.uploadImage(this.productId()!, formData).subscribe({
      next: () => {
        this.toast.success('Image uploaded.');
        this.refreshProduct();
      },
      complete: () => this.uploadingImage.set(false)
    });
  }

  removeImage(imageId: number) {
    if (!this.productId() || !confirm('Remove this image?')) return;
    this.productService.removeImage(this.productId()!, imageId).subscribe({
      next: () => {
        this.toast.success('Image removed.');
        this.refreshProduct();
      }
    });
  }

  onVideoSelected(event: Event) {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file || !this.productId()) return;

    this.uploadingVideo.set(true);
    this.productService.uploadVideo(this.productId()!, file, this.form.value.name).subscribe({
      next: () => {
        this.toast.success('Video uploaded.');
        this.refreshProduct();
      },
      complete: () => this.uploadingVideo.set(false)
    });
  }

  removeVideo(videoId: number) {
    if (!this.productId() || !confirm('Remove this video?')) return;
    this.productService.removeVideo(this.productId()!, videoId).subscribe({
      next: () => {
        this.toast.success('Video removed.');
        this.refreshProduct();
      }
    });
  }

  private refreshProduct() {
    this.productService.getById(this.productId()!).subscribe((res) => {
      if (res.data) this.currentProduct.set(res.data);
    });
  }

  resolveImage(url?: string | null): string {
    return this.productService.resolveMediaUrl(url);
  }
  generateSku(productName: string): string {

    if (!productName) {
      return '';
    }

    const words = productName
      .trim()
      .split(/\s+/)
      .filter(word => word.length > 0);

    const prefix = words
      .map(word => word.replace(/[^a-zA-Z0-9]/g, ''))
      .filter(word => word.length > 0)
      .map(word => word.substring(0, 3).toUpperCase())
      .join('-');

    return `${prefix}-001`;
  }
  generateVariantSku(index: number): any {
    const variant = this.variants.at(index);
    const productName = this.form.get('name')?.value || '';
    const color = variant.get('color')?.value || '';
    const size = variant.get('size')?.value || '';

    const colorCode = color
      .substring(0, 3)
      .toUpperCase();

    const sizeCode = size
      .substring(0, 3)
      .toUpperCase();

    return `${productName.substring(0, 3).toUpperCase()}-${colorCode}-${sizeCode}`;
  }
}
