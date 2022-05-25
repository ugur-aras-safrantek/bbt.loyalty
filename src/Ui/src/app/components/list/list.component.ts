import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {ListService} from "../../services/list.service";
import {Subject} from "rxjs";
import {ActivatedRoute, Router} from "@angular/router";

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})

export class ListComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  @Output() getListEvent = new EventEmitter();
  @Output() downloadExcelFileEvent = new EventEmitter();
  @Output() showDetailEvent = new EventEmitter();

  constructor(private router: Router,
              private route: ActivatedRoute,
              public listService: ListService) {
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

  rowClick(row) {
    if (row.routerLink) {
      this.router.navigate([row.routerLink], {relativeTo: this.route});
    } else {
      this.showDetailEvent.emit(row);
    }
  }

  columnSortDirChange(column) {
    this.listService.columns.map(x => {
      if (x == column) {
        switch (x.sortDir) {
          case null:
            x.sortDir = 'asc';
            break;
          case 'asc':
            x.sortDir = 'desc';
            break;
          case 'desc':
            x.sortDir = null
            break;
        }
      } else {
        x.sortDir = null;
      }
    });

    return column;
  }

  columnNameClick(column) {
    column = this.columnSortDirChange(column);
    this.listService.currentSortDir = column.sortDir;
    this.listService.currentSortBy = column.sortDir
      ? column.propertyName.charAt(0).toUpperCase() + column.propertyName.slice(1)
      : null;

    this.getList();
  }

  changePage(changeValue) {
    this.listService.paging.currentPage = this.listService.paging.currentPage + changeValue;
    this.getList();
  }

  counter(i: number) {
    return new Array(i);
  }
}
