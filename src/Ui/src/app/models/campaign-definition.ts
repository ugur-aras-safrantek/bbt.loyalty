import {IPagingRequestModel} from "./paging.model";

interface ICampaignDefinitionListRequestModel {
  name?: any;
  code?: any;
  contractId?: any;
  startDate?: any;
  endDate?: any;
  programTypeId?: any;
  isActive?: any;
  isBundle?: any;
}

export class CampaignDefinitionListRequestModel implements ICampaignDefinitionListRequestModel, IPagingRequestModel {
  pageNumber: number;
  pageSize: number;
  sortBy?: any;
  sortDir?: any;
  name?: any;
  code?: any;
  contractId?: any;
  startDate?: any;
  endDate?: any;
  programTypeId?: any;
  isActive?: any;
  isBundle?: any;
}

interface ICampaignDefinitionAddRequestModel {
  name?: any;
  descriptionTr?: any;
  descriptionEn?: any;
  titleTr?: any;
  titleEn?: any;
  startDate?: any;
  endDate?: any;
  order?: any;
  maxNumberOfUser?: any;
  sectorId?: any;
  viewOptionId?: any;
  isActive?: any;
  isContract?: any;
  isBundle?: any;
  contractId?: any;
  programTypeId?: any;
  participationTypeId?: any;
  campaignDetail?: {
    campaignListImageUrl?: any;
    campaignDetailImageUrl?: any;
    summaryTr?: any;
    summaryEn?: any;
    contentTr?: any;
    contentEn?: any;
    detailTr?: any;
    detailEn?: any;
  }
}

export class CampaignDefinitionAddRequestModel implements ICampaignDefinitionAddRequestModel {
  name?: any;
  descriptionTr?: any;
  descriptionEn?: any;
  titleTr?: any;
  titleEn?: any;
  startDate?: any;
  endDate?: any;
  order?: any;
  maxNumberOfUser?: any;
  sectorId?: any;
  viewOptionId?: any;
  isActive?: any;
  isContract?: any;
  isBundle?: any;
  contractId?: any;
  programTypeId?: any;
  participationTypeId?: any;
  campaignDetail?: {
    campaignListImageUrl?: any;
    campaignDetailImageUrl?: any;
    summaryTr?: any;
    summaryEn?: any;
    contentTr?: any;
    contentEn?: any;
    detailTr?: any;
    detailEn?: any;
  }
}

interface ICampaignDefinitionUpdateRequestModel {
  id: any;
  name?: any;
  descriptionTr?: any;
  descriptionEn?: any;
  titleTr?: any;
  titleEn?: any;
  startDate?: any;
  endDate?: any;
  order?: any;
  maxNumberOfUser?: any;
  sectorId?: any;
  viewOptionId?: any;
  isActive?: any;
  isContract?: any;
  isBundle?: any;
  contractId?: any;
  programTypeId?: any;
  participationTypeId?: any;
  campaignDetail?: {
    campaignListImageUrl?: any;
    campaignDetailImageUrl?: any;
    summaryTr?: any;
    summaryEn?: any;
    contentTr?: any;
    contentEn?: any;
    detailTr?: any;
    detailEn?: any;
  }
}

export class CampaignDefinitionUpdateRequestModel implements ICampaignDefinitionUpdateRequestModel {
  id: any;
  name?: any;
  descriptionTr?: any;
  descriptionEn?: any;
  titleTr?: any;
  titleEn?: any;
  startDate?: any;
  endDate?: any;
  order?: any;
  maxNumberOfUser?: any;
  sectorId?: any;
  viewOptionId?: any;
  isActive?: any;
  isContract?: any;
  isBundle?: any;
  contractId?: any;
  programTypeId?: any;
  participationTypeId?: any;
  campaignDetail?: {
    campaignListImageUrl?: any;
    campaignDetailImageUrl?: any;
    summaryTr?: any;
    summaryEn?: any;
    contentTr?: any;
    contentEn?: any;
    detailTr?: any;
    detailEn?: any;
  }
}

