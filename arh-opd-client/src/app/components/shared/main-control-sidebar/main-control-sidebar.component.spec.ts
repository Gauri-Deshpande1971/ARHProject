import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MainControlSidebarComponent } from './main-control-sidebar.component';

describe('MainControlSidebarComponent', () => {
  let component: MainControlSidebarComponent;
  let fixture: ComponentFixture<MainControlSidebarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MainControlSidebarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MainControlSidebarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
