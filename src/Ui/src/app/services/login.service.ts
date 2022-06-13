import {Injectable} from '@angular/core';
import {environment} from "../../environments/environment";
import {HttpClient, HttpParams} from "@angular/common/http";
import {ApiPaths} from "../models/api-paths";
import {ApiBaseResponseModel} from "../models/api-base-response.model";
import {AuthorizationModel, LoginRequestModel, UserAuthorizationsModel} from "../models/login.model";

@Injectable({
  providedIn: 'root'
})

export class LoginService {
  private baseUrl = environment.baseUrl;

  private currentUserAuthorizations: UserAuthorizationsModel = new UserAuthorizationsModel();

  constructor(private httpClient: HttpClient) {
  }

  getAccessToken() {
    return JSON.parse(sessionStorage.getItem('accessToken') || '');
  }

  getUserLoginInfo() {
    return JSON.parse(sessionStorage.getItem('isLogin') || 'false');
  }

  getCurrentUserAuthorizations() {
    return JSON.parse(sessionStorage.getItem('currentUserAuthorizations') || '{}');
  }

  setCurrentUserAuthorizations(accessToken: any, userData: any[]) {
    this.currentUserAuthorizations = new UserAuthorizationsModel();
    userData.map(x => {
      this.setAuthorization(x);
    })
    sessionStorage.setItem('isLogin', 'true');
    sessionStorage.setItem('accessToken', JSON.stringify(accessToken));
    sessionStorage.setItem('currentUserAuthorizations', JSON.stringify(this.currentUserAuthorizations));
  }

  logout() {
    sessionStorage.removeItem('isLogin');
    sessionStorage.removeItem('accessToken');
    sessionStorage.removeItem('currentUserAuthorizations');
  }

  login(data: LoginRequestModel) {
    let params = new HttpParams();
    params = params.append('code', data.code);
    params = params.append('state', data.state);

    const url = `${this.baseUrl}/${ApiPaths.Login}`;
    return this.httpClient.post<ApiBaseResponseModel>(url, {}, {params: params});
  }

  private setAuthorization(module) {
    switch (module.moduleId) {
      case 1:
        this.currentUserAuthorizations.campaignDefinitionModuleAuthorizations = this.setAuthorizationModel(module.authorizationList);
        break;
      case 2:
        this.currentUserAuthorizations.campaignLimitsModuleAuthorizations = this.setAuthorizationModel(module.authorizationList);
        break;
      case 3:
        this.currentUserAuthorizations.targetDefinitionModuleAuthorizations = this.setAuthorizationModel(module.authorizationList);
        break;
      case 4:
        this.currentUserAuthorizations.reportsModuleAuthorizations = this.setAuthorizationModel(module.authorizationList);
        break;
    }
  }

  private setAuthorizationModel(authorizationList: any[]) {
    let authorizationModel: AuthorizationModel = {
      create: false,
      update: false,
      view: false,
      approve: false
    };
    authorizationList.map(x => {
      switch (x) {
        case 1:
          authorizationModel.create = true;
          break;
        case 2:
          authorizationModel.update = true;
          break;
        case 3:
          authorizationModel.view = true;
          break;
        case 4:
          authorizationModel.approve = true;
          break;
      }
    })
    return authorizationModel;
  }
}
