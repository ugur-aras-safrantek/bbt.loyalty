import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CampaignDefinitionAwaitingApprovalListComponent } from './campaign-definition-awaiting-approval-list.component';

describe('CampaignDefinitionAwaitingApprovalListComponent', () => {
  let component: CampaignDefinitionAwaitingApprovalListComponent;
  let fixture: ComponentFixture<CampaignDefinitionAwaitingApprovalListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CampaignDefinitionAwaitingApprovalListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CampaignDefinitionAwaitingApprovalListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
