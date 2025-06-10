import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HoursMinutesComponent } from './hours-minutes.component';

describe('HoursMinutesComponent', () => {
  let component: HoursMinutesComponent;
  let fixture: ComponentFixture<HoursMinutesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HoursMinutesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HoursMinutesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
