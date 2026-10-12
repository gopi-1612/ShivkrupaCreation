export interface Category {
  id: number;
  name: string;
  slug: string;
  description?: string | null;
  imageUrl?: string | null;
  isActive: boolean;
  displayOrder: number;
  productCount: number;
}

export interface CategoryCreateRequest {
  name: string;
  description?: string;
  imageUrl?: string;
  displayOrder?: number;
}

export interface CategoryUpdateRequest extends CategoryCreateRequest {
  isActive: boolean;
}

export interface Brand {
  id: number;
  name: string;
  slug: string;
  logoUrl?: string | null;
  description?: string | null;
  isActive: boolean;
  productCount: number;
}

export interface BrandCreateRequest {
  name: string;
  logoUrl?: string;
  description?: string;
}

export interface BrandUpdateRequest extends BrandCreateRequest {
  isActive: boolean;
}
