import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, PagedResult } from '../models/api-response.model';
import {
  ProductCreateRequest,
  ProductDetail,
  ProductImage,
  ProductListItem,
  ProductQuery,
  ProductUpdateRequest,
  ProductVideo
} from '../models/product.model';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/products`;

  getProducts(query: ProductQuery): Observable<ApiResponse<PagedResult<ProductListItem>>> {
    let params = new HttpParams();

    Object.entries(query).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== '') {
        params = params.set(key, String(value));
      }
    });

    return this.http.get<ApiResponse<PagedResult<ProductListItem>>>(this.baseUrl, { params });
  }

  getBySlug(slug: string): Observable<ApiResponse<ProductDetail>> {
    return this.http.get<ApiResponse<ProductDetail>>(`${this.baseUrl}/slug/${slug}`);
  }

  getById(id: number): Observable<ApiResponse<ProductDetail>> {
    return this.http.get<ApiResponse<ProductDetail>>(`${this.baseUrl}/${id}`);
  }

  create(dto: ProductCreateRequest): Observable<ApiResponse<ProductDetail>> {
    return this.http.post<ApiResponse<ProductDetail>>(this.baseUrl, dto);
  }

  update(id: number, dto: ProductUpdateRequest): Observable<ApiResponse<ProductDetail>> {
    return this.http.put<ApiResponse<ProductDetail>>(`${this.baseUrl}/${id}`, dto);
  }

  setActiveStatus(id: number, isActive: boolean): Observable<ApiResponse<object>> {
    return this.http.patch<ApiResponse<object>>(`${this.baseUrl}/${id}/status`, null, { params: { isActive } });
  }

  delete(id: number): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(`${this.baseUrl}/${id}`);
  }

  uploadImage(productId: number, formData: FormData): Observable<ApiResponse<ProductImage>> {
    return this.http.post<ApiResponse<ProductImage>>(`${this.baseUrl}/${productId}/images`, formData);
  }

  removeImage(productId: number, imageId: number): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(`${this.baseUrl}/${productId}/images/${imageId}`);
  }

  uploadVideo(productId: number, file: File, title?: string): Observable<ApiResponse<ProductVideo>> {
    const formData = new FormData();
    formData.append('file', file);
    if (title) formData.append('title', title);

    return this.http.post<ApiResponse<ProductVideo>>(`${this.baseUrl}/${productId}/videos`, formData);
  }

  removeVideo(productId: number, videoId: number): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(`${this.baseUrl}/${productId}/videos/${videoId}`);
  }

  /** Resolves a relative image/video URL from the API into an absolute one for <img>/<video> src. */
  resolveMediaUrl(relativeUrl?: string | null): string {
    if (!relativeUrl) return '';
    if (relativeUrl.startsWith('http')) return relativeUrl;
    return `${environment.uploadsBaseUrl}${relativeUrl}`;
  }
}
