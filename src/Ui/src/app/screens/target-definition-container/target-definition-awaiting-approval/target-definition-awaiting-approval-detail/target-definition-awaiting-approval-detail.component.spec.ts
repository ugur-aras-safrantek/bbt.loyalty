import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TargetDefinitionAwaitingApprovalDetailComponent } from './target-definition-awaiting-approval-detail.component';

describe('TargetDefinitionAwaitingApprovalDetailComponent', () => {
  let component: TargetDefinitionAwaitingApprovalDetailComponent;
  let fixture: ComponentFixture<TargetDefinitionAwaitingApprovalDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TargetDefinitionAwaitingApprovalDetailComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TargetDefinitionAwaitingApprovalDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
