import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TargetDefinitionContainerComponent } from './target-definition-container.component';

describe('TargetDefinitionContainerComponent', () => {
  let component: TargetDefinitionContainerComponent;
  let fixture: ComponentFixture<TargetDefinitionContainerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TargetDefinitionContainerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TargetDefinitionContainerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
