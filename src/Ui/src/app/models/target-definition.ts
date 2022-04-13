import {IPagingRequestModel} from "./paging.model";

interface ITargetDefinitionListRequestModel {
  name?: any;
  id?: any;
  targetViewTypeId?: any;
  targetSourceId?: any;
  isActive?: any;
}

export class TargetDefinitionListRequestModel implements ITargetDefinitionListRequestModel, IPagingRequestModel {
  pageNumber: number;
  pageSize: number;
  sortBy?: any;
  sortDir?: any;
  name?: any;
  id?: any;
  targetViewTypeId?: any;
  targetSourceId?: any;
  isActive?: any;
}

interface ITargetDefinitionAddRequestModel {
  name?: any;
  title?: any;
  isActive?: any;
}

export class TargetDefinitionAddRequestModel implements ITargetDefinitionAddRequestModel {
  name?: any;
  title?: any;
  isActive?: any;
}

interface ITargetDefinitionUpdateRequestModel extends ITargetDefinitionAddRequestModel {
  id: any;
}

export class TargetDefinitionUpdateRequestModel implements ITargetDefinitionUpdateRequestModel {
  id: any;
  name?: any;
  title?: any;
  isActive?: any;
}

interface ITargetSourceAddUpdateRequestModel {
  targetId?: any;
  targetSourceId?: any;
  targetViewTypeId?: any;
  triggerTimeId?: any;
  verificationTimeId?: any;
  flowName?: any;
  totalAmount?: any;
  numberOfTransaction?: any;
  flowFrequency?: any;
  additionalFlowTime?: any;
  query?: any;
  condition?: any;
  targetDetailEn?: any;
  targetDetailTr?: any;
  descriptionEn?: any;
  descriptionTr?: any;
}

export class TargetSourceAddUpdateRequestModel implements ITargetSourceAddUpdateRequestModel {
  targetId?: any;
  targetSourceId?: any;
  targetViewTypeId?: any;
  triggerTimeId?: any;
  verificationTimeId?: any;
  flowName?: any;
  totalAmount?: any;
  numberOfTransaction?: any;
  flowFrequency?: any;
  additionalFlowTime?: any;
  query?: any;
  condition?: any;
  targetDetailEn?: any;
  targetDetailTr?: any;
  descriptionEn?: any;
  descriptionTr?: any;
}
