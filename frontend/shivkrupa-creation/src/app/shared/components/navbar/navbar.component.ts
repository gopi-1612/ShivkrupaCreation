import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'sc-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent {
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);

  readonly auth = inject(AuthService);

  onLogout() {
    const result = this.auth.logout();
    if (result) {
      result.subscribe();
    }
    this.toast.success('You have been logged out.');
    this.router.navigate(['/']);
  }
}
