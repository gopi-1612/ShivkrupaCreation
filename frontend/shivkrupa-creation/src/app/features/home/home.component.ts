import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CategoryService } from '../../core/services/category.service';
import { ProductService } from '../../core/services/product.service';
import { Category } from '../../core/models/category-brand.model';
import { ProductListItem } from '../../core/models/product.model';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';
import { EmptyStateComponent } from '../../shared/components/empty-state/empty-state.component';

@Component({
  selector: 'sc-home',
  standalone: true,
  imports: [CommonModule, RouterLink, LoadingSpinnerComponent, EmptyStateComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  private readonly categoryService = inject(CategoryService);
  private readonly productService = inject(ProductService);

  readonly categories = signal<Category[]>([]);
  readonly featuredProducts = signal<ProductListItem[]>([]);
  readonly loadingCategories = signal(true);
  readonly loadingProducts = signal(true);

  ngOnInit(): void {
    this.categoryService.getAll().subscribe({
      next: (res) => this.categories.set(res.data?.filter(c => c.productCount > 0) ?? []),
      complete: () => this.loadingCategories.set(false)
    });

    this.productService.getProducts({ isFeatured: true, pageSize: 8, sortBy: 'Newest' }).subscribe({
      next: (res) => this.featuredProducts.set(res.data?.items ?? []),
      complete: () => this.loadingProducts.set(false)
    });
  }

  resolveImage(url?: string | null): string {
    return this.productService.resolveMediaUrl(url) || 'https://placehold.co/500x600?text=Shivkrupa+Creation';
  }
}
