import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ToastContainerComponent } from '../../components/toast-container/toast-container.component';

@Component({
  selector: 'sc-admin-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, ToastContainerComponent],
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.scss'
})
export class AdminLayoutComponent {
  private readonly router = inject(Router);
  readonly auth = inject(AuthService);

  readonly sidebarOpen = signal(true);

  toggleSidebar() {
    this.sidebarOpen.update((v) => !v);
  }

  onLogout() {
    const result = this.auth.logout();
    if (result) result.subscribe();
    this.router.navigate(['/']);
  }
}
