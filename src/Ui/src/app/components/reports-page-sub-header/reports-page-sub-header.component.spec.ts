import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportsPageSubHeaderComponent } from './reports-page-sub-header.component';

describe('ReportsPageSubHeaderComponent', () => {
  let component: ReportsPageSubHeaderComponent;
  let fixture: ComponentFixture<ReportsPageSubHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ReportsPageSubHeaderComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportsPageSubHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
