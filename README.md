# Shivkrupa Creation — Wholesale Clothing E-Commerce Platform

**Tagline:** "Style Ka Naya Andaz"

Stack: Angular + Bootstrap 5 (frontend) · ASP.NET Core 8 Web API (backend) · SQL Server + EF Core · JWT auth with Admin/Customer roles.

This project is being built **phase-by-phase**. Each phase adds working, reviewable code on top of the last.

## Build Roadmap

- [x] **Phase 1:** Solution structure, EF Core domain models (Category, Brand, Product, ProductImage, ProductVideo, ProductVariant), ApplicationUser/Identity setup, ApplicationDbContext with fluent configuration, appsettings, role/admin seeder, Program.cs skeleton.
- [x] **Phase 2:** JWT authentication (register/login/refresh/logout), RefreshToken entity, Auth DTOs + FluentValidation validators, AutoMapper profile, centralized `ApiException`-based error-handling middleware, `ApiResponse<T>`/`PagedResult<T>` envelopes, full `Program.cs` wiring (JWT bearer auth, role policies, Swagger with Bearer auth, DI).
- [x] **Phase 3:** Category/Brand/Product CRUD REST APIs, product search/filter/sort/pagination, secure admin-only image & video upload, product variant (color/size) management, customer management + self-service profile APIs, admin dashboard statistics.
- [x] **Phase 4 (this delivery):** Full Angular workspace — routing, JWT + error HTTP interceptors, auth/admin/guest route guards, core services for every backend endpoint, shared Bootstrap navbar/footer/toast/layouts, **and** every customer + admin page (built ahead of schedule): Home, Product listing (search/filter/sort/pagination), Product details (fully custom responsive image carousel), Login/Register/Profile, About/Contact, and the full Admin panel (Dashboard, Product CRUD with variants + image/video upload, Category CRUD, Brand CRUD, Customer management).
- [ ] **Phase 5 / 6 (optional polish):** Visual refinement pass, richer empty/error states, unit tests, deployment configs (Docker/CI) — see "What's left" below.

## Phase 1 — What's included

```
backend/ShivkrupaCreation.API/
├── ShivkrupaCreation.API.csproj      # EF Core, Identity, JWT, FluentValidation, AutoMapper, Swagger packages
├── appsettings.json                  # Connection string, JWT settings, upload settings, CORS origins
├── appsettings.Development.json
├── Models/
│   ├── ApplicationUser.cs            # extends IdentityUser; Roles.Admin / Roles.Customer constants
│   ├── Category.cs
│   ├── Brand.cs
│   ├── Product.cs                    # includes WholesalePrice, MinWholesaleQuantity, StockQuantity
│   ├── ProductVariant.cs             # color + size + own SKU/stock/price override
│   ├── ProductImage.cs               # ImageUrl, AltText, IsPrimary, DisplayOrder (drives carousel order)
│   └── ProductVideo.cs
└── Data/
    ├── ApplicationDbContext.cs       # IdentityDbContext<ApplicationUser> + fluent config, indexes, decimals
    └── DbSeeder.cs                   # seeds Admin/Customer roles + default admin user
```

## Phase 2 — What's included

```
backend/ShivkrupaCreation.API/
├── Program.cs                        # full wiring: EF Core, Identity, JWT bearer auth, role policies,
│                                      #   CORS, AutoMapper, FluentValidation, Swagger+Bearer, exception middleware
├── Models/
│   └── RefreshToken.cs               # backs the refresh-token rotation flow
├── Settings/
│   └── JwtSettings.cs                # strongly-typed options bound from appsettings JwtSettings section
├── DTOs/
│   ├── Common/ApiResponse.cs         # ApiResponse<T> envelope + PagedResult<T> (used by every endpoint)
│   ├── Common/ApiException.cs        # custom exception carrying an HTTP status code
│   └── Auth/AuthDtos.cs              # RegisterDto, LoginDto, RefreshTokenRequestDto, UserDto, AuthResponseDto
├── Validators/
│   └── AuthValidators.cs             # FluentValidation rules for Register/Login (email, password, GST, phone)
├── Mapping/
│   └── AutoMapperProfile.cs          # ApplicationUser -> UserDto
├── Services/
│   ├── Interfaces/ITokenService.cs, IAuthService.cs
│   ├── TokenService.cs               # generates JWT access tokens + refresh tokens, validates expired tokens
│   └── AuthService.cs                # register/login/refresh/logout business logic
├── Middleware/
│   └── ExceptionHandlingMiddleware.cs # centralized error handling -> uniform ApiResponse JSON errors
└── Controllers/
    └── AuthController.cs             # POST /api/auth/register, /login, /refresh, /logout
```

`frontend/` folder is reserved for the Angular workspace, created in Phase 4.

### API endpoints added in Phase 2

