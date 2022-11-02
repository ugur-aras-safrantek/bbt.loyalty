import { IPagingRequestModel, PagingResponseModel } from "./paging.model";

interface ICampaignReportRequestModel {
  code?: any;
  name?: any;
  viewOptionId?: any;
  startDate?: any;
  endDate?: any;
  isActive?: any;
  isBundle?: any;
  programTypeId?: any;
  achievementTypeId?: any;
  joinTypeId?: any;
  sectorId?: any;
  statusId?: any;
}

export class CampaignReportRequestModel implements ICampaignReportRequestModel, IPagingRequestModel {
  pageNumber: number;
  pageSize: number;
  sortBy?: any;
  sortDir?: any;
  code?: any;
  name?: any;
  viewOptionId?: any;
  startDate?: any;
  endDate?: any;
  isActive?: any;
  isBundle?: any;
  programTypeId?: any;
  achievementTypeId?: any;
  joinTypeId?: any;
  sectorId?: any;
  statusId?: any;
}

interface IEarningReportRequestModel {
  customerCode?: any;
  customerIdentifier?: any;
  customerTypeId?: any;
  campaignStartTermId?: any;
  branchCode?: any;
  achievementTypeId?: any;
  businessLineId?: any;
  isActive?: any;
}

export class EarningReportRequestModel implements IEarningReportRequestModel, IPagingRequestModel {
  pageNumber: number;
  pageSize: number;
  sortBy?: any;
  sortDir?: any;
  customerCode?: any;
  customerIdentifier?: any;
  customerTypeId?: any;
  campaignStartTermId?: any;
  branchCode?: any;
  achievementTypeId?: any;
  businessLineId?: any;
  isActive?: any;
  campaignCode?: any;
}

interface ITargetReportRequestModel {
  campaignId?: any;
  targetId?: any;
  identitySubTypeId?: any;
  isJoin?: any;
  customerCode?: any;
  targetSuccessStartDate?: any;
  targetSuccessEndDate?: any;
}

export class TargetReportRequestModel implements ITargetReportRequestModel, IPagingRequestModel {
  pageNumber: number;
  pageSize: number;
  sortBy?: any;
  sortDir?: any;
  campaignId?: any;
  targetId?: any;
  identitySubTypeId?: any;
  isJoin?: any;
  customerCode?: any;
  targetSuccessStartDate?: any;
  targetSuccessEndDate?: any;
  term?: any;
}

interface ICustomerReportRequestModel {
  customerCode?: any,
  customerIdentifier?: any,
  isActive?: any,
  isExited?: any,
  campaignCode?: any,
}

export class CustomerReportRequestModel implements ICustomerReportRequestModel, IPagingRequestModel{
  pageNumber: number;
  pageSize: number;
  sortBy?: any;
  sortDir?: any;
  customerIdentifier?: any;
  isActive?: any;
  isExited?: any;
  campaignCode?: any;  
}