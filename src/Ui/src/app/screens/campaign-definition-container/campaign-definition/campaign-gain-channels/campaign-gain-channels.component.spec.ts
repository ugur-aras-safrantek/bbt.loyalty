import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CampaignGainChannelsComponent } from './campaign-gain-channels.component';

describe('CampaignGainChannelComponent', () => {
  let component: CampaignGainChannelsComponent;
  let fixture: ComponentFixture<CampaignGainChannelsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CampaignGainChannelsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CampaignGainChannelsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
