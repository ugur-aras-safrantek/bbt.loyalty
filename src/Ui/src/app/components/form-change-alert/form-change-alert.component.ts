import {Component, OnInit} from '@angular/core';
import {NgxSmartModalService} from "ngx-smart-modal";
import {Subject} from "rxjs";

@Component({
  selector: 'app-form-change-alert',
  templateUrl: './form-change-alert.component.html',
  styleUrls: ['./form-change-alert.component.scss']
})

export class FormChangeAlertComponent implements OnInit {
  subject: Subject<boolean>;

  constructor(private modalService: NgxSmartModalService) {
  }

  ngOnInit(): void {
    this.subject = new Subject<boolean>();
  }

  openAlertModal() {
    this.modalService.open('formChangeAlertModal');
  }

  actionAlertModal(value) {
    this.modalService.close('formChangeAlertModal');
    this.subject.next(value);
    if (value)
      this.subject.unsubscribe();
  }
}
