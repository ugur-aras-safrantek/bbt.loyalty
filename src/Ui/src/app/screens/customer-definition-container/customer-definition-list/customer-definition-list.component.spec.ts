import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerDefinitionListComponent } from './customer-definition-list.component';

describe('CustomerDefinitionListComponent', () => {
  let component: CustomerDefinitionListComponent;
  let fixture: ComponentFixture<CustomerDefinitionListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CustomerDefinitionListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerDefinitionListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
