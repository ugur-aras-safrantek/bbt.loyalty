import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerDefinitionContainerComponent } from './customer-definition-container.component';

describe('CustomerDefinitionContainerComponent', () => {
  let component: CustomerDefinitionContainerComponent;
  let fixture: ComponentFixture<CustomerDefinitionContainerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CustomerDefinitionContainerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerDefinitionContainerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
