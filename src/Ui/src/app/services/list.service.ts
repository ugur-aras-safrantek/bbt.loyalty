import {Injectable} from '@angular/core';
import {Column} from "../models/list.model";
import {PagingResponseModel} from "../models/paging.model";

@Injectable({
  providedIn: 'root'
})

export class ListService {
  columns: Column[];
  rows: any[];

  currentSortBy: null;
  currentSortDir: null;

  paging: PagingResponseModel = {
    currentPage: 1,
    totalPages: 1,
    totalItems: 0,
  }

  hasError: boolean = false;
  errorMessage: string = '';

  constructor() {
  }

  clearTable(){
    this.columns = [];
    this.rows = [];
  }

  clearList() {
    this.clearTable();
    this.paging = {
      currentPage: 1,
      totalPages: 1,
      totalItems: 0,
    };
    this.currentSortBy = null;
    this.currentSortDir = null;
    this.hasError = false;
    this.errorMessage = '';
  }

  setList(columns: Column[], rows: any[], paging: any) {
    this.hasError = false;
    this.errorMessage = '';

    this.columns = columns;
    this.rows = rows;
    this.paging = paging;
  }

  setError(errorMessage: string) {
    this.clearList();

    this.hasError = true;
    this.errorMessage = errorMessage;
  }
}
