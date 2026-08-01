export interface Result<T> {
  success: boolean;
  data: T | null;
  errors: string[] | null;
  statusCode: number;
}

export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}
