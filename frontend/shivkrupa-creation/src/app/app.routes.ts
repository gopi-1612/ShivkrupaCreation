import { Routes } from '@angular/router';
import { CustomerLayoutComponent } from './shared/layouts/customer-layout/customer-layout.component';
import { AdminLayoutComponent } from './shared/layouts/admin-layout/admin-layout.component';
import { authGuard, adminGuard, guestGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    component: CustomerLayoutComponent,
    children: [
      {
        path: '',
        loadComponent: () => import('./features/home/home.component').then((m) => m.HomeComponent),
        title: 'Shivkrupa Creation | Style Ka Naya Andaz'
      },
      {
        path: 'products',
        loadComponent: () =>
          import('./features/products/product-list/product-list.component').then((m) => m.ProductListComponent),
        title: 'Shop Wholesale Clothing | Shivkrupa Creation'
      },
      {
        path: 'products/:slug',
        loadComponent: () =>
          import('./features/products/product-detail/product-detail.component').then(
            (m) => m.ProductDetailComponent
          ),
        title: 'Product Details | Shivkrupa Creation'
      },
      {
        path: 'login',
        canActivate: [guestGuard],
        loadComponent: () => import('./features/auth/login/login.component').then((m) => m.LoginComponent),
        title: 'Login | Shivkrupa Creation'
      },
      {
        path: 'register',
        canActivate: [guestGuard],
        loadComponent: () => import('./features/auth/register/register.component').then((m) => m.RegisterComponent),
        title: 'Register | Shivkrupa Creation'
      },
      {
        path: 'profile',
        canActivate: [authGuard],
        loadComponent: () => import('./features/customer/profile/profile.component').then((m) => m.ProfileComponent),
        title: 'My Profile | Shivkrupa Creation'
      },
      {
        path: 'about',
        loadComponent: () => import('./features/static/about/about.component').then((m) => m.AboutComponent),
        title: 'About Us | Shivkrupa Creation'
      },
      {
        path: 'contact',
        loadComponent: () => import('./features/static/contact/contact.component').then((m) => m.ContactComponent),
        title: 'Contact Us | Shivkrupa Creation'
      }
    ]
  },
  {
    path: 'admin',
    component: AdminLayoutComponent,
    canActivate: [adminGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/admin/dashboard/dashboard.component').then((m) => m.AdminDashboardComponent),
        title: 'Admin Dashboard | Shivkrupa Creation'
      },
      {
        path: 'products',
        loadComponent: () =>
          import('./features/admin/products/admin-product-list.component').then((m) => m.AdminProductListComponent),
        title: 'Manage Products | Shivkrupa Creation'
      },
      {
        path: 'products/new',
        loadComponent: () =>
          import('./features/admin/products/admin-product-form.component').then((m) => m.AdminProductFormComponent),
        title: 'New Product | Shivkrupa Creation'
      },
      {
        path: 'products/:id/edit',
        loadComponent: () =>
          import('./features/admin/products/admin-product-form.component').then((m) => m.AdminProductFormComponent),
        title: 'Edit Product | Shivkrupa Creation'
      },
      {
        path: 'categories',
        loadComponent: () =>
          import('./features/admin/categories/admin-categories.component').then((m) => m.AdminCategoriesComponent),
        title: 'Manage Categories | Shivkrupa Creation'
      },
      {
        path: 'brands',
        loadComponent: () =>
          import('./features/admin/brands/admin-brands.component').then((m) => m.AdminBrandsComponent),
        title: 'Manage Brands | Shivkrupa Creation'
      },
      {
        path: 'customers',
        loadComponent: () =>
          import('./features/admin/customers/admin-customers.component').then((m) => m.AdminCustomersComponent),
        title: 'Manage Customers | Shivkrupa Creation'
      },
      {
        path: 'messages',
        loadComponent: () =>
          import('./features/admin/messages/admin-messages.component').then((m) => m.AdminMessagesComponent),
        title: 'Manage Messages | Shivkrupa Creation'
      }
    ]
  },
  { path: '**', redirectTo: '' }
];
