import {IPagingRequestModel} from "./paging.model";

interface ICustomerDefinitionListRequestModel {
  campaignId?: any;
  identitySubTypeId?: any;
  identities?: any;
}

export class CustomerDefinitionListRequestModel implements ICustomerDefinitionListRequestModel, IPagingRequestModel {
  pageNumber: number;
  pageSize: number;
  sortBy?: any;
  sortDir?: any;
  campaignId?: any;
  identitySubTypeId?: any;
  identities?: any;
}

interface ICustomerDefinitionAddUpdateRequestModel {
  campaignId?: any;
  identitySubTypeId?: any;
  isSingleIdentity?: any
  identity?: any;
  file?: any;
}

export class CustomerDefinitionAddUpdateRequestModel implements ICustomerDefinitionAddUpdateRequestModel {
  campaignId?: any;
  identitySubTypeId?: any;
  isSingleIdentity?: any
  identity?: any;
  file?: any;
}

interface ICustomerDefinitionDeleteRequestModel {
  idList?: any;
}

export class CustomerDefinitionDeleteRequestModel implements ICustomerDefinitionDeleteRequestModel {
  idList?: any;
}

