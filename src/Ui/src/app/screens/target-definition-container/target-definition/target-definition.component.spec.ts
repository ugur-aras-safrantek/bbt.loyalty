import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TargetDefinitionComponent } from './target-definition.component';

describe('TargetDefinitionComponent', () => {
  let component: TargetDefinitionComponent;
  let fixture: ComponentFixture<TargetDefinitionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TargetDefinitionComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TargetDefinitionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
