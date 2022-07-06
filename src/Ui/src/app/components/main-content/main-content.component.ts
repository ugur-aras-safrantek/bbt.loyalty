import {Component, Input, OnInit} from '@angular/core';
import {AuthorizationModel} from "../../models/login.model";

@Component({
  selector: 'app-main-content',
  templateUrl: './main-content.component.html',
  styleUrls: ['./main-content.component.scss']
})
export class MainContentComponent implements OnInit {
  @Input('blockTitle') blockTitle = '';
  @Input('createRoute') createRoute = '';
  @Input('newButtonText') newButtonText = '';
  @Input('isAwaitingApprovalListPage') isAwaitingApprovalListPage: boolean = false;
  @Input('isSentToApprovalRecord') isSentToApprovalRecord: boolean = false;
  @Input('authorization') authorization: AuthorizationModel = new AuthorizationModel();
  @Input('isReportsPage') isReportsPage: boolean = false;

  awaitingApprovalListRoute = '../awaiting-approval/list';
  returnListRoute = './list';

  constructor() {
  }

  ngOnInit(): void {
  }

}
