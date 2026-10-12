import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { FooterComponent } from '../../components/footer/footer.component';
import { ToastContainerComponent } from '../../components/toast-container/toast-container.component';

@Component({
  selector: 'sc-customer-layout',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent, FooterComponent, ToastContainerComponent],
  template: `
    <sc-navbar></sc-navbar>
    <main class="sc-fade-in">
      <router-outlet></router-outlet>
    </main>
    <sc-footer></sc-footer>
    <sc-toast-container></sc-toast-container>
  `
})
export class CustomerLayoutComponent {}
