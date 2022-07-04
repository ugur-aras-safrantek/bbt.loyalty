import {Component, OnInit} from '@angular/core';
import {Subject, takeUntil} from "rxjs";
import {DropdownListModel} from "../../../models/dropdown-list.model";
import {UtilityService} from "../../../services/utility.service";
import {ListService} from "../../../services/list.service";
import {saveAs} from 'file-saver';
import {TargetDefinitionService} from "../../../services/target-definition.service";
import {TargetDefinitionListRequestModel} from "../../../models/target-definition";
import {ToastrHandleService} from 'src/app/services/toastr-handle.service';
import {LoginService} from 'src/app/services/login.service';
import {UserAuthorizationsModel} from "../../../models/login.model";

@Component({
  selector: 'app-target-definition-list',
  templateUrl: './target-definition-list.component.html',
  styleUrls: ['./target-definition-list.component.scss']
})

export class TargetDefinitionListComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  currentUserAuthorizations: UserAuthorizationsModel = new UserAuthorizationsModel();

  columns = [
    {columnName: 'Hedef Tanımı Adı', propertyName: 'name', isBoolean: false, sortDir: null},
    {columnName: 'Hedef ID', propertyName: 'targetId', isBoolean: false, sortDir: null},
    {columnName: 'Hedef Gösterim Tipi', propertyName: 'targetViewType', isBoolean: false, sortDir: null},
    {columnName: 'Akış', propertyName: 'flow', isBoolean: true, sortDir: null},
    {columnName: 'Sorgu', propertyName: 'query', isBoolean: true, sortDir: null},
    {columnName: 'Aktif', propertyName: 'isActive', isBoolean: true, sortDir: null}
  ];

  targetViewTypeList: DropdownListModel[];
  targetSourceList: DropdownListModel[];

  filterForm = {
    name: '',
    id: '',
    targetViewTypeId: null,
    targetSourceId: null,
    isActive: null
  };

  isSentToApprovalRecord: boolean = false;

  constructor(private targetDefinitionService: TargetDefinitionService,
              private loginService: LoginService,
              private toastrHandleService: ToastrHandleService,
              private utilityService: UtilityService,
              private listService: ListService) {
    this.currentUserAuthorizations = this.loginService.getCurrentUserAuthorizations();
  }

  ngOnInit(): void {
    this.getListParameters();
    this.clear();
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();

    this.targetDefinitionService.isTargetValuesChanged = false;
  }

  clear() {
    this.filterForm = {
      name: '',
      id: '',
      targetViewTypeId: null,
      targetSourceId: null,
      isActive: null
    };

    this.listService.clearList();

    this.targetDefinitionListGetByFilter();
  }

  getListParameters() {
    this.getTargetViewTypes();
    this.getTargetSources();
  }

  private getTargetViewTypes() {
    this.targetDefinitionService.getTargetViewTypes()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.targetViewTypeList = res.data;
          } else
            this.toastrHandleService.error(res.errorMessage);
        },
        error: err => {
          if (err.error)
            this.toastrHandleService.error(err.error);
        }
      });
  }

  private getTargetSources() {
    this.targetDefinitionService.getTargetSources()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data) {
            this.targetSourceList = res.data;
          } else
            this.toastrHandleService.error(res.errorMessage);
        },
        error: err => {
          if (err.error)
            this.toastrHandleService.error(err.error);
        }
      });
  }

  targetDefinitionListGetByFilter() {
    let requestModel: TargetDefinitionListRequestModel = {
      pageNumber: this.listService.paging.currentPage,
      pageSize: 10,
      sortBy: this.listService.currentSortBy,
      sortDir: this.listService.currentSortDir,
      name: this.filterForm.name,
      id: parseInt(this.filterForm.id),
      targetViewTypeId: this.filterForm.targetViewTypeId,
      targetSourceId: this.filterForm.targetSourceId,
      isActive: this.filterForm.isActive,
      statusId: 4,
    };
    this.targetDefinitionService.targetDefinitionListGetByFilter(requestModel)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (!res.hasError && res.data && res.data.responseList.length > 0) {
            this.isSentToApprovalRecord = res.data.isSentToApprovalRecord;
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

  targetDefinitionListGetByFilterExcelFile() {
    let requestModel: TargetDefinitionListRequestModel = {
      pageNumber: this.listService.paging.currentPage,
      pageSize: 10,
      sortBy: this.listService.currentSortBy,
      sortDir: this.listService.currentSortDir,
      name: this.filterForm.name,
      id: parseInt(this.filterForm.id),
      targetViewTypeId: this.filterForm.targetViewTypeId,
      targetSourceId: this.filterForm.targetSourceId,
      isActive: this.filterForm.isActive,
      statusId: 4,
    };
    this.targetDefinitionService.targetDefinitionListGetByFilterExcelFile(requestModel)
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
      res.routerLink = `../update/${res.id}/definition`;
    });
    return responseList;
  }
}
