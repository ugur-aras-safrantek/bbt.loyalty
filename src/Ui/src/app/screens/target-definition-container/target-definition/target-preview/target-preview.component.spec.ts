import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TargetPreviewComponent } from './target-preview.component';

describe('TargetPreviewComponent', () => {
  let component: TargetPreviewComponent;
  let fixture: ComponentFixture<TargetPreviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TargetPreviewComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TargetPreviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
