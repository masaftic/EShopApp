
export type PaginatedList<T> = {
  items: T[];
  totalCount: number;
  pageSize: number;
  pageNumber: number;
  totalPages: number;
};
