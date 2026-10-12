import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { Category, CategoryCreateRequest, CategoryUpdateRequest } from '../models/category-brand.model';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/categories`;

  getAll(includeInactive = false): Observable<ApiResponse<Category[]>> {
    return this.http.get<ApiResponse<Category[]>>(this.baseUrl, { params: { includeInactive } });
  }

  getById(id: number): Observable<ApiResponse<Category>> {
    return this.http.get<ApiResponse<Category>>(`${this.baseUrl}/${id}`);
  }

  create(dto: FormData): Observable<ApiResponse<Category>> {
    return this.http.post<ApiResponse<Category>>(this.baseUrl, dto);
  }

  update(id: number, dto: FormData): Observable<ApiResponse<Category>> {
    return this.http.put<ApiResponse<Category>>(`${this.baseUrl}/${id}`, dto);
  }

  setActiveStatus(id: number, isActive: boolean): Observable<ApiResponse<object>> {
    return this.http.patch<ApiResponse<object>>(`${this.baseUrl}/${id}/status`, null, { params: { isActive } });
  }

  delete(id: number): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(`${this.baseUrl}/${id}`);
  }
  /** Resolves a relative image/video URL from the API into an absolute one for <img>/<video> src. */
  resolveImageUrl(relativeUrl?: string | null): string {
    if (!relativeUrl) return '';
    if (relativeUrl.startsWith('http')) return relativeUrl;
    return `${environment.uploadsBaseUrl}${relativeUrl}`;
  }
}
