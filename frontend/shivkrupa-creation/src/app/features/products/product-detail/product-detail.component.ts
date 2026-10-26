import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ProductService } from '../../../core/services/product.service';
import { ProductDetail, ProductVariant } from '../../../core/models/product.model';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';
import { ImageViewerComponent } from '../image-viewer/image-viewer';
import { single } from 'rxjs';
import { videoViewerComponent } from '../video-viewer/video-viewer';

@Component({
  selector: 'sc-product-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, LoadingSpinnerComponent, EmptyStateComponent, ImageViewerComponent, videoViewerComponent],
  templateUrl: './product-detail.component.html',
  styleUrl: './product-detail.component.scss'
})
export class ProductDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly productService = inject(ProductService);

  readonly product = signal<ProductDetail | null>(null);
  readonly loading = signal(true);
  readonly notFound = signal(false);

  // ---------- Carousel state ----------
  readonly activeImageIndex = signal(0);

  readonly activeVideoIndex = signal(0);


  // Signal to track whether the preview modal is open

  showPreview = signal<boolean>(false);

  showVideoPreview = signal<boolean>(false);

  openPreview() {
    this.showPreview.set(true);
  }

  closePreview() {
    this.showPreview.set(false);
  }

  openVideoPreview() {
    this.showVideoPreview.set(true);
  }

  closeVideoPreview() {
    this.showVideoPreview.set(false);
  }


  readonly images = computed(() => {
    const p = this.product();
    if (!p) return [];
    return [...p.images].sort((a, b) => a.displayOrder - b.displayOrder);
  });

  // ---------- Variant selection ----------
  readonly selectedColor = signal<string | null>(null);
  readonly selectedSize = signal<string | null>(null);

  readonly selectedVariant = computed<ProductVariant | undefined>(() => {
    const p = this.product();
    if (!p) return undefined;
    return p.variants.find(
      (v) => v.color === this.selectedColor() && v.size === this.selectedSize() && v.isActive
    );
  });

  readonly availableSizesForColor = computed(() => {
    const p = this.product();
    if (!p || !this.selectedColor()) return [];
    return p.variants.filter((v) => v.color === this.selectedColor() && v.isActive).map((v) => v.size);
  });

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const slug = params.get('slug');
      if (!slug) return;
      this.loadProduct(slug);
    });
  }

  private loadProduct(slug: string) {
    this.loading.set(true);
    this.notFound.set(false);

    this.productService.getBySlug(slug).subscribe({
      next: (res) => {
        const product = res.data;
        this.product.set(product);
        this.activeImageIndex.set(0);

        if (product) {
          const firstColor = product.availableColors[0] ?? null;
          this.selectedColor.set(firstColor);
          const firstSize = product.variants.find((v) => v.color === firstColor)?.size ?? null;
          this.selectedSize.set(firstSize);
        }
      },
      error: () => this.notFound.set(true),
      complete: () => this.loading.set(false)
    });
  }

  // ---------- Carousel controls ----------

  selectImage(index: number) {
    if (index < 0 || index >= this.images().length) return;
    this.activeImageIndex.set(index);
  }

  nextImage() {
    const count = this.images().length;
    if (count === 0) return;
    this.activeImageIndex.set((this.activeImageIndex() + 1) % count);
  }

  prevImage() {
    const count = this.images().length;
    if (count === 0) return;
    this.activeImageIndex.set((this.activeImageIndex() - 1 + count) % count);
  }

  onCarouselKeydown(event: KeyboardEvent) {
    if (event.key === 'ArrowRight') {
      this.nextImage();
      event.preventDefault();
    } else if (event.key === 'ArrowLeft') {
      this.prevImage();
      event.preventDefault();
    }
  }

  selectColor(color: string) {
    this.selectedColor.set(color);
    const firstSizeForColor = this.product()?.variants.find((v) => v.color === color && v.isActive)?.size ?? null;
    this.selectedSize.set(firstSizeForColor);
  }

  selectSize(size: string) {
    this.selectedSize.set(size);
  }

  getColorHex(color: string): string {
    const p = this.product();
    return p?.variants.find((v) => v.color === color)?.colorHex || '#cccccc';
  }

  resolveImage(url?: string | null): string {
    return this.productService.resolveMediaUrl(url);
  }

  effectivePrice(): number {
    const p = this.product();
    if (!p) return 0;
    return this.selectedVariant()?.priceOverride ?? p.wholesalePrice;
  }

}
