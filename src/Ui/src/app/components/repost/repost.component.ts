import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {NgxSmartModalService} from "ngx-smart-modal";
import {AuthorizationModel} from "../../models/login.model";

@Component({
  selector: 'app-repost',
  templateUrl: './repost.component.html',
  styleUrls: ['./repost.component.scss']
})

export class RepostComponent implements OnInit {
  @Input('repostData') repostData: any;
  @Input('authority') authority: AuthorizationModel;

  @Output() copyItemEvent = new EventEmitter<any>();
  @Output() previewItemEvent = new EventEmitter<any>();

  constructor(private modalService: NgxSmartModalService,
              private router: Router,
              private route: ActivatedRoute) {
  }

  ngOnInit(): void {
  }

  ngOnDestroy() {
  }

  backToList() {
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
    this.modalService.close('copyModal');
  }

  preview() {
    if (this.previewItemEvent.observed) {
      this.previewItemEvent.emit({id: this.repostData.id});
    } else {
      let baseUrl = window.location.href.replace(this.router.url, '');
      let previewUrl = `${this.repostData.previewLink}/${this.repostData.id}`;
      window.open(baseUrl + previewUrl, '_blank');
    }
  }

  private copy() {
    this.copyItemEvent.emit({id: this.repostData.id});
  }
}
