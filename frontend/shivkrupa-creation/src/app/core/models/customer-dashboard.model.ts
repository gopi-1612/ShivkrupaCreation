export interface CustomerListItem {
  id: string;
  fullName: string;
  email: string;
  phoneNumber?: string | null;
  businessName?: string | null;
  city?: string | null;
  state?: string | null;
  isActive: boolean;
  createdAt: string;
}

export interface CustomerDetail extends CustomerListItem {
  gstNumber?: string | null;
  address?: string | null;
  pinCode?: string | null;
}

export interface DashboardStats {
  totalProducts: number;
  activeProducts: number;
  inactiveProducts: number;
  outOfStockProducts: number;
  totalCategories: number;
  totalBrands: number;
  totalCustomers: number;
  activeCustomers: number;
  newCustomersThisMonth: number;
  productsByCategory: { categoryName: string; productCount: number }[];
  recentlyAddedProducts: {
    id: number;
    name: string;
    primaryImageUrl?: string | null;
    wholesalePrice: number;
    createdAt: string;
  }[];
}
