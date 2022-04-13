import {Component, OnInit,} from '@angular/core';
import {IAngularMyDpOptions} from "angular-mydatepicker";
import {CampaignDefinitionService} from "../../../services/campaign-definition.service";
import {CampaignDefinitionListRequestModel} from "../../../models/campaign-definition";
import {ListService} from "../../../services/list.service";
import {DropdownListModel} from "../../../models/dropdown-list.model";
import {Subject, takeUntil} from "rxjs";
import {saveAs} from 'file-saver';
import {UtilityService} from "../../../services/utility.service";
import {ToastrHandleService} from 'src/app/services/toastr-handle.service';

@Component({
  selector: 'app-campaign-definition-list',
  templateUrl: './campaign-definition-list.component.html',
  styleUrls: ['./campaign-definition-list.component.scss']
})

export class CampaignDefinitionListComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  dpOptions: IAngularMyDpOptions = {
    dateRange: false,
    dateFormat: 'dd-mm-yyyy',
  };
  locale: string = 'tr';

  columns = [
    {columnName: 'Kampanya Adı', propertyName: 'name', isBoolean: false, sortDir: null},
    {columnName: 'Kampanya Kodu', propertyName: 'code', isBoolean: false, sortDir: null},
    {columnName: 'Sözleşme ID', propertyName: 'contractId', isBoolean: false, sortDir: null},
    {columnName: 'Başlama Tarihi', propertyName: 'startDateStr', isBoolean: false, sortDir: null},
    {columnName: 'Bitiş Tarihi', propertyName: 'endDateStr', isBoolean: false, sortDir: null},
    {columnName: 'Program Tipi', propertyName: 'programType', isBoolean: false, sortDir: null},
    {columnName: 'Aktif', propertyName: 'isActive', isBoolean: true, sortDir: null},
    {columnName: 'Birleştirilebilir', propertyName: 'isBundle', isBoolean: true, sortDir: null}
  ];

  programTypeList: DropdownListModel[];

  filterForm = {
    campaignName: '',
    campaignCode: '',
    contractId: '',
    programTypeId: null,
    isActive: null,
    isBundle: null
  };
  startDate: any;
  endDate: any;

  constructor(private campaignDefinitionService: CampaignDefinitionService,
              private toastrHandleService: ToastrHandleService,
              private utilityService: UtilityService,
              private listService: ListService) {
  }

  ngOnInit(): void {
    this.getProgramTypes();
    this.clear();
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();

    this.campaignDefinitionService.isCampaignValuesChanged = false;
  }

  clear() {
    this.filterForm = {
      campaignName: '',
      campaignCode: '',
      contractId: '',
      programTypeId: null,
      isActive: null,
      isBundle: null
    };
    this.startDate = '';
    this.endDate = '';

    this.listService.clearList();

    this.campaignDefinitionListGetByFilter();
  }

  campaignDefinitionListGetByFilter() {
    let requestModel: CampaignDefinitionListRequestModel = {
      pageNumber: this.listService.paging.currentPage,
      pageSize: 10,
      sortBy: this.listService.currentSortBy,
      sortDir: this.listService.currentSortDir,
      campaignName: this.filterForm.campaignName,
      campaignCode: this.filterForm.campaignCode,
      contractId: parseInt(this.filterForm.contractId),
      startDate: this.startDate?.singleDate?.formatted,
      endDate: this.endDate?.singleDate?.formatted,
      programTypeId: this.filterForm.programTypeId,
      isActive: this.filterForm.isActive,
      isBundle: this.filterForm.isBundle
    };
    this.campaignDefinitionService.campaignDefinitionListGetByFilter(requestModel)
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

  campaignDefinitionListGetByFilterExcelFile() {
    let requestModel: CampaignDefinitionListRequestModel = {
      pageNumber: this.listService.paging.currentPage,
      pageSize: 10,
      sortBy: this.listService.currentSortBy,
      sortDir: this.listService.currentSortDir,
      campaignName: this.filterForm.campaignName,
      campaignCode: this.filterForm.campaignCode,
      contractId: parseInt(this.filterForm.contractId),
      startDate: this.startDate?.singleDate?.formatted,
      endDate: this.endDate?.singleDate?.formatted,
      programTypeId: this.filterForm.programTypeId,
      isActive: this.filterForm.isActive,
      isBundle: this.filterForm.isBundle
    };
    this.campaignDefinitionService.campaignDefinitionListGetByFilterExcelFile(requestModel)
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

  getProgramTypes() {
    this.campaignDefinitionService.getProgramTypes()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.programTypeList = res.data;
          } else
            this.toastrHandleService.error(res.errorMessage);
        },
        error: err => {
          if (err.error)
            this.toastrHandleService.error(err.error);
        }
      });
  }

  setRouterLinks(responseList) {
    responseList.map(res => {
      res.routerLink = `../update/${res.id}/definition`;
    });
    return responseList;
  }
}
