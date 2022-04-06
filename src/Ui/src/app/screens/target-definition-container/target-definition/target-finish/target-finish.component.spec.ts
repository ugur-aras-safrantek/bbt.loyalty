import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TargetFinishComponent } from './target-finish.component';

describe('TargetFinishComponent', () => {
  let component: TargetFinishComponent;
  let fixture: ComponentFixture<TargetFinishComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TargetFinishComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TargetFinishComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
