import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {NgxSmartModalService} from "ngx-smart-modal";

@Component({
  selector: 'app-repost',
  templateUrl: './repost.component.html',
  styleUrls: ['./repost.component.scss']
})

export class RepostComponent implements OnInit {
  @Input('repostData') repostData: any;

  @Output() copyItemEvent = new EventEmitter<any>();

  constructor(private modalService: NgxSmartModalService,
              private router: Router,
              private route: ActivatedRoute) {
  }

  ngOnInit(): void {
  }

  ngOnDestroy() {
  }

  backToList() {
    if (this.repostData.isFormChanged) {
      this.openAlertModal();
    } else {
      this.router.navigate([this.repostData.listLink], {relativeTo: this.route});
    }
  }

  openAlertModal() {
    this.modalService.open('alertModal');
  }

  closeAlertModal() {
    this.modalService.close('alertModal');
  }

  alertModalOk() {
    this.router.navigate([this.repostData.listLink], {relativeTo: this.route});
  }

  openCopyModal() {
    this.modalService.open('copyModal');
  }

  closeCopyModal() {
    this.modalService.close('copyModal');
  }

  copyModalOk() {
    this.copy();
  }

  private copy() {
    this.copyItemEvent.emit({id: this.repostData.id});
  }
}
