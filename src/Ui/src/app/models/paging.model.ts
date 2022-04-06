export interface IPagingRequestModel {
  pageNumber: number;
  pageSize: number;
}

export interface IPagingResponseModel {
  currentPage: number;
  totalPages: number;
  totalItems: number;
}

export class PagingResponseModel implements IPagingResponseModel {
  currentPage: number;
  totalPages: number;
  totalItems: number;
}
