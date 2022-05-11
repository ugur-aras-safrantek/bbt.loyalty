import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuthCheckComponent } from './auth-check.component';

describe('AuthCheckComponent', () => {
  let component: AuthCheckComponent;
  let fixture: ComponentFixture<AuthCheckComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AuthCheckComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AuthCheckComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
