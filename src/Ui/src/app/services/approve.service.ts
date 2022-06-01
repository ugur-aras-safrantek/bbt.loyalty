import {Injectable} from '@angular/core';
import {environment} from 'src/environments/environment';
import {ApiPaths} from '../models/api-paths';
import {HttpClient, HttpParams} from "@angular/common/http";
import {ApiBaseResponseModel} from '../models/api-base-response.model';

@Injectable({
  providedIn: 'root'
})

export class ApproveService {
  private baseUrl = environment.baseUrl;

  constructor(private httpClient: HttpClient) {
  }

  campaignDefinitionApproveForm(id: any) {
    let params = new HttpParams();
    params = params.append('id', id);

    const url = `${this.baseUrl}/${ApiPaths.CampaignDefinitionApproveForm}`;
    return this.httpClient.get<ApiBaseResponseModel>(url, {params: params});
  }

  campaignRuleDocumentDownload(id: any) {
    let params = new HttpParams();
    params = params.append('id', id);

    const url = `${this.baseUrl}/${ApiPaths.CampaignRuleDocumentDownload}`;
    return this.httpClient.get<ApiBaseResponseModel>(url, {params: params});
  }

  campaignDefinitionApproveState(state: boolean) {
    const url = `${this.baseUrl}/${ApiPaths.CampaignDefinitionApproveState}/${state}`;
    return this.httpClient.get<ApiBaseResponseModel>(url);
  }
}
