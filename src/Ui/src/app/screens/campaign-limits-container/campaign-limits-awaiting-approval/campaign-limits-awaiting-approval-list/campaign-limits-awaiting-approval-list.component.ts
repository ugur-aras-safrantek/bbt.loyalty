import { Component, OnInit } from '@angular/core';
import {Subject, takeUntil} from "rxjs";
import {UserAuthorizationsModel} from "../../../../models/login.model";
import {DropdownListModel} from "../../../../models/dropdown-list.model";
import {saveAs} from 'file-saver';
import {CampaignLimitsService} from "../../../../services/campaign-limits.service";
import {LoginService} from "../../../../services/login.service";
import {ToastrHandleService} from "../../../../services/toastr-handle.service";
import {UtilityService} from "../../../../services/utility.service";
import {ListService} from "../../../../services/list.service";
import {CampaignLimitsListRequestModel} from "../../../../models/campaign-limits";

@Component({
  selector: 'app-campaign-limits-awaiting-approval-list',
  templateUrl: './campaign-limits-awaiting-approval-list.component.html',
  styleUrls: ['./campaign-limits-awaiting-approval-list.component.scss']
})

export class CampaignLimitsAwaitingApprovalListComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  currentUserAuthorizations: UserAuthorizationsModel = new UserAuthorizationsModel();

  columns = [
    {columnName: 'Çatı Limiti Adı', propertyName: 'name', isBoolean: false, sortDir: null},
    {columnName: 'Kampanya Adı', propertyName: 'campaignNames', isBoolean: false, sortDir: null},
    {columnName: 'Kazanım Sıklığı', propertyName: 'achievementFrequency', isBoolean: false, sortDir: null},
    {columnName: 'Para Birimi', propertyName: 'currency', isBoolean: false, sortDir: null},
    {columnName: 'Çatı Max Tutar', propertyName: 'maxTopLimitAmountStr', isBoolean: false, sortDir: null},
    {columnName: 'Çatı Max Yararlanma', propertyName: 'maxTopLimitUtilization', isBoolean: false, sortDir: null},
    {columnName: 'Çatı Oranı', propertyName: 'maxTopLimitRateStr', isBoolean: false, sortDir: null},
    {columnName: 'Tutar', propertyName: 'amount', isBoolean: true, sortDir: null},
    {columnName: 'Oran', propertyName: 'rate', isBoolean: true, sortDir: null},
    {columnName: 'Aktif', propertyName: 'isActive', isBoolean: true, sortDir: null}
  ];

  achievementFrequencyList: DropdownListModel[];
  currencyTypeList: DropdownListModel[];
  typeList: DropdownListModel[];

  filterForm = {
    name: '',
    achievementFrequencyId: null,
    currencyId: null,
    maxTopLimitAmount: null,
    maxTopLimitUtilization: '',
    maxTopLimitRate: null,
    type: null,
    isActive: null
  };

  constructor(private campaignLimitsService: CampaignLimitsService,
              private loginService: LoginService,
              private toastrHandleService: ToastrHandleService,
              private utilityService: UtilityService,
              private listService: ListService) {
    this.currentUserAuthorizations = this.loginService.getCurrentUserAuthorizations();
  }

  ngOnInit(): void {
    this.getParameterList();
    this.clear();
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  clear() {
    this.filterForm = {
      name: '',
      achievementFrequencyId: null,
      currencyId: null,
      maxTopLimitAmount: null,
      maxTopLimitUtilization: '',
      maxTopLimitRate: null,
      type: null,
      isActive: null
    };

    this.listService.clearList();

    this.campaignLimitsListGetByFilter();
  }

  getParameterList() {
    this.campaignLimitsService.getParameterList()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.achievementFrequencyList = res.data.achievementFrequencyList;
            this.currencyTypeList = res.data.currencyList;
            this.typeList = res.data.typeList;
          } else
            this.toastrHandleService.error(res.errorMessage);
        },
        error: err => {
          if (err.error)
            this.toastrHandleService.error(err.error);
        }
      });
  }

  campaignLimitsListGetByFilter() {
    let requestModel: CampaignLimitsListRequestModel = {
      pageNumber: this.listService.paging.currentPage,
      pageSize: 10,
      sortBy: this.listService.currentSortBy,
      sortDir: this.listService.currentSortDir,
      name: this.filterForm.name,
      achievementFrequencyId: this.filterForm.achievementFrequencyId,
      currencyId: this.filterForm.currencyId,
      maxTopLimitAmount: this.filterForm.maxTopLimitAmount,
      maxTopLimitUtilization: parseInt(this.filterForm.maxTopLimitUtilization),
      maxTopLimitRate: this.filterForm.maxTopLimitRate,
      type: this.filterForm.type,
      isActive: this.filterForm.isActive
    };
    this.campaignLimitsService.campaignLimitsListGetByFilter(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data && res.data.responseList.length > 0) {
            this.listService.setList(this.columns, this.setRouterLinks(res.data.responseList), res.data.paging);
          } else {
            this.listService.setError("Listeleme için uygun kayıt bulunamadı");
          }
        },
        error: err => {
          if (err.error) {
            this.toastrHandleService.error(err.error);
          }
        }
      });
  }

  campaignLimitsListGetByFilterExcelFile() {
    let requestModel: CampaignLimitsListRequestModel = {
      pageNumber: this.listService.paging.currentPage,
      pageSize: 10,
      sortBy: this.listService.currentSortBy,
      sortDir: this.listService.currentSortDir,
      name: this.filterForm.name,
      achievementFrequencyId: this.filterForm.achievementFrequencyId,
      currencyId: this.filterForm.currencyId,
      maxTopLimitAmount: this.filterForm.maxTopLimitAmount,
      maxTopLimitUtilization: parseInt(this.filterForm.maxTopLimitUtilization),
      maxTopLimitRate: this.filterForm.maxTopLimitRate,
      type: this.filterForm.type,
      isActive: this.filterForm.isActive
    };
    this.campaignLimitsService.campaignLimitsListGetByFilterExcelFile(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data?.document) {
            let document = res.data.document;
            let file = this.utilityService.convertBase64ToFile(document.data, document.documentName, document.mimeType);
            saveAs(file, res.data?.document.documentName);
            this.toastrHandleService.success();
          } else {
            this.toastrHandleService.error(res.errorMessage);
          }
        },
        error: err => {
          if (err.error) {
            this.toastrHandleService.error(err.error);
          }
        }
      });
  }

  setRouterLinks(responseList) {
    responseList.map(res => {
      res.routerLink = `../update/${res.id}/limit`;
    });
    return responseList;
  }
}
