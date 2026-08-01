export interface BookDto {
  id: string;
  title: string;
  isbn: string;
  description: string;
  publisher: string;
  publishedYear: number;
  language: string;
  imageUrl: string;
  authorId: string;
  categoryId: string;
  authorName: string;
  categoryName: string;
}

export interface CreateBookRequest {
  title: string;
  isbn: string;
  description: string;
  publisher: string;
  publishedYear: number;
  language: string;
  imageUrl: string;
  authorId: string;
  categoryId: string;
}

export interface UpdateBookRequest {
  title: string;
  isbn: string;
  description: string;
  publisher: string;
  publishedYear: number;
  language: string;
  imageUrl: string;
  authorId: string;
  categoryId: string;
}
