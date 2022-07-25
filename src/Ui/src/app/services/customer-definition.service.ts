import {Injectable} from '@angular/core';
import {environment} from 'src/environments/environment';
import {ApiPaths} from '../models/api-paths';
import {HttpClient, HttpParams} from "@angular/common/http";
import {ApiBaseResponseModel} from '../models/api-base-response.model';
import {Subject} from "rxjs";
import {
  CustomerDefinitionAddUpdateRequestModel,
  CustomerDefinitionDeleteRequestModel,
  CustomerDefinitionListRequestModel,
} from "../models/customer-definition";

@Injectable({
  providedIn: 'root'
})

export class CustomerDefinitionService {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  private baseUrl = environment.baseUrl;

  constructor(private httpClient: HttpClient) {
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  getFilterFormByList() {
    const url = `${this.baseUrl}/${ApiPaths.CustomerDefinitionGetUpdateForm}`;
    return this.httpClient.get<ApiBaseResponseModel>(url);
  }

  customerDefinitionListGetByFilter(data: CustomerDefinitionListRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.CustomerDefinitionList}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }

  customerDefinitionListGetByFilterExcelFile(data: CustomerDefinitionListRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.CustomerDefinitionListGetExcelFile}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }

  customerDefinitionGetUpdateForm(customerId: any) {
    let params = new HttpParams();
    params = params.append('id', customerId);

    const url = `${this.baseUrl}/${ApiPaths.CustomerDefinitionGetUpdateForm}`;
    return this.httpClient.get<ApiBaseResponseModel>(url, {params: params});
  }

  customerDefinitionAddUpdate(data: CustomerDefinitionAddUpdateRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.CustomerDefinitionAddUpdate}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }

  customerDefinitionDelete(data: CustomerDefinitionDeleteRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.CustomerDefinitionDelete}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }
}
