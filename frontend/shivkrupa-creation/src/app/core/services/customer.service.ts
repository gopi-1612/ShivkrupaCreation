import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, PagedResult } from '../models/api-response.model';
import {
  ChangePasswordRequest,
  CustomerProfile,
  UpdateProfileRequest
} from '../models/user.model';
import { CustomerDetail, CustomerListItem } from '../models/customer-dashboard.model';

@Injectable({ providedIn: 'root' })
export class CustomerService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/customers`;

  // ---------- Admin ----------

  getAll(
    search?: string,
    isActive?: boolean,
    pageNumber = 1,
    pageSize = 20
  ): Observable<ApiResponse<PagedResult<CustomerListItem>>> {
    let params = new HttpParams().set('pageNumber', pageNumber).set('pageSize', pageSize);
    if (search) params = params.set('search', search);
    if (isActive !== undefined) params = params.set('isActive', isActive);

    return this.http.get<ApiResponse<PagedResult<CustomerListItem>>>(this.baseUrl, { params });
  }

  getById(id: string): Observable<ApiResponse<CustomerDetail>> {
    return this.http.get<ApiResponse<CustomerDetail>>(`${this.baseUrl}/${id}`);
  }

  setActiveStatus(id: string, isActive: boolean): Observable<ApiResponse<object>> {
    return this.http.patch<ApiResponse<object>>(`${this.baseUrl}/${id}/status`, { isActive });
  }

  // ---------- Self-service ----------

  getMyProfile(): Observable<ApiResponse<CustomerProfile>> {
    return this.http.get<ApiResponse<CustomerProfile>>(`${this.baseUrl}/me`);
  }

  updateMyProfile(dto: UpdateProfileRequest): Observable<ApiResponse<CustomerProfile>> {
    return this.http.put<ApiResponse<CustomerProfile>>(`${this.baseUrl}/me`, dto);
  }

  changeMyPassword(dto: ChangePasswordRequest): Observable<ApiResponse<object>> {
    return this.http.post<ApiResponse<object>>(`${this.baseUrl}/me/change-password`, dto);
  }
}
