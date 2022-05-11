export interface ILoginRequestModel {
  userId: any;
}

export class LoginRequestModel implements ILoginRequestModel {
  userId: any;
}

export interface IAuthorizationModel {
  view: boolean;
  create: boolean;
  update: boolean;
  approve: boolean;
}

export class AuthorizationModel implements IAuthorizationModel {
  view: boolean;
  create: boolean;
  update: boolean;
  approve: boolean;
}

export interface IUserAuthorizationsModel {
  campaignDefinitionModuleAuthorizations: IAuthorizationModel;
  campaignLimitsModuleAuthorizations: IAuthorizationModel;
  targetDefinitionModuleAuthorizations: IAuthorizationModel;
  reportsModuleAuthorizations: IAuthorizationModel;
}

export class UserAuthorizationsModel implements IUserAuthorizationsModel {
  campaignDefinitionModuleAuthorizations: AuthorizationModel;
  campaignLimitsModuleAuthorizations: AuthorizationModel;
  targetDefinitionModuleAuthorizations: AuthorizationModel;
  reportsModuleAuthorizations: AuthorizationModel;
}
