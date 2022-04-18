import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FormChangeAlertComponent } from './form-change-alert.component';

describe('FormChangeAlertComponent', () => {
  let component: FormChangeAlertComponent;
  let fixture: ComponentFixture<FormChangeAlertComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FormChangeAlertComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FormChangeAlertComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
