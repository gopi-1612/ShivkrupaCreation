export interface ContactRequest {
  name: string;
  email: string;
  phone: string;
  message: string;
}

export interface ContactResponse {
  success: boolean;
  message: string;
}

export interface ContactMessage {
  id: number;
  name: string;
  email: string;
  phone: string;
  message: string;
  isRead: boolean;
  submittedAtUtc: string;
}
