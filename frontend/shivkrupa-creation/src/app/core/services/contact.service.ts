import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ContactMessage, ContactRequest, ContactResponse } from '../models/contact.model';

@Injectable({ providedIn: 'root' })
export class ContactService {
  private readonly baseUrl = `${environment.apiUrl}/contact`;

  constructor(private http: HttpClient) { }

  submit(request: ContactRequest): Observable<ContactResponse> {
    return this.http.post<ContactResponse>(this.baseUrl, request);
  }

  getMessages(): Observable<ContactMessage[]> {
    return this.http.get<ContactMessage[]>(this.baseUrl);
  }

  markRead(id: number): Observable<ContactMessage> {
    return this.http.patch<ContactMessage>(`${this.baseUrl}/${id}/read`, {});
  }

  deleteMessage(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
