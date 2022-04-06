import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {ListService} from "../../services/list.service";
import {Subject, takeUntil} from "rxjs";

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})

export class ListComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  @Output() getListEvent = new EventEmitter();
  @Output() downloadExcelFileEvent = new EventEmitter();

  constructor(public listService: ListService) {
  }

  ngOnInit(): void {
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  getList() {
    this.getListEvent.emit();
  }

  downloadExcelFile() {
    this.downloadExcelFileEvent.emit();
  }

  changePage(changeValue) {
    this.listService.paging.currentPage = this.listService.paging.currentPage + changeValue;
    this.getList();
  }

  counter(i: number) {
    return new Array(i);
  }
}
