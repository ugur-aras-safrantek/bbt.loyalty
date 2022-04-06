import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TargetDefinitionListComponent } from './target-definition-list.component';

describe('TargetDefinitionListComponent', () => {
  let component: TargetDefinitionListComponent;
  let fixture: ComponentFixture<TargetDefinitionListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TargetDefinitionListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TargetDefinitionListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
