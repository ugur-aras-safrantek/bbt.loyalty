import {Component, OnInit} from '@angular/core';
import {Subject, takeUntil} from "rxjs";
import {saveAs} from 'file-saver';
import {IAngularMyDpOptions} from "angular-mydatepicker";
import {DropdownListModel} from "../../../models/dropdown-list.model";
import {ToastrHandleService} from "../../../services/toastr-handle.service";
import {UtilityService} from "../../../services/utility.service";
import {ListService} from "../../../services/list.service";
import {ReportsService} from "../../../services/reports.service";
import {CampaignReportRequestModel} from "../../../models/reports";

@Component({
  selector: 'app-reports-by-campaign',
  templateUrl: './reports-by-campaign.component.html',
  styleUrls: ['./reports-by-campaign.component.scss']
})

export class ReportsByCampaignComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  dpOptions: IAngularMyDpOptions = {
    dateRange: false,
    dateFormat: 'dd-mm-yyyy',
  };
  locale: string = 'tr';

  columns = [
    {columnName: 'Kampanya Kodu', propertyName: 'code', isBoolean: false, sortDir: null},
    {columnName: 'Kampanya Adı', propertyName: 'name', isBoolean: false, sortDir: null},
    {columnName: 'Başlama Tarihi', propertyName: 'startDateStr', isBoolean: false, sortDir: null},
    {columnName: 'Bitiş Tarihi', propertyName: 'endDateStr', isBoolean: false, sortDir: null},
    {columnName: 'Aktif', propertyName: 'isActive', isBoolean: true, sortDir: null},
    {columnName: 'Birleştirilebilir', propertyName: 'isBundle', isBoolean: true, sortDir: null},
    {columnName: 'Program Tipi', propertyName: 'programTypeName', isBoolean: false, sortDir: null},
    {columnName: 'Sözleşme', propertyName: 'isContract', isBoolean: true, sortDir: null},
    {columnName: 'Sektör', propertyName: 'sectorName', isBoolean: false, sortDir: null},
    {columnName: 'Sıralama', propertyName: 'order', isBoolean: false, sortDir: null},
    {columnName: 'Görüntüleme', propertyName: 'viewOptionName', isBoolean: false, sortDir: null},
    {columnName: 'Dahil Olma Şekli', propertyName: 'joinTypeName', isBoolean: false, sortDir: null},
    {columnName: 'Katılım Sağlanma Adedi', propertyName: 'customerCount', isBoolean: false, sortDir: null},
    {columnName: 'Kazanım Tipi', propertyName: 'achievementTypeName', isBoolean: false, sortDir: null},
    {columnName: 'Kazanım Tutarı', propertyName: 'achievementAmount', isBoolean: false, sortDir: null},
    {columnName: 'Kazanım Oranı', propertyName: 'achievementRate', isBoolean: false, sortDir: null},
    {columnName: 'Çatı Limiti', propertyName: 'topLimitName', isBoolean: false, sortDir: null},
  ];

  viewOptionList: DropdownListModel[];
  programTypeList: DropdownListModel[];
  achievementTypes: DropdownListModel[];
  joinTypeList: DropdownListModel[];
  sectorList: DropdownListModel[];

  filterForm = {
    code: '',
    name: '',
    viewOptionId: null,
    isActive: null,
    isBundle: null,
    programTypeId: null,
    achievementTypeId: null,
    joinTypeId: null,
    sectorId: null,
  };
  startDate: any;
  endDate: any;

  constructor(private reportsService: ReportsService,
              private toastrHandleService: ToastrHandleService,
              private utilityService: UtilityService,
              private listService: ListService) {
  }

  ngOnInit(): void {
    this.getCampaignReportFilterForm();
    this.clear();
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  clear() {
    this.filterForm = {
      code: '',
      name: '',
      viewOptionId: null,
      isActive: null,
      isBundle: null,
      programTypeId: null,
      achievementTypeId: null,
      joinTypeId: null,
      sectorId: null,
    };
    this.startDate = '';
    this.endDate = '';

    this.listService.clearList();

    this.campaignReportGetByFilter();
  }

  campaignReportGetByFilter() {
    let requestModel: CampaignReportRequestModel = {
      pageNumber: this.listService.paging.currentPage,
      pageSize: 10,
      sortBy: this.listService.currentSortBy,
      sortDir: this.listService.currentSortDir,
      code: this.filterForm.code,
      name: this.filterForm.name,
      viewOptionId: this.filterForm.viewOptionId,
      startDate: this.startDate?.singleDate?.formatted,
      endDate: this.endDate?.singleDate?.formatted,
      isActive: this.filterForm.isActive,
      isBundle: this.filterForm.isBundle,
      programTypeId: this.filterForm.programTypeId,
      achievementTypeId: this.filterForm.achievementTypeId,
      joinTypeId: this.filterForm.joinTypeId,
      sectorId: this.filterForm.sectorId,
      statusId: 4,
    };
    this.reportsService.campaignReportGetByFilter(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data && res.data.campaignList.length > 0) {
            this.listService.setList(this.columns, res.data.campaignList, res.data.paging);
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

  campaignReportGetByFilterExcelFile() {
    let requestModel: CampaignReportRequestModel = {
      pageNumber: this.listService.paging.currentPage,
      pageSize: 10,
      sortBy: this.listService.currentSortBy,
      sortDir: this.listService.currentSortDir,
      code: this.filterForm.code,
      name: this.filterForm.name,
      viewOptionId: this.filterForm.viewOptionId,
      startDate: this.startDate?.singleDate?.formatted,
      endDate: this.endDate?.singleDate?.formatted,
      isActive: this.filterForm.isActive,
      isBundle: this.filterForm.isBundle,
      programTypeId: this.filterForm.programTypeId,
      achievementTypeId: this.filterForm.achievementTypeId,
      joinTypeId: this.filterForm.joinTypeId,
      sectorId: this.filterForm.sectorId,
      statusId: 4,
    };
    this.reportsService.campaignReportGetByFilterExcelFile(requestModel)
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

  getCampaignReportFilterForm() {
    this.reportsService.getCampaignReportFilterForm()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.viewOptionList = res.data.viewOptionList;
            this.programTypeList = res.data.programTypeList;
            this.achievementTypes = res.data.achievementTypes;
            this.joinTypeList = res.data.joinTypeList;
            this.sectorList = res.data.sectorList;
          } else
            this.toastrHandleService.error(res.errorMessage);
        },
        error: err => {
          if (err.error)
            this.toastrHandleService.error(err.error);
        }
      });
  }
}
