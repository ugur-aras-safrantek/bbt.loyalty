import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TargetSourceComponent } from './target-source.component';

describe('TargetSourceComponent', () => {
  let component: TargetSourceComponent;
  let fixture: ComponentFixture<TargetSourceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TargetSourceComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TargetSourceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
