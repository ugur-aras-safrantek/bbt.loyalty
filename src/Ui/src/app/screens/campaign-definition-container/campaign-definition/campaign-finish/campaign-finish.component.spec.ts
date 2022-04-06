import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CampaignFinishComponent } from './campaign-finish.component';

describe('CampaignFinishComponent', () => {
  let component: CampaignFinishComponent;
  let fixture: ComponentFixture<CampaignFinishComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CampaignFinishComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CampaignFinishComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
