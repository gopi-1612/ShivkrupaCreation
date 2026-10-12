import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { Brand, BrandCreateRequest, BrandUpdateRequest } from '../models/category-brand.model';

@Injectable({ providedIn: 'root' })
export class BrandService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/brands`;

  getAll(includeInactive = false): Observable<ApiResponse<Brand[]>> {
    return this.http.get<ApiResponse<Brand[]>>(this.baseUrl, { params: { includeInactive } });
  }

  getById(id: number): Observable<ApiResponse<Brand>> {
    return this.http.get<ApiResponse<Brand>>(`${this.baseUrl}/${id}`);
  }

  create(dto: BrandCreateRequest): Observable<ApiResponse<Brand>> {
    return this.http.post<ApiResponse<Brand>>(this.baseUrl, dto);
  }

  update(id: number, dto: BrandUpdateRequest): Observable<ApiResponse<Brand>> {
    return this.http.put<ApiResponse<Brand>>(`${this.baseUrl}/${id}`, dto);
  }

  setActiveStatus(id: number, isActive: boolean): Observable<ApiResponse<object>> {
    return this.http.patch<ApiResponse<object>>(`${this.baseUrl}/${id}/status`, null, { params: { isActive } });
  }

  delete(id: number): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(`${this.baseUrl}/${id}`);
  }
}
