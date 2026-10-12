import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DashboardService } from '../../../core/services/dashboard.service';
import { ProductService } from '../../../core/services/product.service';
import { DashboardStats } from '../../../core/models/customer-dashboard.model';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'sc-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, LoadingSpinnerComponent],
  templateUrl: './dashboard.component.html'
})
export class AdminDashboardComponent implements OnInit {
  private readonly dashboardService = inject(DashboardService);
  readonly productService = inject(ProductService);

  readonly stats = signal<DashboardStats | null>(null);
  readonly loading = signal(true);

  ngOnInit(): void {
    this.dashboardService.getStats().subscribe({
      next: (res) => this.stats.set(res.data),
      complete: () => this.loading.set(false)
    });
  }

  maxCategoryCount(): number {
    const list = this.stats()?.productsByCategory ?? [];
    return Math.max(1, ...list.map((c) => c.productCount));
  }
}
