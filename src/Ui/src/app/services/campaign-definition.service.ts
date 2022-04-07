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
import {ToastrService} from "ngx-toastr";

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
    copyModalMessage: 'Mevcut kampanyanın aynısı olacak şekilde yeni bir kampanya tanımı yapılacaktır. Onaylıyor musunuz?',
    isFormChanged: false
  };

  isCampaignValuesChanged: boolean = false;

  private baseUrl = environment.baseUrl;

  constructor(private httpClient: HttpClient,
              private toastrService: ToastrService,
              private utilityService: UtilityService) {
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  campaignFormChanged(value: boolean) {
    this.repostData.isFormChanged = value;
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

  getCampaignDetail(campaignId: any) {
    const url = `${this.baseUrl}/${ApiPaths.GetCampaignDetail}/${campaignId}`;
    return this.httpClient.get<ApiBaseResponseModel>(url);
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
            this.utilityService.redirectTo(`/campaign-definition/create/${res.data.id}/true/definition`);
            this.toastrService.success("İşlem başarılı");
          } else
            this.toastrService.error(res.errorMessage);
        },
        error: err => {
          if (err.error)
            this.toastrService.error(err.error.title);
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

  getCampaignDefinitionGainsGetInsertForm(campaignId: any) {
    let params = new HttpParams();
    params = params.append('campaignId', campaignId);

    const url = `${this.baseUrl}/${ApiPaths.CampaignGainsGetInsertForm}`;
    return this.httpClient.get<ApiBaseResponseModel>(url, {params: params});
  }

  getCampaignDefinitionGain(campaignId: any) {
    let params = new HttpParams();
    params = params.append('campaignId', campaignId);

    const url = `${this.baseUrl}/${ApiPaths.CampaignGainsGetUpdateForm}`;
    return this.httpClient.get<ApiBaseResponseModel>(url, {params: params});
  }

  campaignDefinitionGainsAdd(data: CampaignDefinitionGainsAddUpdateRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.CampaignGainsAdd}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }

  campaignDefinitionGainsUpdate(data: CampaignDefinitionGainsAddUpdateRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.CampaignGainsUpdate}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }
}
