import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CampaignDefinitionAwaitingApprovalDetailComponent } from './campaign-definition-awaiting-approval-detail.component';

describe('CampaignDefinitionAwaitingApprovalDetailComponent', () => {
  let component: CampaignDefinitionAwaitingApprovalDetailComponent;
  let fixture: ComponentFixture<CampaignDefinitionAwaitingApprovalDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CampaignDefinitionAwaitingApprovalDetailComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CampaignDefinitionAwaitingApprovalDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
