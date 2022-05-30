import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CampaignLimitsAwaitingApprovalListComponent } from './campaign-limits-awaiting-approval-list.component';

describe('CampaignLimitsAwaitingApprovalListComponent', () => {
  let component: CampaignLimitsAwaitingApprovalListComponent;
  let fixture: ComponentFixture<CampaignLimitsAwaitingApprovalListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CampaignLimitsAwaitingApprovalListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CampaignLimitsAwaitingApprovalListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
