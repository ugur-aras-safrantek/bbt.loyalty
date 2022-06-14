export interface ILoginRequestModel {
  code: any;
  state: any;
}

export class LoginRequestModel implements ILoginRequestModel {
  code: any;
  state: any;
}

export interface IAuthorizationModel {
  view: boolean;
  create: boolean;
  update: boolean;
  approve: boolean;
}

export class AuthorizationModel implements IAuthorizationModel {
  view: boolean = false;
  create: boolean = false;
  update: boolean = false;
  approve: boolean = false;
}

export interface IUserAuthorizationsModel {
  campaignDefinitionModuleAuthorizations: IAuthorizationModel;
  campaignLimitsModuleAuthorizations: IAuthorizationModel;
  targetDefinitionModuleAuthorizations: IAuthorizationModel;
  reportsModuleAuthorizations: IAuthorizationModel;
}

export class UserAuthorizationsModel implements IUserAuthorizationsModel {
  campaignDefinitionModuleAuthorizations: AuthorizationModel = new AuthorizationModel();
  campaignLimitsModuleAuthorizations: AuthorizationModel = new AuthorizationModel();
  targetDefinitionModuleAuthorizations: AuthorizationModel = new AuthorizationModel();
  reportsModuleAuthorizations: AuthorizationModel = new AuthorizationModel();
}
