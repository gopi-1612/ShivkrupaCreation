/** Mirrors the backend's ApiResponse<T> envelope (DTOs/Common/ApiResponse.cs). */
export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T | null;
  errors: string[] | null;
}

/** Mirrors the backend's PagedResult<T> envelope. */
export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
