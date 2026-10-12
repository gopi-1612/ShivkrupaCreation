export interface ProductListItem {
  id: number;
  name: string;
  slug: string;
  sku: string;
  mrp: number;
  wholesalePrice: number;
  minWholesaleQuantity: number;
  primaryImageUrl?: string | null;
  categoryName: string;
  brandName: string;
  isActive: boolean;
  isFeatured: boolean;
  averageRating: number;
  inStock: boolean;
}

export interface ProductImage {
  id: number;
  imageUrl: string;
  altText: string;
  isPrimary: boolean;
  displayOrder: number;
}

export interface ProductVideo {
  id: number;
  videoUrl: string;
  thumbnailUrl?: string | null;
  title: string;
  displayOrder: number;
}

export interface ProductVariant {
  id: number;
  color: string;
  colorHex: string;
  size: string;
  sku: string;
  stockQuantity: number;
  priceOverride?: number | null;
  isActive: boolean;
}

export interface ProductDetail {
  id: number;
  name: string;
  slug: string;
  sku: string;
  shortDescription?: string | null;
  description?: string | null;
  fabric: string;
  pattern: string;
  setType?: string | null;
  work: string | null;
  style: string | null;
  bottomStyle: string | null;
  stitchType: string | null;
  sleeveType: string | null;
  neck: string | null;
  occasion: string | null;
  mrp: number;
  wholesalePrice: number;
  minWholesaleQuantity: number;
  stockQuantity: number;
  isActive: boolean;
  isFeatured: boolean;
  averageRating: number;
  categoryId: number;
  categoryName: string;
  brandId: number;
  brandName: string;
  images: ProductImage[];
  videos: ProductVideo[];
  variants: ProductVariant[];
  availableColors: string[];
  availableSizes: string[];
}

export type ProductSortBy = 'Newest' | 'NameAsc' | 'NameDesc' | 'PriceAsc' | 'PriceDesc';

export interface ProductQuery {
  searchTerm?: string;
  categoryId?: number;
  brandId?: number;
  color?: string;
  size?: string;
  minPrice?: number;
  maxPrice?: number;
  isFeatured?: boolean;
  isActive?: boolean;
  sortBy?: ProductSortBy;
  pageNumber?: number;
  pageSize?: number;
}

export interface ProductVariantWrite {
  id?: number | null;
  color: string;
  colorHex: string;
  size: string;
  sku: string;
  stockQuantity: number;
  priceOverride?: number | null;
  isActive: boolean;
}

export interface ProductCreateRequest {
  name: string;
  sku: string;
  shortDescription?: string;
  description?: string;
  fabric: string;
  pattern: string;
  mrp: number;
  wholesalePrice: number;
  minWholesaleQuantity: number;
  stockQuantity: number;
  categoryId: number;
  brandId: number;
  isFeatured: boolean;
  variants: ProductVariantWrite[];
}

export interface ProductUpdateRequest extends ProductCreateRequest {
  isActive: boolean;
}
