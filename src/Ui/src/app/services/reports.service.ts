import {Injectable} from '@angular/core';
import {environment} from 'src/environments/environment';
import {ApiPaths} from '../models/api-paths';
import {HttpClient, HttpParams} from "@angular/common/http";
import {ApiBaseResponseModel} from '../models/api-base-response.model';
import {CampaignReportRequestModel, EarningReportRequestModel, TargetReportRequestModel, CustomerReportRequestModel} from "../models/reports";

@Injectable({
  providedIn: 'root'
})

export class ReportsService {
  private baseUrl = environment.baseUrl;

  constructor(private httpClient: HttpClient) {
  }

  getCampaignReportFilterForm() {
    const url = `${this.baseUrl}/${ApiPaths.CampaignReportFilterForm}`;
    return this.httpClient.get<ApiBaseResponseModel>(url);
  }

  campaignReportGetByFilter(data: CampaignReportRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.CampaignReportGetByFilter}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }

  campaignReportGetByFilterExcelFile(data: CampaignReportRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.CampaignReportGetByFilterExcelFile}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }

  getEarningReportFilterForm() {
    const url = `${this.baseUrl}/${ApiPaths.EarningReportFilterForm}`;
    return this.httpClient.get<ApiBaseResponseModel>(url);
  }

  earningReportGetByFilter(data: EarningReportRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.EarningReportGetByFilter}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }

  earningReportGetByFilterExcelFile(data: EarningReportRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.EarningReportGetByFilterExcelFile}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }

  getCustomerReportFilterForm() {
    const url = `${this.baseUrl}/${ApiPaths.CustomerReportFilterForm}`;
    return this.httpClient.get<ApiBaseResponseModel>(url);
  }

  customerReportGetByFilter(data: CustomerReportRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.CustomerReportGetByFilter}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }

  customerReportGetByFilterExcelFile(data: CustomerReportRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.CustomerReportGetByFilterExcelFile}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }

  getCustomerDetail(data: any) {
    const url = `${this.baseUrl}/${ApiPaths.CustomerDetail}/${data.customerCode}/${data.campaignId}?term=${data.term}`;
    return this.httpClient.get<ApiBaseResponseModel>(url);
  }

  getTargetReportFilterForm() {
    const url = `${this.baseUrl}/${ApiPaths.TargetReportFilterForm}`;
    return this.httpClient.get<ApiBaseResponseModel>(url);
  }

  targetReportGetByFilter(data: TargetReportRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.TargetReportGetByFilter}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }

  targetReportGetByFilterExcelFile(data: TargetReportRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.TargetReportGetByFilterExcelFile}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }
}
