import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CampaignLimitsAwaitingApprovalDetailComponent } from './campaign-limits-awaiting-approval-detail.component';

describe('CampaignLimitsAwaitingApprovalDetailComponent', () => {
  let component: CampaignLimitsAwaitingApprovalDetailComponent;
  let fixture: ComponentFixture<CampaignLimitsAwaitingApprovalDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CampaignLimitsAwaitingApprovalDetailComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CampaignLimitsAwaitingApprovalDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
