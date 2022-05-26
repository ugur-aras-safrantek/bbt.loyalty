import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TargetDefinitionAwaitingApprovalListComponent } from './target-definition-awaiting-approval-list.component';

describe('TargetDefinitionAwaitingApprovalListComponent', () => {
  let component: TargetDefinitionAwaitingApprovalListComponent;
  let fixture: ComponentFixture<TargetDefinitionAwaitingApprovalListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TargetDefinitionAwaitingApprovalListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TargetDefinitionAwaitingApprovalListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
