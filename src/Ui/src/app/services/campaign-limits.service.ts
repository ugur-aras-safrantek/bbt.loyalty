import {Injectable} from '@angular/core';
import {Step} from "../models/step.model";
import {environment} from "../../environments/environment";
import {HttpClient, HttpParams} from "@angular/common/http";
import {ApiPaths} from "../models/api-paths";
import {ApiBaseResponseModel} from "../models/api-base-response.model";
import {GlobalVariable} from "../global";
import {Subject, takeUntil} from "rxjs";
import {UtilityService} from "./utility.service";
import {
  CampaignLimitsListRequestModel,
  CampaignLimitAddRequestModel,
  CampaignLimitUpdateRequestModel
} from '../models/campaign-limits';
import {ToastrService} from "ngx-toastr";

@Injectable({
  providedIn: 'root'
})

export class CampaignLimitsService {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  stepData = [
    {id: 1, title: 'Kampanya Çatı Limitleri', isActive: true, passed: false, route: 'limit'},
    {id: 2, title: 'Tamamlandı', isActive: false, passed: false, route: 'limit'},
  ];

  repostData = {
    id: null,
    listLink: GlobalVariable.limitList,
    previewButtonVisible: false,
    previewLink: GlobalVariable.limitPreview,
    copyModalMessage: 'Mevcut çatı limitinin aynısı olacak şekilde yeni bir çatı limiti tanımı yapılacaktır. Onaylıyor musunuz?',
    isFormChanged: false
  };

  private baseUrl = environment.baseUrl;

  constructor(private httpClient: HttpClient,
              private toastrService: ToastrService,
              private utilityService: UtilityService) {
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  limitFormChanged(value: boolean) {
    this.repostData.isFormChanged = value;
  }

  getParameterList() {
    const url = `${this.baseUrl}/${ApiPaths.CampaignLimitsGetParameterList}`;
    return this.httpClient.get<ApiBaseResponseModel>(url);
  }

  campaignLimitsListGetByFilter(data: CampaignLimitsListRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.CampaignLimitsGetByFilter}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }

  campaignLimitsListGetByFilterExcelFile(data: CampaignLimitsListRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.CampaignLimitsListGetExcelFile}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }

  getLimitDetail(limitId: any) {
    let params = new HttpParams();
    params = params.append('id', limitId);

    const url = `${this.baseUrl}/${ApiPaths.GetLimitDetail}`;
    return this.httpClient.get<ApiBaseResponseModel>(url, {params: params});
  }

  copyLimit(limitId: any) {
    let params = new HttpParams();
    params = params.append('topLimitId', limitId);

    const url = `${this.baseUrl}/${ApiPaths.CopyLimit}`;

    return this.httpClient.get<ApiBaseResponseModel>(url, {params: params})
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.utilityService.redirectTo(`${GlobalVariable.limitUpdate}/${res.data.id}/limit`);
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

  campaignLimitGetInsertForm() {
    const url = `${this.baseUrl}/${ApiPaths.CampaignLimitGetInsertForm}`;
    return this.httpClient.get<ApiBaseResponseModel>(url);
  }

  campaignLimitAdd(data: CampaignLimitAddRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.CampaignLimitAdd}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }

  campaignLimitUpdate(data: CampaignLimitUpdateRequestModel) {
    const url = `${this.baseUrl}/${ApiPaths.CampaignLimitUpdate}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, data);
  }
}
