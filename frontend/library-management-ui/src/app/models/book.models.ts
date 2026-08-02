export interface BookDto {
  id: string;
  title: string;
  isbn: string;
  description: string | null;
  publisher: string | null;
  publishedYear: number | null;
  language: string | null;
  imageUrl: string | null;
  authorName: string;
  categoryName: string;
  totalCopies: number;
  availableCopies: number;
  createdAt: string;
  [key: string]: unknown;
}

export interface CreateBookRequest {
  title: string;
  isbn: string;
  description?: string;
  publisher?: string;
  publishedYear?: number;
  language?: string;
  imageUrl?: string;
  authorId: string;
  categoryId: string;
}

export interface UpdateBookRequest {
  title: string;
  description?: string;
  publisher?: string;
  publishedYear?: number;
  language?: string;
  imageUrl?: string;
  categoryId?: string;
}

export interface BookSearchResponse {
  id: string;
  title: string;
  isbn: string;
  authorName: string;
  categoryName: string;
  availableCopies: number;
  totalCopies: number;
}
