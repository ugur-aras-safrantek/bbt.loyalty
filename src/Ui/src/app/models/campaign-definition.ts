import {IPagingRequestModel} from "./paging.model";

interface ICampaignDefinitionListRequestModel {
  campaignName?: any;
  campaignCode?: any;
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
  campaignName?: any;
  campaignCode?: any;
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

interface ICampaignGainModel {
  id?: any;
  type?: any;
  achievementType?: any;
  action?: any;
  maxUtilization?: any;
  typeId?: any;
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
}

export class CampaignGainModel implements ICampaignGainModel {
  id?: any;
  type?: any;
  achievementType?: any;
  action?: any;
  maxUtilization?: any;
  typeId?: any;
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
}

interface ICampaignGainChannelModel {
  campaignChannelCode?: any;
  campaignChannelName?: any;
  hasAchievement?: any;
  achievementList: ICampaignGainModel[];
}

export class CampaignGainChannelModel implements ICampaignGainChannelModel {
  campaignChannelCode?: any;
  campaignChannelName?: any;
  hasAchievement?: any;
  achievementList: CampaignGainModel[];

  constructor() {
    this.achievementList = new Array<CampaignGainModel>()
  }
}

interface ICampaignDefinitionGainsAddRequestModel {
  campaignId?: any;
  channelsAndAchievements: ICampaignGainChannelModel[];
}

export class CampaignDefinitionGainsAddRequestModel implements ICampaignDefinitionGainsAddRequestModel {
  campaignId?: any;
  channelsAndAchievements: CampaignGainChannelModel[];

  constructor() {
    this.channelsAndAchievements = new Array<CampaignGainChannelModel>()
  }
}