interface ICampaignPreviewModel {
  id: any;
  name?: any;
  code?: any;
  descriptionTr?: any;
  descriptionEn?: any;
  titleTr?: any;
  titleEn?: any;
  startDate?: any;
  endDate?: any;
  order?: any;
  maxNumberOfUser?: any;
  sectorId?: any;
  viewOptionId?: any;
  isActive?: any;
  isContract?: any;
  isBundle?: any;
  isApproved?: any;
  contractId?: any;
  programTypeId?: any;
  campaignDetail?: {
    campaignListImageUrl?: any;
    campaignDetailImageUrl?: any;
    summaryTr?: any;
    summaryEn?: any;
    contentTr?: any;
    contentEn?: any;
    detailTr?: any;
    detailEn?: any;
  }
}

export class CampaignPreviewModel implements ICampaignPreviewModel {
  id: any;
  name?: any;
  code?: any;
  descriptionTr?: any;
  descriptionEn?: any;
  titleTr?: any;
  titleEn?: any;
  startDate?: any;
  endDate?: any;
  order?: any;
  maxNumberOfUser?: any;
  sectorId?: any;
  viewOptionId?: any;
  isActive?: any;
  isContract?: any;
  isBundle?: any;
  isApproved?: any;
  contractId?: any;
  programTypeId?: any;
  campaignDetail?: {
    campaignListImageUrl?: any;
    campaignDetailImageUrl?: any;
    summaryTr?: any;
    summaryEn?: any;
    contentTr?: any;
    contentEn?: any;
    detailTr?: any;
    detailEn?: any;
  }
}

interface ICampaignRulesAddRequestModel {
  campaignId?: any;
  joinTypeId?: any;
  isSingleIdentity?: any
  identity?: any;
  file?: any;
  businessLines?: any;
  branches?: any;
  customerTypes?: any;
  startTermId?: any;
}

export class CampaignRulesAddRequestModel implements ICampaignRulesAddRequestModel {
  campaignId?: any;
  joinTypeId?: any;
  isSingleIdentity?: any
  identity?: any;
  file?: any;
  businessLines?: any;
  branches?: any;
  customerTypes?: any;
  startTermId?: any;
}

interface ICampaignTarget {
  id: any;
  name: any;
}

export class CampaignTarget implements ICampaignTarget {
  id: any;
  name: any;
}

interface ICampaignTargetGroup {
  id: any;
  targetList: ICampaignTarget[];
}

export class CampaignTargetGroup implements ICampaignTargetGroup {
  id: any;
  targetList: CampaignTarget[];

  constructor() {
    this.targetList = new Array<CampaignTarget>();
  }
}

interface ICampaignTargetsAddRequestModel {
  campaignId?: any;
  targetList?: any[];
}

export class CampaignTargetsAddRequestModel implements ICampaignTargetsAddRequestModel {
  campaignId?: any;
  targetList?: any[];
}

interface ICampaignDefinitionGainChannelsAddUpdateRequestModel {
  campaignId?: any;
  campaignChannelCodeList?: any[];
}

export class CampaignDefinitionGainChannelsAddUpdateRequestModel implements ICampaignDefinitionGainChannelsAddUpdateRequestModel {
  campaignId?: any;
  campaignChannelCodeList?: any[];
}

interface ICampaignDefinitionGainModel {
  id: any;
  fakeId: any;
  campaignId?: any;
  type?: any;
  achievementTypeId?: any;
  actionOptionId?: any;
  titleTr?: any;
  titleEn?: any;
  descriptionTr?: any;
  descriptionEn?: any;
  currencyId?: any;
  maxAmount?: any;
  amount?: any;
  rate?: any;
  maxUtilization?: any;
}

export class CampaignDefinitionGainModel implements ICampaignDefinitionGainModel {
  id: any;
  fakeId: any;
  campaignId?: any;
  type?: any;
  achievementTypeId?: any;
  actionOptionId?: any;
  titleTr?: any;
  titleEn?: any;
  descriptionTr?: any;
  descriptionEn?: any;
  currencyId?: any;
  maxAmount?: any;
  amount?: any;
  rate?: any;
  maxUtilization?: any;
}

interface ICampaignDefinitionGainsAddUpdateRequestModel {
  campaignId?: any;
  campaignAchievementList?: ICampaignDefinitionGainModel[];
}

export class CampaignDefinitionGainsAddUpdateRequestModel implements ICampaignDefinitionGainsAddUpdateRequestModel {
  campaignId?: any;
  campaignAchievementList?: CampaignDefinitionGainModel[];
}

