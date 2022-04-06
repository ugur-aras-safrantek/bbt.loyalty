import {IPagingRequestModel} from "./paging.model";

interface ICampaignLimitsListRequestModel {
  name?: any;
  achievementFrequencyId?: any;
  currencyId?: any;
  maxTopLimitAmount?: any;
  maxTopLimitRate?: any;
  maxTopLimitUtilization?: any;
  type?: any;
  isActive?: any;
  isApproved?: any;
  isDraft?: any;
}

export class CampaignLimitsListRequestModel implements ICampaignLimitsListRequestModel, IPagingRequestModel {
  pageNumber: number;
  pageSize: number;
  name?: any;
  achievementFrequencyId?: any;
  currencyId?: any;
  maxTopLimitAmount?: any;
  maxTopLimitRate?: any;
  maxTopLimitUtilization?: any;
  type?: any;
  isActive?: any;
  isApproved?: any;
  isDraft?: any;
}

interface ICampaignLimitAddRequestModel {
  name?: any;
  isActive?: any;
  campaignIds?: any;
  achievementFrequencyId?: any;
  type?: any;
  currencyId?: any;
  maxTopLimitAmount?: any;
  maxTopLimitRate?: any;
  maxTopLimitUtilization?: any;
}

export class CampaignLimitAddRequestModel implements ICampaignLimitAddRequestModel {
  name?: any;
  isActive?: any;
  campaignIds?: any;
  achievementFrequencyId?: any;
  type?: any;
  currencyId?: any;
  maxTopLimitAmount?: any;
  maxTopLimitRate?: any;
  maxTopLimitUtilization?: any;
}

interface ICampaignLimitUpdateRequestModel extends ICampaignLimitAddRequestModel {
  id: any;
}

export class CampaignLimitUpdateRequestModel implements ICampaignLimitUpdateRequestModel {
  id: any;
  name?: any;
  isActive?: any;
  campaignIds?: any;
  achievementFrequencyId?: any;
  type?: any;
  currencyId?: any;
  maxTopLimitAmount?: any;
  maxTopLimitRate?: any;
  maxTopLimitUtilization?: any;
}



