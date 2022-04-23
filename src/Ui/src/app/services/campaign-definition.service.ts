import {Injectable} from '@angular/core';
import {environment} from 'src/environments/environment';
import {ApiPaths} from '../models/api-paths';
import {HttpClient, HttpParams} from "@angular/common/http";
import {ApiBaseResponseModel} from '../models/api-base-response.model';
import {GlobalVariable} from "../global";
import {Subject, takeUntil} from "rxjs";
import {UtilityService} from "./utility.service";
import {
  CampaignDefinitionAddRequestModel,
  CampaignDefinitionGainsAddUpdateRequestModel,
  CampaignDefinitionListRequestModel,
  CampaignDefinitionUpdateRequestModel,
  CampaignRulesAddRequestModel,
  CampaignTargetsAddRequestModel
} from "../models/campaign-definition";
import {ToastrHandleService} from 'src/app/services/toastr-handle.service';

@Injectable({
  providedIn: 'root'
})

export class CampaignDefinitionService {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  stepData = [
    {id: 1, title: 'Kampanya Tanımı', isActive: true, passed: false, route: 'definition'},
    {id: 2, title: 'Kampanya Kuralları', isActive: false, passed: false, route: 'rules'},
    {id: 3, title: 'Hedef Seçimi', isActive: false, passed: false, route: 'target-selection'},
    {id: 4, title: 'Kampanya Kazanımlar', isActive: false, passed: false, route: 'gains'},
  ];

  repostData = {
    id: null,
    listLink: GlobalVariable.list,
    previewButtonVisible: true,
    previewLink: GlobalVariable.preview,
    copyModalMessage: 'Mevcut kampanyanın aynısı olacak şekilde yeni bir kampanya tanımı yapılacaktır. Onaylıyor musunuz?'
  };

  isCampaignValuesChanged: boolean = false;

  private baseUrl = environment.baseUrl;

  constructor(private httpClient: HttpClient,
              private toastrHandleService: ToastrHandleService,
              private utilityService: UtilityService) {
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  getProgramTypes() {
    const url = `${this.baseUrl}/${ApiPaths.GetProgramTypes}`;
    return this.httpClient.get<ApiBaseResponseModel>(url);
  }

  campaignDefinitionListGetByFilter(data: CampaignDefinitionListRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.CampaignDefinitionList}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }

  campaignDefinitionListGetByFilterExcelFile(data: CampaignDefinitionListRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.CampaignDefinitionListGetExcelFile}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }

  CampaignDefinitionGetUpdateForm(campaignId: any) {
    let params = new HttpParams();
    params = params.append('id', campaignId);

    const url = `${this.baseUrl}/${ApiPaths.CampaignDefinitionGetUpdateForm}`;
    return this.httpClient.get<ApiBaseResponseModel>(url, {params: params});
  }

  getCampaignInfo(campaignId: any) {
    let params = new HttpParams();
    params = params.append('campaignId', campaignId);

    const url = `${this.baseUrl}/${ApiPaths.GetCampaignInfo}`;
    return this.httpClient.get<ApiBaseResponseModel>(url, {params: params});
  }

  copyCampaign(campaignId: any) {
    let params = new HttpParams();
    params = params.append('campaignId', campaignId);

    const url = `${this.baseUrl}/${ApiPaths.CopyCampaign}`;

    return this.httpClient.get<ApiBaseResponseModel>(url, {params: params})
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.utilityService.redirectTo(`/campaign-definition/update/${res.data.id}/definition`);
            this.toastrHandleService.success();
          } else
            this.toastrHandleService.error(res.errorMessage);
        },
        error: err => {
          if (err.error)
            this.toastrHandleService.error(err.error);
        }
      });
  }

  campaignDefinitionGetInsertForm() {
    const url = `${this.baseUrl}/${ApiPaths.CampaignDefinitionGetInsertForm}`;
    return this.httpClient.get<ApiBaseResponseModel>(url);
  }

  campaignDefinitionAdd(data: CampaignDefinitionAddRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.CampaignDefinitionAdd}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }

  campaignDefinitionUpdate(data: CampaignDefinitionUpdateRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.CampaignDefinitionUpdate}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }

  campaignDefinitionGetContractFile(campaignId: any) {
    let params = new HttpParams();
    params = params.append('campaignId', campaignId);

    const url = `${this.baseUrl}/${ApiPaths.CampaignDefinitionGetContractFile}`;
    return this.httpClient.get<ApiBaseResponseModel>(url, {params: params});
  }

  campaignRulesGetInsertForm() {
    const url = `${this.baseUrl}/${ApiPaths.CampaignRulesGetInsertForm}`;
    return this.httpClient.get<ApiBaseResponseModel>(url);
  }

  getCampaignRules(campaignId: any) {
    let params = new HttpParams();
    params = params.append('campaignId', campaignId);

    const url = `${this.baseUrl}/${ApiPaths.CampaignRulesGetUpdateForm}`;
    return this.httpClient.get<ApiBaseResponseModel>(url, {params: params});
  }

  campaignRulesAdd(data: CampaignRulesAddRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.CampaignRulesAdd}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }

  campaignRulesUpdate(data: CampaignRulesAddRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.CampaignRulesUpdate}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }

  campaignRuleDocumentDownload(campaignId: any) {
    let params = new HttpParams();
    params = params.append('campaignId', campaignId);

    const url = `${this.baseUrl}/${ApiPaths.CampaignRulesGetIdentityFile}`;
    return this.httpClient.get<ApiBaseResponseModel>(url, {params: params});
  }

  campaignTargetsGetInsertForm() {
    const url = `${this.baseUrl}/${ApiPaths.CampaignTargetsGetInsertForm}`;
    return this.httpClient.get<ApiBaseResponseModel>(url);
  }

  campaignTargetsAdd(data: CampaignTargetsAddRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.CampaignTargetsAdd}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }

  campaignTargetsUpdate(data: CampaignTargetsAddRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.CampaignTargetsUpdate}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }

  getCampaignTargets(campaignId: any) {
    let params = new HttpParams();
    params = params.append('campaignId', campaignId);

    const url = `${this.baseUrl}/${ApiPaths.CampaignTargetsGetUpdateForm}`;
    return this.httpClient.get<ApiBaseResponseModel>(url, {params: params});
  }

  campaignDefinitionGainsGetUpdateForm(campaignId: any) {
    let params = new HttpParams();
    params = params.append('campaignId', campaignId);

    const url = `${this.baseUrl}/${ApiPaths.CampaignGainsGetUpdateForm}`;
    return this.httpClient.get<ApiBaseResponseModel>(url, {params: params});
  }

  campaignDefinitionGainsUpdate(data: CampaignDefinitionGainsAddUpdateRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.CampaignGainsUpdate}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }
}
