import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { ProductService } from '../../../core/services/product.service';
import { CategoryService } from '../../../core/services/category.service';
import { BrandService } from '../../../core/services/brand.service';
import { ProductListItem, ProductQuery, ProductSortBy } from '../../../core/models/product.model';
import { Category, Brand } from '../../../core/models/category-brand.model';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';

@Component({
  selector: 'sc-product-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, LoadingSpinnerComponent, EmptyStateComponent],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.scss'
})
export class ProductListComponent implements OnInit {
  private readonly productService = inject(ProductService);
  private readonly categoryService = inject(CategoryService);
  private readonly brandService = inject(BrandService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly products = signal<ProductListItem[]>([]);
  readonly categories = signal<Category[]>([]);
  readonly brands = signal<Brand[]>([]);
  readonly loading = signal(true);

  readonly totalCount = signal(0);
  readonly totalPages = signal(0);

  readonly query: ProductQuery = {
    pageNumber: 1,
    pageSize: 12,
    sortBy: 'Newest'
  };

  private readonly searchSubject = new Subject<string>();

  ngOnInit(): void {
    // pick up ?categoryId= from the Home page category links
    this.route.queryParamMap.subscribe((params) => {
      const categoryId = params.get('categoryId');
      if (categoryId) this.query.categoryId = Number(categoryId);
      this.loadProducts();
    });

    this.categoryService.getAll().subscribe((res) => this.categories.set(res.data ?? []));
    this.brandService.getAll().subscribe((res) => this.brands.set(res.data ?? []));

    this.searchSubject.pipe(debounceTime(400), distinctUntilChanged()).subscribe((term) => {
      this.query.searchTerm = term || undefined;
      this.query.pageNumber = 1;
      this.loadProducts();
    });
  }

  onSearchInput(term: string) {
    this.searchSubject.next(term);
  }

  onFilterChange() {
    this.query.pageNumber = 1;
    this.loadProducts();
  }

  onSortChange(sortBy: ProductSortBy) {
    this.query.sortBy = sortBy;
    this.loadProducts();
  }

  goToPage(page: number) {
    if (page < 1 || page > this.totalPages()) return;
    this.query.pageNumber = page;
    this.loadProducts();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  clearFilters() {
    this.query.categoryId = undefined;
    this.query.brandId = undefined;
    this.query.minPrice = undefined;
    this.query.maxPrice = undefined;
    this.query.searchTerm = undefined;
    this.query.pageNumber = 1;
    this.router.navigate([], { relativeTo: this.route, queryParams: {} });
    this.loadProducts();
  }

  private loadProducts() {
    this.loading.set(true);
    this.productService.getProducts(this.query).subscribe({
      next: (res) => {
        this.products.set(res.data?.items ?? []);
        this.totalCount.set(res.data?.totalCount ?? 0);
        this.totalPages.set(res.data?.totalPages ?? 0);
      },
      complete: () => this.loading.set(false)
    });
  }

  resolveImage(url?: string | null): string {
    return this.productService.resolveMediaUrl(url) || 'https://placehold.co/400x480?text=Shivkrupa+Creation';
  }

  get pageNumbers(): number[] {
    return Array.from({ length: this.totalPages() }, (_, i) => i + 1);
  }
}