| Method | Route                 | Auth       | Description                                  |
|--------|------------------------|------------|-----------------------------------------------|
| POST   | `/api/auth/register`   | Anonymous  | Create a Customer account, returns JWT pair   |
| POST   | `/api/auth/login`      | Anonymous  | Login, returns JWT access + refresh token     |
| POST   | `/api/auth/refresh`    | Anonymous  | Exchange expired access token + refresh token |
| POST   | `/api/auth/logout`     | Authorized | Revoke a refresh token                        |

## Phase 3 — What's included

```
backend/ShivkrupaCreation.API/
├── Settings/FileUploadSettings.cs    # bound from appsettings FileUploadSettings section
├── Helpers/SlugHelper.cs             # generates URL-safe slugs for categories/brands/products
├── DTOs/
│   ├── Category/CategoryDtos.cs
│   ├── Brand/BrandDtos.cs
│   ├── Product/ProductDtos.cs        # list/detail/create/update/variant/image/video/query DTOs
│   ├── Customer/CustomerDtos.cs      # admin list/detail + self-profile + change-password
│   └── Dashboard/DashboardStatsDto.cs
├── Validators/
│   ├── CategoryBrandValidators.cs
│   └── ProductValidators.cs          # includes per-variant validation
├── Services/
│   ├── Interfaces/ (ICategoryService, IBrandService, IProductService, ICustomerService,
│   │                IDashboardService, IFileUploadService)
│   ├── FileUploadService.cs          # extension/size allow-list, random file names, path-traversal safe
│   ├── CategoryService.cs / BrandService.cs   # CRUD + slug uniqueness + delete-guard when products linked
│   ├── ProductService.cs             # search/filter/sort/pagination, CRUD, variant sync, image/video mgmt
│   ├── CustomerService.cs            # admin listing/status + self profile/password change
│   └── DashboardService.cs           # aggregated admin statistics
└── Controllers/
    ├── CategoriesController.cs       # public GET, admin POST/PUT/PATCH(status)/DELETE
    ├── BrandsController.cs           # same pattern as Categories
    ├── ProductsController.cs         # public search/detail, admin CRUD + image/video upload endpoints
    ├── CustomersController.cs        # admin management + /me self-service endpoints
    └── DashboardController.cs        # admin-only GET /api/dashboard/stats
```

All responses (Phase 2 and Phase 3 endpoints alike) use the `ApiResponse<T>` envelope: `{ success, message, data, errors }`. Paginated list endpoints wrap `data` in `PagedResult<T>`: `{ items, pageNumber, pageSize, totalCount, totalPages, hasPreviousPage, hasNextPage }`.

### API endpoints added in Phase 3

| Method | Route                                   | Auth       | Description                                    |
|--------|-------------------------------------------|------------|-------------------------------------------------|
| GET    | `/api/categories`                         | Public     | List categories (`?includeInactive=`)          |
| POST/PUT/DELETE | `/api/categories/{id}`           | Admin      | Category CRUD                                   |
| PATCH  | `/api/categories/{id}/status`             | Admin      | Activate/deactivate                              |
| GET    | `/api/brands`                             | Public     | List brands                                      |
| POST/PUT/DELETE | `/api/brands/{id}`               | Admin      | Brand CRUD                                       |
| GET    | `/api/products`                           | Public     | Search/filter/sort/paginate (`ProductQueryDto`)  |
| GET    | `/api/products/slug/{slug}`               | Public     | Product details for the details page             |
| POST/PUT/DELETE | `/api/products/{id}`             | Admin      | Product CRUD (incl. variants)                    |
| POST   | `/api/products/{id}/images`               | Admin      | Upload image (multipart: file, altText, isPrimary) |
| DELETE | `/api/products/{id}/images/{imageId}`     | Admin      | Remove image                                     |
| POST   | `/api/products/{id}/videos`               | Admin      | Upload video (multipart: file, title)            |
| DELETE | `/api/products/{id}/videos/{videoId}`     | Admin      | Remove video                                     |
| GET    | `/api/customers`                          | Admin      | Paginated customer list (`?search=&isActive=`)   |
| PATCH  | `/api/customers/{id}/status`              | Admin      | Activate/deactivate a customer                   |
| GET/PUT | `/api/customers/me`                      | Customer   | View/update own profile                          |
| POST   | `/api/customers/me/change-password`       | Customer   | Change own password                              |
| GET    | `/api/dashboard/stats`                    | Admin      | Dashboard statistics                             |

Uploaded files are stored under `wwwroot/uploads/products/{images|videos}` and served as static files at `/uploads/...`, so `ProductImage.ImageUrl` / `ProductVideo.VideoUrl` values are ready to use directly as `<img src>` / `<video src>` in Angular.

`frontend/` folder is reserved for the Angular workspace, created in Phase 4.

## Running the backend locally

Prerequisites: .NET 8 SDK, SQL Server (LocalDB or full instance).

```bash
cd backend/ShivkrupaCreation.API

# restore & build
dotnet restore
dotnet build

# update the connection string in appsettings.json to match your SQL Server instance

# create the first migration (adds Identity + domain tables)
dotnet ef migrations add InitialCreate

# apply it (Program.cs also auto-applies migrations + seeds on startup)
dotnet ef database update

dotnet run
```

