import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CustomerService } from '../../../core/services/customer.service';
import { ToastService } from '../../../core/services/toast.service';
import { CustomerListItem } from '../../../core/models/customer-dashboard.model';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';

@Component({
  selector: 'sc-admin-customers',
  standalone: true,
  imports: [CommonModule, FormsModule, LoadingSpinnerComponent, EmptyStateComponent],
  templateUrl: './admin-customers.component.html'
})
export class AdminCustomersComponent implements OnInit {
  private readonly customerService = inject(CustomerService);
  private readonly toast = inject(ToastService);

  readonly customers = signal<CustomerListItem[]>([]);
  readonly loading = signal(true);
  readonly searchTerm = signal('');
  readonly pageNumber = signal(1);
  readonly totalPages = signal(0);
  readonly totalCount = signal(0);

  ngOnInit(): void {
    this.load();
  }

  load() {
    this.loading.set(true);
    this.customerService.getAll(this.searchTerm() || undefined, undefined, this.pageNumber(), 15).subscribe({
      next: (res) => {
        this.customers.set(res.data?.items ?? []);
        this.totalPages.set(res.data?.totalPages ?? 0);
        this.totalCount.set(res.data?.totalCount ?? 0);
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

  toggleStatus(customer: CustomerListItem) {
    this.customerService.setActiveStatus(customer.id, !customer.isActive).subscribe({
      next: () => {
        this.toast.success(customer.isActive ? 'Customer deactivated.' : 'Customer activated.');
        this.load();
      }
    });
  }

  get pageNumbers(): number[] {
    return Array.from({ length: this.totalPages() }, (_, i) => i + 1);
  }
}
