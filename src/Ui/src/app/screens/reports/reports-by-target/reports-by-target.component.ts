import {Component, OnInit} from '@angular/core';
import {Subject, takeUntil} from "rxjs";
import {saveAs} from 'file-saver';
import {IAngularMyDpOptions} from "angular-mydatepicker";
import {DropdownListModel} from "../../../models/dropdown-list.model";
import {ToastrHandleService} from "../../../services/toastr-handle.service";
import {UtilityService} from "../../../services/utility.service";
import {ListService} from "../../../services/list.service";
import {ReportsService} from "../../../services/reports.service";
import {TargetReportRequestModel} from "../../../models/reports";

@Component({
  selector: 'app-reports-by-target',
  templateUrl: './reports-by-target.component.html',
  styleUrls: ['./reports-by-target.component.scss']
})

export class ReportsByTargetComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  dpOptions: IAngularMyDpOptions = {
    dateRange: false,
    dateFormat: 'dd-mm-yyyy',
  };
  locale: string = 'tr';

  columns = [
    {columnName: 'Hedef Adı', propertyName: 'targetName', isBoolean: false, sortDir: null},
    {columnName: 'Kampanya / Program', propertyName: 'campaignName', isBoolean: false, sortDir: null},
    {columnName: 'Programa Dahil Mi?', propertyName: 'isIncludedProgram', isBoolean: false, sortDir: null},
    {columnName: 'Müşteri No', propertyName: 'customerNo', isBoolean: false, sortDir: null},
    {columnName: 'Alt Kırılım', propertyName: 'identitySubTypeName', isBoolean: false, sortDir: null},
    {columnName: 'Harcama Hedefi', propertyName: '', isBoolean: false, sortDir: null},
    {columnName: 'Hedef Tuttu Mu?', propertyName: '', isBoolean: false, sortDir: null},
    {columnName: 'Kalan Harcama', propertyName: '', isBoolean: false, sortDir: null},
    {columnName: 'Hedefin Gerçekleştiği Tarih', propertyName: '', isBoolean: false, sortDir: null},
  ];

  campaignList: DropdownListModel[];
  targetList: DropdownListModel[];
  identitySubTypeList: DropdownListModel[];

  filterForm = {
    campaignId: null,
    targetId: null,
    identitySubTypeId: null,
    isIncludedProgram: null,
    customerNo: '',
  };
  startDate: any;
  endDate: any;

  constructor(private reportsService: ReportsService,
              private toastrHandleService: ToastrHandleService,
              private utilityService: UtilityService,
              private listService: ListService) {
  }

  ngOnInit(): void {
    this.getTargetReportFilterForm();
    this.clear();
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  clear() {
    this.filterForm = {
      campaignId: null,
      targetId: null,
      identitySubTypeId: null,
      isIncludedProgram: null,
      customerNo: '',
    };
    this.startDate = '';
    this.endDate = '';

    this.listService.clearList();

    this.targetReportGetByFilter();
  }

  targetReportGetByFilter() {
    let requestModel: TargetReportRequestModel = {
      pageNumber: this.listService.paging.currentPage,
      pageSize: 10,
      sortBy: this.listService.currentSortBy,
      sortDir: this.listService.currentSortDir,
      campaignId: this.filterForm.campaignId,
      targetId: this.filterForm.targetId,
      identitySubTypeId: this.filterForm.identitySubTypeId,
      isIncludedProgram: this.filterForm.isIncludedProgram,
      customerNo: this.filterForm.customerNo,
      startDate: this.startDate?.singleDate?.formatted,
      endDate: this.endDate?.singleDate?.formatted,
      statusId: 4,
    };
    this.reportsService.targetReportGetByFilter(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data && res.data.targetList.length > 0) {
            this.listService.setList(this.columns, res.data.targetList, res.data.paging);
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

  targetReportGetByFilterExcelFile() {
    let requestModel: TargetReportRequestModel = {
      pageNumber: this.listService.paging.currentPage,
      pageSize: 10,
      sortBy: this.listService.currentSortBy,
      sortDir: this.listService.currentSortDir,
      campaignId: this.filterForm.campaignId,
      targetId: this.filterForm.targetId,
      identitySubTypeId: this.filterForm.identitySubTypeId,
      isIncludedProgram: this.filterForm.isIncludedProgram,
      customerNo: this.filterForm.customerNo,
      startDate: this.startDate?.singleDate?.formatted,
      endDate: this.endDate?.singleDate?.formatted,
      statusId: 4,
    };
    this.reportsService.targetReportGetByFilterExcelFile(requestModel)
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

  getTargetReportFilterForm() {
    this.reportsService.getTargetReportFilterForm()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.campaignList = res.data.campaignList;
            this.targetList = res.data.targetList;
            this.identitySubTypeList = res.data.identitySubTypeList;
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