Swagger UI will be available at `https://localhost:{port}/swagger` once run.

**Default seeded admin login** (change immediately in production):
- Email: `admin@shivkrupacreation.com`
- Password: `Admin@123`

⚠️ **Before running:** open `appsettings.json` and replace `JwtSettings:Secret` with your own long random string (32+ characters) — never ship the placeholder value.

## Phase 4 — Frontend (Angular) — What's included

```
frontend/shivkrupa-creation/
├── package.json, angular.json, tsconfig*.json
├── src/
│   ├── index.html, main.ts, styles.scss        # brand color palette (maroon/gold/ivory) + Bootstrap overrides
│   ├── environments/                            # apiUrl / uploadsBaseUrl for dev & prod
│   └── app/
│       ├── app.config.ts                        # provideRouter, provideHttpClient(withInterceptors(...))
│       ├── app.routes.ts                        # full route table, lazy-loaded standalone components
│       ├── core/
│       │   ├── models/                          # TS interfaces mirroring every backend DTO
│       │   ├── services/                        # AuthService (signals), Category/Brand/Product/Customer/
│       │   │                                     #   Dashboard services, ToastService
│       │   ├── interceptors/                     # jwt.interceptor.ts (attach + silent refresh),
│       │   │                                     #   error.interceptor.ts (ApiResponse -> toast)
│       │   └── guards/                            # authGuard, adminGuard, guestGuard
│       ├── shared/
│       │   ├── components/                       # navbar, footer, toast-container, loading-spinner, empty-state
│       │   └── layouts/                           # customer-layout (navbar+outlet+footer), admin-layout (sidebar)
│       └── features/
│           ├── home/                              # hero w/ tagline, categories grid, featured products
│           ├── products/
│           │   ├── product-list/                  # search, category/brand/price filters, sort, pagination
│           │   └── product-detail/                 # ★ custom responsive image carousel (see below)
│           ├── auth/login/, auth/register/         # reactive forms, password match validation
│           ├── customer/profile/                   # view/update profile + change password
│           ├── static/about/, static/contact/       # About Us, Contact Us (client-validated enquiry form)
│           └── admin/
│               ├── dashboard/                      # stat cards, category breakdown, recent products
│               ├── products/                       # list w/ search+pagination, create/edit form w/ variants
│               │                                    #   (FormArray) + image/video upload
│               ├── categories/, brands/             # list + modal create/edit + activate/deactivate + delete
│               └── customers/                       # searchable list + activate/deactivate
```

### The product image carousel (product-detail page)

Built to spec as a self-contained, framework-controlled carousel (Bootstrap-styled, not dependent on
Bootstrap's JS carousel instance so it stays in sync with the thumbnail row):

- Main preview cross-fades between slides (`opacity`/`transform` CSS transition) — no page reload
- Previous/Next controls plus dot indicators, all driving the same `activeImageIndex` signal
- Clickable thumbnail strip below the main image; the active thumbnail is visually highlighted
- Full keyboard support — arrow keys move the carousel when it has focus (`tabindex="0"` + `(keydown)`)
- Every image uses `alt` text from `ProductImage.altText`; thumbnails and non-first slides use
  `loading="lazy"`
- A dedicated empty state (dashed placeholder box) renders when a product has no images
- Color swatches / size buttons below the carousel update price, SKU, and stock live from the
  selected `ProductVariant` — still no page reload

### Running the frontend locally

Prerequisites: Node.js 18+ and npm.

```bash
cd frontend/shivkrupa-creation
npm install
npm start          # ng serve — runs on http://localhost:4200
```

Before running, confirm `src/environments/environment.ts` points `apiUrl` at your running backend
(defaults to `https://localhost:7000/api`, matching the Phase 1–3 backend's default Kestrel port —
adjust to whatever port `dotnet run` actually binds to on your machine).

For a production build:

```bash
npm run build:prod   # outputs to dist/shivkrupa-creation
```

## Next step

The application is functionally complete end-to-end: backend APIs (auth, catalog, uploads, customers, dashboard) and a full Angular frontend (customer storefront + admin panel) are both in place. What's left is optional polish — deeper responsive/visual QA, unit/e2e tests, Docker/CI deployment configs, and a real backend `/api/contact` endpoint for the Contact Us form (it currently validates client-side only). Let me know if you'd like me to continue with any of those, or with a specific fix/feature.

> **A note on "Angular 22":** at the time this was built, Angular's latest stable major was v18. Angular 22 doesn't exist yet in my training data, so this project is scaffolded with the latest APIs and patterns I know to be stable — standalone components (no NgModules), signals for state, the new `@if`/`@for` control-flow syntax, and functional guards/interceptors — all of which are forward-compatible with recent and upcoming Angular releases. When you run `npm install`, npm will resolve to whatever Angular 18.x (or newer, if you bump the versions in `package.json`) is actually published; run `ng update` afterwards if you want to move to a newer major.
