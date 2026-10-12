import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ProductService } from '../../../core/services/product.service';
import { ToastService } from '../../../core/services/toast.service';
import { ProductListItem } from '../../../core/models/product.model';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';

@Component({
  selector: 'sc-admin-product-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, LoadingSpinnerComponent, EmptyStateComponent],
  templateUrl: './admin-product-list.component.html'
})
export class AdminProductListComponent implements OnInit {
  private readonly productService = inject(ProductService);
  private readonly toast = inject(ToastService);

  readonly products = signal<ProductListItem[]>([]);
  readonly loading = signal(true);
  readonly searchTerm = signal('');
  readonly pageNumber = signal(1);
  readonly totalPages = signal(0);

  ngOnInit(): void {
    this.load();
  }

  load() {
    this.loading.set(true);
    this.productService
      .getProducts({
        searchTerm: this.searchTerm() || undefined,
        isActive: undefined, // admin sees active + inactive
        pageNumber: this.pageNumber(),
        pageSize: 10,
        sortBy: 'Newest'
      })
      .subscribe({
        next: (res) => {
          this.products.set(res.data?.items ?? []);
          this.totalPages.set(res.data?.totalPages ?? 0);
        },
        complete: () => this.loading.set(false)
      });
  }

  onSearch() {
    this.pageNumber.set(1);
    this.load();
  }

  goToPage(page: number) {
    if (page < 1 || page > this.totalPages()) return;
    this.pageNumber.set(page);
    this.load();
  }

  toggleStatus(product: ProductListItem) {
    this.productService.setActiveStatus(product.id, !product.isActive).subscribe({
      next: () => {
        this.toast.success(product.isActive ? 'Product deactivated.' : 'Product activated.');
        this.load();
      }
    });
  }

  deleteProduct(product: ProductListItem) {
    if (!confirm(`Delete "${product.name}"? This cannot be undone.`)) return;

    this.productService.delete(product.id).subscribe({
      next: () => {
        this.toast.success('Product deleted.');
        this.load();
      }
    });
  }

  resolveImage(url?: string | null): string {
    return this.productService.resolveMediaUrl(url) || 'https://placehold.co/60x60';
  }

  get pageNumbers(): number[] {
    return Array.from({ length: this.totalPages() }, (_, i) => i + 1);
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
}
