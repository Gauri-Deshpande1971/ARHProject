import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CellsEventComponent } from './cells-event.component';

describe('CellsEventComponent', () => {
  let component: CellsEventComponent;
  let fixture: ComponentFixture<CellsEventComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CellsEventComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CellsEventComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
